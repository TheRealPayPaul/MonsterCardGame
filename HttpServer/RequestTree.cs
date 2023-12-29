using Server.Enums;
using System.Text.RegularExpressions;

namespace Server
{
    internal class RequestTree
    {
        private RequestTreeNode _root;

        public RequestTree()
        {
            _root = new RequestTreeNode("");
        }

        public void AddRoute(string path, RequestMethod requestMethod, EndpointChain endpointChain)
        {
            // Assures no '/' at the beginning
            if (path[0] == '/')
                path = path.Substring(1);

            // Assures no '/' at the end
            if (path.Length > 0 && path[path.Length - 1] == '/')
                path = path.Remove(path.Length - 1);

            // Handle root: ""
            if (path == "")
            {
                _root.AddEndpointChain(requestMethod, endpointChain);
            }

            // Validate path
            string[] pathFragments = path.Split('/');
            foreach (string pathFragment in pathFragments)
            {
                if (!Regex.IsMatch(pathFragment, @"^{[a-zA-Z0-9$\-_.]+}$|^[a-zA-Z0-9$\-_.]+$"))
                    throw new Exception($"[{nameof(RequestTree)}] Path cannot be added to route. Invalid Syntax: {path}");
            }

            // Traverse and add Path
            RequestTreeNode currentNode = _root;
            foreach (string pathFragment in pathFragments)
            {
                bool isVariable = Regex.IsMatch(pathFragment, @"^{[a-zA-Z0-9$\-_.]+}$");
                string nodeName = isVariable ? pathFragment.Substring(1, pathFragment.Length-2) : pathFragment;

                RequestTreeNode? foundNode = currentNode.FindChildExplicit(nodeName, isVariable);
                
                // Subnode exists
                if (foundNode != null)
                {
                    currentNode = foundNode;
                    continue;
                }

                // Create new subnode
                RequestTreeNode newNode = new(nodeName, isVariable);
                currentNode.AddChild(newNode);
                currentNode = newNode;
            }

            currentNode.AddEndpointChain(requestMethod, endpointChain);
        }

        public EndpointChain? GetEndpoint(HttpRequestObject reqObj)
        {
            string path = reqObj.Path;

            // Assures no '/' at the beginning
            if (path[0] == '/')
                path = path.Substring(1);

            // Assures no '/' at the end
            if (path.Length > 0 && path[path.Length - 1] == '/')
                path = path.Remove(path.Length - 1);

            // Handle root: ""
            if (path == "")
            {
                return _root.GetEndpointChain(reqObj.RequestType);
            }

            string[] pathFragments = path.Split('/');
            RequestTreeNode currentnode = _root;
            foreach (string pathFragment in pathFragments)
            {
                RequestTreeNode? nextNode = currentnode.FindChildImplicit(pathFragment);
                if (nextNode == null) return null;

                // Fill ReqObj with Dynamic Path Parameters
                if (nextNode.IsVariable)
                    reqObj.DynamicPathParameters.Add(nextNode.Name, pathFragment);

                currentnode = nextNode;
            }

            return currentnode.GetEndpointChain(reqObj.RequestType);
        }
    }
}
