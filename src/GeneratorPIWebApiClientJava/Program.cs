using GeneratorPIWebApiClient.Core;

namespace GeneratorPIWebApiClientJava
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneratorStarter generatorStarter = new GeneratorStarter(new JavaDefinitionGenerator(), new JavaControllerGenerator());
            generatorStarter.Start();
        }
    }
}
