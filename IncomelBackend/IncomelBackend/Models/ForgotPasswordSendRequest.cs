using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IncomelBackend.Models
{
    public class ForgotPasswordSendRequest
    {
        [Required, JsonProperty("email")]
        public string Email { get; set; }


        [Required, JsonProperty("birthDate")]
        public string BirthDate { get; set; }
    }
}
