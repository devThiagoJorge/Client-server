using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Classes
{
    public class AccessLogData
    {
        public AccessLogData(string time, long durationInMilliseconds, string clientRequest, string actionProxy, int requestSizeInBytes, string requestMethod, string urlRequest, string clientName, string proxyRequest, string fileDownload)
        {
            Time = time;
            DurationInMilliseconds = durationInMilliseconds;
            ClientRequest = clientRequest;
            ActionProxy = actionProxy;
            RequestSizeInBytes = requestSizeInBytes;
            RequestMethod = requestMethod;
            UrlRequest = urlRequest;
            ClientName = clientName;
            ProxyRequest = proxyRequest;
            FileDownload = fileDownload;
        }

        public string Time { get; private set; }
        public long DurationInMilliseconds { get; private set; }
        public string ClientRequest { get; private set; }
        public string ActionProxy { get; private set; }
        public int RequestSizeInBytes { get; private set; }
        public string RequestMethod { get; private set; }
        public string UrlRequest { get; private set; }
        public string ClientName { get; private set; }
        public string ProxyRequest { get; private set; }
        public string FileDownload { get; private set; }
    }
}
