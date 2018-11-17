using Edison.Devices.Onboarding.Helpers;
using System;
using System.Collections.Generic;
using System.Net;

namespace Edison.Devices.Onboarding.Models
{
    internal class HttpRequest
    {
        public string RawRequest { get; private set; }
        public string Query { get; set; }
        public string HttpVersion { get; set; }
        public string Method { get; private set; }
        public string Body { get; set; }
        public Dictionary<string,string> QueryParameters { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public static HttpRequest FromString(string rawRequest)
        {
            string savetest = rawRequest;
            rawRequest = rawRequest.TrimEnd('\0');
            HttpRequest httpRequest = new HttpRequest
            {
                RawRequest = rawRequest
            };

            //Request parsing
            string[] rawRequestParse = rawRequest.Split('\n');
            string[] requestParse = rawRequestParse[0].Split(' ');
            if (requestParse.Length < 2)
                throw new Exception("HttpRequest: Not enough headers found");

            //Parse Request Data
            httpRequest.Method = requestParse[0]; //Method
            if (requestParse.Length >= 3)
                httpRequest.HttpVersion = requestParse[2].Trim(); //Http Version
            httpRequest.ParseQueryParameters(requestParse[1]); //Query parameters

            //Parse Headers
            httpRequest.ParseHeaders(rawRequestParse);

            //Parse Body
            httpRequest.ParseBody(rawRequestParse);

            return httpRequest;
        }

        private void ParseQueryParameters(string queryUrl)
        {
            QueryParameters = new Dictionary<string, string>();
            string[] queryParse = queryUrl.Split('?');
            Query = queryParse[0].ToLower();

            if (queryParse.Length > 1)
            {
                //Part after the ?
                string queryParams = queryParse[1];

                foreach (string paramStr in queryParams.Split('&'))
                {
                    string[] paramParse = paramStr.Split('=', 2);
                    string key = WebUtility.UrlDecode(paramParse[0].Trim()).ToLower();
                    if (!QueryParameters.ContainsKey(key))
                    {
                        if (paramParse.Length > 1)
                            QueryParameters.Add(key, WebUtility.UrlDecode(paramParse[1]));
                        else
                            QueryParameters.Add(key, null);
                    }
                }
            }
        }

        private void ParseHeaders(string[] rawRequestParse)
        {
            Headers = new Dictionary<string, string>();

            foreach (string requestLine in rawRequestParse)
            {
                if (requestLine.Length == 1 && requestLine[0] == '\r')
                    break;
                string[] header = requestLine.Split(':', 2);
                if (header.Length > 1)
                {
                    string key = WebUtility.UrlDecode(header[0].Trim()).ToLower();
                    string value = WebUtility.UrlDecode(header[1].Trim());
                    if (!Headers.ContainsKey(key))
                        Headers.Add(key, value);
                }
            }
        }

        private void ParseBody(string[] rawRequestParse)
        {
            if (Headers.ContainsKey("content-length"))
            {
                int bodySizeExpected = Convert.ToInt32(Headers["content-length"]);
                int bodyArrayStart = Array.IndexOf(rawRequestParse, "\r") + 1;
                Body = string.Join('\n', rawRequestParse, bodyArrayStart, rawRequestParse.Length - bodyArrayStart);

                if (bodySizeExpected != Body.Length)
                    DebugHelper.LogVerbose("HttpRequest: Content-Length does not match the size of body.");
            }
        }

        public bool ValidateHttpRequest()
        {
            if (Headers.Count < 3)
                return false;

            if(Method == "POST")
            {
                if (!Headers.ContainsKey("content-length"))
                    return false;

                int bodySizeExpected = Convert.ToInt32(Headers["content-length"]);
                if (bodySizeExpected != Body.Length)
                    return false;
            }

            //Add more checks

            return true;
        }
    }
}
