using Server.Converter;
using Server.Enums;
using Server.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Handlers
{
    internal static class ClientRequestHandler
    {
        public static HttpRequestObject Handle(NetworkStream networkStream)
        {
            string rawData = GetRawData(networkStream);
            var requestFragments = GetRequestStringFragments(rawData);
            MapRequestFragments(requestFragments, out HttpRequestObject requestObj);

            return requestObj;
        }

        private static string GetRawData(NetworkStream networkStream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            StringBuilder rawDataBuilder = new();
            while (networkStream.DataAvailable)
            {
                bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                string s = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                rawDataBuilder.Append(s);
            }

            return rawDataBuilder.ToString();
        }

        private static (string, List<string>, string) GetRequestStringFragments(string rawData)
        {
            using (StringReader stringReader = new(rawData))
            {
                string line;
                bool hasContent = false;

                string rawRequest;
                List<string> rawHeaders = new();
                string rawContent;

                // Read Request
                line = stringReader.ReadLine();
                if (line == null)
                    throw new Exception($"[{nameof(GetRequestStringFragments)}] Nothing sent");

                if (!Regex.IsMatch(line, @"^(GET|POST|PUT|DELETE) .+$", RegexOptions.IgnoreCase))
                    throw new Exception($"[{nameof(GetRequestStringFragments)}] Request section not valid");
                rawRequest = line;

                // Read Headers
                while (!string.IsNullOrWhiteSpace(line = stringReader.ReadLine()))
                {
                    rawHeaders.Add(line);
                    if (line.Contains("Content-Length:"))
                        hasContent = true;
                }

                // Read Content
                if (!hasContent)
                    return (rawRequest, rawHeaders, string.Empty);

                StringBuilder contentBuilder = new();
                while (!string.IsNullOrWhiteSpace(line = stringReader.ReadLine()))
                {
                    contentBuilder.Append(line);
                }
                rawContent = contentBuilder.ToString();

                return (rawRequest, rawHeaders, rawContent);
            }
        }

        private static void MapRequestFragments((string request, List<string> headers, string content) fragments, out HttpRequestObject httpRequestObj)
        {
            string[] requestFragments = fragments.request.Split(' ');
            string[] pathFragments = requestFragments[1].Split('?');

            httpRequestObj = new()
            {
                RequestType = RequestMethodConverter.StringToEnum(requestFragments[0]),
                Path = pathFragments[0],
                ContentString = fragments.content,
            };

            // Map Request Parameters
            if (pathFragments.Length > 1)
            {
                string[] rawRequestParameters = pathFragments[1].Split(';');
                foreach (string rawRequestParameter in rawRequestParameters)
                {
                    string[] requestParameterPair = rawRequestParameter.Split('=');
                    KeyValuePair<string, string> pair = new(requestParameterPair[0], requestParameterPair[1]);
                    httpRequestObj.RequestParameters.Add(pair);
                }
            }

            // Map Headers
            foreach (string rawHeader in fragments.headers)
            {
                string[] headerPair = rawHeader.Split(": ");
                KeyValuePair<string, string> pair = new(headerPair[0], headerPair[1]);
                httpRequestObj.Headers.Add(pair);
            }
        }
    }
}
