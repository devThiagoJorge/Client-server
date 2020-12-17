namespace Client.Classes
{
    public class AccessLogData
    {
        public string TimeStampTransmissionFinish { get; set; }
        public long TimeInMillisecondsRequestFinish { get; set; }
        public string ClientRequest { get; set; }
        public string ActionProxy { get; set; }
        public int RequestSizeInBytes { get; set; }
        public string HttpMethodResponse { get; set; }
        public string UrlRequest { get; set; }
        public string ClientName { get; set; }


    }
}
