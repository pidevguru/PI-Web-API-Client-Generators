using GeneratorPIWebApiClient.Core;

namespace GeneratorPIWebApiClientDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneratorStarter generatorStarter = new GeneratorStarter(new DotNetDefinitionGenerator(), new DotNetControllerGenerator());
            generatorStarter.Start();
        }
    }
}
