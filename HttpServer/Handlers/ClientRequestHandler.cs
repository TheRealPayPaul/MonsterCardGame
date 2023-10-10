using Server.Converter;
using Server.Enums;
using Server.Exceptions;
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
        public static HttpRequestObject Handle(StreamReader streamReader)
        {
            (string rawData, char[] rawBody) = GetRawData(streamReader);
            var requestFragments = GetRequestStringFragments(rawData);
            MapRequestFragments(requestFragments, out HttpRequestObject requestObj);
            requestObj.RawBody = new(rawBody);

            return requestObj;
        }

        private static (string, char[]) GetRawData(StreamReader streamReader)
        {
            string? contentLengthHeader = null;
            string? line;
            StringBuilder rawDataBuilder = new();

            while (!string.IsNullOrEmpty((line = streamReader.ReadLine())))
            {
                rawDataBuilder.Append($"{line}\n");
                if (line.StartsWith("Content-Length: "))
                    contentLengthHeader = line;
            }

            if (contentLengthHeader == null)
                return (rawDataBuilder.ToString(), new char[0]);

            int bodyLength = int.Parse(contentLengthHeader.Substring(15) ?? "0");

            if (bodyLength < 1)
                return (rawDataBuilder.ToString(), new char[0]);

            int offset = 0;
            char[] rawBody = new char[bodyLength];
            while(offset < bodyLength)
            {
                int bytesRead = streamReader.Read(rawBody, offset, bodyLength - offset);
                offset += bytesRead;

                if (bytesRead == 0)
                    break;
            }

            return (rawDataBuilder.ToString(), rawBody);
        }

        private static (string, List<string>, string) GetRequestStringFragments(string rawData)
        {
            using (StringReader stringReader = new(rawData))
            {
                string? line;
                bool hasContent = false;

                string rawRequest;
                List<string> rawHeaders = new();
                string rawContent;

                // Read Request
                line = stringReader.ReadLine();
                if (line == null)
                    throw new BadRequestException($"[{nameof(ClientRequestHandler)}] Nothing sent");

                if (!Regex.IsMatch(line, @"^(GET|POST|PUT|DELETE) .+$", RegexOptions.IgnoreCase))
                    throw new BadRequestException($"[{nameof(ClientRequestHandler)}] Request section not valid");
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
                    httpRequestObj.RequestParameters.Add(requestParameterPair[0], requestParameterPair[1]);
                }
            }

            // Map Headers
            foreach (string rawHeader in fragments.headers)
            {
                string[] headerPair = rawHeader.Split(": ");
                httpRequestObj.Headers.Add(headerPair[0], headerPair[1]);
            }
        }
    }
}
