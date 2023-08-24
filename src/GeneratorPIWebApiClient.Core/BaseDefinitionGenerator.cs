using GeneratorPIWebApiClient.Core.Models;
using System;
using System.Collections.Generic;

namespace GeneratorPIWebApiClient.Core
{
    public class BaseDefinitionGenerator
    {

        public void Generate(PIWebApiSwaggerSpec piWebApiSwaggerSpec)
        {
            foreach (var def in piWebApiSwaggerSpec.definitions)
            {
                WriteModelFile(def);
            }
            WriteModelIndex(piWebApiSwaggerSpec.definitions);
        }

        public virtual void WriteModelIndex(Dictionary<string, DefinitionData> definitions)
        {

        }

        public virtual void WriteModelFile(KeyValuePair<string, DefinitionData> def)
        {

        }
    }
}
        

    