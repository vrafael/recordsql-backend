using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Dynamic;

namespace RecordBackend.Models
{
    [DataContract]
    public class RpcRequest
    {
        //JsonRpc Version
        [DataMember(Name = "jsonrpc")]
        public string Version { get { return "2.0 + params only key-value array"; }}
        //Method
        [StringLength(128, ErrorMessage = "Method name must be between {2} and {1} characters long.", MinimumLength = 3)]
        [RegularExpression(@"((\[[a-zA-Z1-9]+\]|[a-zA-Z1-9]+)\.)?(\[[a-zA-Z1-9]+\]|[a-zA-Z1-9]+)", ErrorMessage = "Method name is incorrect")]
        [DataMember(Name = "method")]
        public string Method { get; set; }
        //Params
        //[MaxLength(1024, ErrorMessage = "Maximum method params is {1}")]
        [DataMember(Name = "params")]
        public ExpandoObject Params { get; set; }
        //ID
        [DataMember(Name = "id")]
        public string ID { get; set; }
    }
}
