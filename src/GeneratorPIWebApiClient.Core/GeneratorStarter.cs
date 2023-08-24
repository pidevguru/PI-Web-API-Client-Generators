using GeneratorPIWebApiClient.Core.Models;
using Newtonsoft.Json;
using System.IO;

namespace GeneratorPIWebApiClient.Core
{
    public class GeneratorStarter
    {
        private BaseDefinitionGenerator definitionGenerator;
        private BaseControllerGenerator controllerGenerator;

        public GeneratorStarter(BaseDefinitionGenerator definitionGenerator, BaseControllerGenerator controllerGenerator)
        {
            this.definitionGenerator = definitionGenerator;
            this.controllerGenerator = controllerGenerator;
        }

        public void Start()
        {
            StreamReader r = new StreamReader("swagger.json");
            string json = r.ReadToEnd();
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
            PIWebApiSwaggerSpec piWebApiSwaggerSpec = JsonConvert.DeserializeObject<PIWebApiSwaggerSpec>(json, settings);
            r.Dispose();
            PIWebApiGenerator piWebApiGenerator = new PIWebApiGenerator(this.definitionGenerator, this.controllerGenerator);
            piWebApiGenerator.Generate(piWebApiSwaggerSpec);
        }
    }
}
