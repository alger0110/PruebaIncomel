using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IncomelBackend.Models
{
    public class LoginRequest
    {
        [Required,JsonProperty("username")]
        public string Username { get; set; }
        [Required, JsonProperty("password")]
        public string Password { get; set; }
    }
}
