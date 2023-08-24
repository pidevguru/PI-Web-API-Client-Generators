using Newtonsoft.Json;
using System.Collections.Generic;

namespace GeneratorPIWebApiClient.Core.Models
{
    public class PIWebApiSwaggerSpec
    {
        public string swagger { get; set; }
        public List<string> schemes { get; set; }
        public string host { get; set; }
        public string basePath { get; set; }
        public Info info { get; set; }
        
        public Dictionary<string, Dictionary<string, HttpMethodData>> paths { get; set; }
        
        public Dictionary<string, DefinitionData> definitions { get; set; }
    }

    public class Info
    {
        public string description { get; set; }

        public string termsOfService { get; set; }
        public string title { get; set; }
        public string version { get; set; }

        [JsonProperty("x-apisguru-categories")]
        public List<string> xapisgurucategories { get; set; }


        [JsonProperty("x-providerName")]
        public string xproviderName { get; set; }
    }



}