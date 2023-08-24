using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorPIWebApiClient.Core.Models
{
    public class Schema
    {
        [JsonProperty("$ref")]
        public string @ref { get; set; }
        public AdditionalProperties additionalProperties { get; set; }
        public string type { get; set; }
        public Items items { get; set; }
    }
}
