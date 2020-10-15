using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace RecordBackend.Models
{
    public class RpcParameter
    {
        [Required]
        [StringLength(128, ErrorMessage = "Parameter name must be between {2} and {1} characters long.", MinimumLength = 1)]
        [DataMember(Name = "params")]
        public string Name { get; set; }
        [MaxLength(128)]
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "value")]
        public object Value { get; set; }
    }
}
