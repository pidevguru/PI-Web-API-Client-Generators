using Newtonsoft.Json;
using System.Collections.Generic;

namespace GeneratorPIWebApiClient.Core.Models
{
    public class PropertyData
    {
        public dynamic example { get; set; }
        public string format { get; set; }
        public Items items { get; set; }
        public string type { get; set; }

        [JsonProperty("$ref")]
        public string @ref { get; set; }


        [JsonProperty("x-disallow-patch")]
        public bool xdisallowpatch { get; set; }

        [JsonProperty("x-disallow-post")]
        public bool xdisallowpost { get; set; }

        [JsonProperty("x-disallow-put")]
        public bool xdisallowput { get; set; }

        [JsonProperty("x-link-optional")]
        public bool xlinkoptional { get; set; }

        [JsonProperty("x-link-path")]
        public string xlinkpath { get; set; }

        [JsonProperty("additionalProperties")]
        public PropertyData additionalProperties { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, PropertyData> properties { get; set; }
    }
}