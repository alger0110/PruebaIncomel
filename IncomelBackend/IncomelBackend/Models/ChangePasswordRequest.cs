using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IncomelBackend.Models
{
    public class ChangePasswordRequest
    {
        [Required, JsonProperty("email")]
        public string Email { get; set; }


        [Required, JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }
}
