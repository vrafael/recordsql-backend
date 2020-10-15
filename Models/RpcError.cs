using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RecordBackend.Models
{
    [DataContract]
    public class RpcError
    {
        //Code
        [DataMember(Name = "code")]
        public short Code { set; get; }
        //Message
        [DataMember(Name = "message")]
        public string Message { set; get; }

        public RpcError(short code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
