namespace GeneratorPIWebApiClient.Core.Models
{
    public class Parameter
    {
        public string description { get; set; }
        public string @in { get; set; }
        public string name { get; set; }
        public bool required { get; set; }
        public string type { get; set; }
        public Schema schema { get; set; }
        public string collectionFormat { get; set; }
        public Items items { get; set; }
    }
}
