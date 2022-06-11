using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class TemplateEmailsConfig
    {
        [JsonProperty("forgotPassword")]
        public string ForgotPassword { get; set; }
    }
}
