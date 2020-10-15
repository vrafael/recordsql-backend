using System.Dynamic;
using System.Runtime.Serialization;

namespace RecordBackend.Models
{
    [DataContract]
    public class RpcResponse
    {
        //JsonRpc Version
        /*[DataMember(Name = "jsonrpc")]
        public string Version {get; set; }*/
        //Result
        [DataMember(Name = "result")]
        public object Result { set; get; }
        //Error
        [DataMember(Name = "error")]
        public RpcError Error { set; get; }
        //ID
        [DataMember(Name = "id")]
        public string ID { set; get; }
    }
}
