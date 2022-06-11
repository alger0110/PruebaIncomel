using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class EmailConfig
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
    }
}
