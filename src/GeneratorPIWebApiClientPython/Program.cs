using GeneratorPIWebApiClient.Core;

namespace GeneratorPIWebApiClientPython
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneratorStarter generatorStarter = new GeneratorStarter(new PythonDefinitionGenerator(), new PythonControllerGenerator());
            generatorStarter.Start();
        }
    }
}
