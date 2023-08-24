using GeneratorPIWebApiClient.Core.Models;

namespace GeneratorPIWebApiClient.Core
{
    public class PIWebApiGenerator
    {
        private BaseDefinitionGenerator definitionGenerator;
        private BaseControllerGenerator controllerGenerator;

        public PIWebApiGenerator(BaseDefinitionGenerator definitionGenerator, BaseControllerGenerator controllerGenerator)
        {
            this.definitionGenerator = definitionGenerator;
            this.controllerGenerator = controllerGenerator;
        }

        public void Generate(PIWebApiSwaggerSpec piWebApiSwaggerSpec)
        {
            this.definitionGenerator.Generate(piWebApiSwaggerSpec);
            this.controllerGenerator.Generate(piWebApiSwaggerSpec);
        }
    }
}
