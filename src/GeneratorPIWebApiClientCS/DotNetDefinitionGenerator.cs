using GeneratorPIWebApiClient.Core;
using GeneratorPIWebApiClient.Core.Extensions;
using GeneratorPIWebApiClient.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeneratorPIWebApiClientDotNet
{
    public class DotNetDefinitionGenerator : BaseDefinitionGenerator
    {
        public override void WriteModelFile(KeyValuePair<string, DefinitionData> def)
        {
            string fileName = "PWA" + (def.Key).RemoveBracketsFromString() + ".cs";
            string filePath = "C:\\Git\\PI-Web-API-Client-DotNet\\src\\PIDevGuru.PIWebApiClient\\Models\\" + fileName;
            DefinitionData definitionData = def.Value;
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("using System.Collections.Generic;");                
                writer.WriteLine("using System.Runtime.Serialization;");
                writer.WriteLine("");
                writer.WriteLine("namespace PIDevGuru.PIWebApiClient.Models");
                writer.WriteLine("{");
                writer.WriteLine("\t/// <summary>");
                writer.WriteLine(string.Format("\t/// {0}", (def.Key).RemoveBracketsFromString()));
                writer.WriteLine("\t/// </summary>");
                writer.WriteLine("\t[DataContract]");
                writer.WriteLine($"\tpublic class PWA{(def.Key).RemoveBracketsFromString()}");
                writer.WriteLine("\t{");
                string constrInputs = string.Empty;
                foreach (var prop in definitionData.properties)
                {
                    constrInputs += GetProperty(prop.Value.type, prop.Value.items, prop.Value.@ref, prop.Value.additionalProperties, prop.Value.properties).RemoveBracketsFromString() + " " + prop.Key + " = null, ";
                }
                if (constrInputs.Length > 0)
                {
                    constrInputs = constrInputs.Substring(0, constrInputs.Length - 2);
                }
                writer.WriteLine($"\t\tpublic PWA{(def.Key).RemoveBracketsFromString()}(" + constrInputs + ")");
                writer.WriteLine("\t\t{");              

                foreach (var prop in definitionData.properties)
                {
                    writer.WriteLine("\t\t\tthis." + prop.Key + " = " + prop.Key + ";");
                }
                writer.WriteLine("\t\t}");

                foreach (var prop in definitionData.properties)
                {
                    writer.WriteLine("\t\t/// <summary>");
                    writer.WriteLine(string.Format("\t\t/// Gets or Sets {0}", prop.Key));
                    writer.WriteLine("\t\t/// </summary>");
                    writer.WriteLine("\t\t[DataMember(Name = \"" + prop.Key + "\", EmitDefaultValue = false)]");
                    writer.WriteLine("\t\tpublic " + GetProperty(prop.Value.type, prop.Value.items, prop.Value.@ref, prop.Value.additionalProperties, prop.Value.properties).RemoveBracketsFromString() + " " + prop.Key + " { get; set; }");
                    writer.WriteLine("");
                }
                writer.WriteLine("\t}");
                writer.WriteLine("}");
            }

        }

        private string GetProperty(string type, Items items, string reference, PropertyData additional, Dictionary<string, PropertyData> properties)
        {
            if (type == "string")
            {
                return "string";
            }
            else if (type == "boolean")
            {
                return "bool?";
            }
            else if (type == "integer")
            {
                return "int?";
            }
            else if (type == "number")
            {
                return "double?";
            }
            else if (type == "array")
            {
                if (items.type == "object")
                {
                    if ((items.additionalProperties != null) && (items.additionalProperties.type == "object"))
                    {
                        return "List<Dictionary<string, object>>";
                    }
                    else
                    {
                        return "List<object>";
                    }
                }
                return "List<" + GetProperty(items.type, null, items.@ref, additional, properties) + ">";
            }
            else if (type == "object")
            {
                if ((properties != null) && (properties.Count > 0))
                {
                    return "Dictionary<string, string>";
                }
                else if (additional != null)
                {
                    if (additional.@ref != null)
                    {
                        string referenceModel = "PWA" + additional.@ref.Substring(14, additional.@ref.Length - 14).Trim();
                        return string.Format("Dictionary<string, {0}>", referenceModel);
                    }
                    else if (additional.type != null)
                    {
                        return string.Format("Dictionary<string, {0}>", additional.type);
                    }
                }
                else
                {
                    return "object";
                }
            }
            else if (type == null)
            {
                return "PWA" + reference.Substring(14, reference.Length - 14).Trim();
            }
            throw new Exception("Not found");
        }


    }
}
