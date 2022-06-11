using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class FrontendConfig
    {
        [JsonProperty("url")]
        public string URL { get; set; }
    }
}
