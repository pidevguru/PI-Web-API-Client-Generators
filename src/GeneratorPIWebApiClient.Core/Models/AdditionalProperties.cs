using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorPIWebApiClient.Core.Models
{
    public class AdditionalProperties
    {
        [JsonProperty("$ref")]
        public string @ref { get; set; }
        public string type { get; set; }
    }
}
