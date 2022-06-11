using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IncomelBackend.Models
{
    public class ForgotPasswordValidationRequest
    {
        [Required, JsonProperty("token")]
        public string Token{ get; set; }
    }
}
