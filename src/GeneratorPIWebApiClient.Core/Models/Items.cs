using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorPIWebApiClient.Core.Models
{
    public class Items
    {
        public string type { get; set; }

        [JsonProperty("$ref")]
        public string @ref { get; set; }
        public AdditionalProperties additionalProperties { get; set; }
    }
}
