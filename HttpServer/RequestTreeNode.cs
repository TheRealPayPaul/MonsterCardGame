using Server.Enums;
using Server.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class RequestTreeNode
    {
        public string Name { get; private set; }
        public List<RequestTreeNode> ChildNodes { get; private set; }

        public bool IsVariable { get; private set; }

        public bool HasVariableChild { get { return _variableChild != null; } }
        private RequestTreeNode? _variableChild; 

        private Dictionary<RequestMethod, EndpointChain> _endpointChains { get; } = new();

        public RequestTreeNode(string name, bool isVariable = false)
        {
            Name = name;
            IsVariable = isVariable;
            ChildNodes = new List<RequestTreeNode>();
        }

        public void AddChild(RequestTreeNode node)
        {
            if (node.IsVariable && HasVariableChild)
                throw new InternalServerException($"[{nameof(RequestTreeNode)}] Variable TreeNode can't be added, since already one exists!");

            if (node.IsVariable)
                _variableChild = node;

            ChildNodes.Add(node);
        }

        public void AddEndpointChain(RequestMethod requestMethod, EndpointChain endpointChain)
        {
            if (_endpointChains.ContainsKey(requestMethod))
                throw new InternalServerException($"[{nameof(RequestTreeNode)}] An enpoint chain already exists for this route and request method! .../{Name}; {requestMethod}");

            _endpointChains.Add(requestMethod, endpointChain);
        }

        public EndpointChain? GetEndpointChain(RequestMethod requestMethod)
        {
            _endpointChains.TryGetValue(requestMethod, out EndpointChain? endpointChain);
            return endpointChain;
        }

        // Returns node with exact name and isVariable state
        public RequestTreeNode? FindChildExplicit(string name, bool isVariable)
        {
            foreach (RequestTreeNode node in ChildNodes)
            {
                if (node.Name == name && node.IsVariable == isVariable)
                    return node;
            }

            return null;
        }

        // Returns node that matches name and ist not variable
        // If no node was found and variable node exists
        // Return variable node
        public RequestTreeNode? FindChildImplicit(string name)
        {
            foreach (RequestTreeNode node in ChildNodes)
            {
                if (node.Name == name && node.IsVariable == false)
                    return node;
            }

            if (HasVariableChild)
                return _variableChild;

            return null;
        }

        public override string ToString()
        {
            return $"{Name}: {IsVariable}";
        }
    }
}
