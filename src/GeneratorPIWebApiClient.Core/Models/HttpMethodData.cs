using System.Collections.Generic;

namespace GeneratorPIWebApiClient.Core.Models
{
    public class HttpMethodData
    {
        public string function;
        public string path;
        public string httpVerb;

        public List<string> consumes { get; set; }
        public bool deprecated { get; set; }
        public string operationId { get; set; }
        public List<Parameter> parameters { get; set; }
        public List<string> produces { get; set; }
        public Dictionary<string, ResponseData> responses { get; set; }
        public string summary { get; set; }
        public List<string> tags { get; set; }
        public string description { get; set; }
    }
}
