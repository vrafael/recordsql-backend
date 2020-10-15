using System;

namespace RecordBackend.Models
{
    public class RpcResponseContainer
    {
        public RpcResponse RpcResponse;
        public string SessionKey;
        public DateTime? ExpirationDate;

        public RpcResponseContainer()
        {
            RpcResponse = new RpcResponse();
        }
    }
}
