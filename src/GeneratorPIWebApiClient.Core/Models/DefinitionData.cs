using System.Collections.Generic;

namespace GeneratorPIWebApiClient.Core.Models
{
    public class DefinitionData
    {
        public Dictionary<string, PropertyData> properties { get; set; }
        public string type { get; set; }
    }
}