namespace RecordBackend.Models
{
    public class Identity
    {
        public string UserAgent;
        public string SessionKey;
        public string IPAddress;

        public Identity(string sessionKey, string userAgent, string ipAddress)
        {
            SessionKey = sessionKey;
            UserAgent = userAgent;
            IPAddress = ipAddress;
        }
    }
}
