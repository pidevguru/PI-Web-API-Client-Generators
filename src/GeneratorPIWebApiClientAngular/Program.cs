using GeneratorPIWebApiClient.Core;

namespace GeneratorPIWebApiClientAngular
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneratorStarter generatorStarter = new GeneratorStarter(new AngularDefinitionGenerator(), new AngularControllerGenerator());
            generatorStarter.Start();
        }
    }
}
