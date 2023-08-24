using GeneratorPIWebApiClient.Core;
using GeneratorPIWebApiClient.Core.Extensions;
using GeneratorPIWebApiClient.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeneratorPIWebApiClientAngular
{
    public class AngularDefinitionGenerator : BaseDefinitionGenerator
    {
        public override void WriteModelIndex(Dictionary<string, DefinitionData> definitions)
        {
            string filePath = "C:\\Git\\PI-Web-API-Client-Angular\\projects\\piwebapi-angular\\src\\models\\models.ts";
            File.Delete(filePath);
            using (StreamWriter sw = File.AppendText(filePath))
            {
                foreach (var model in definitions)
                {
                    string modelName = "PWA" + model.Key.RemoveBracketsFromString();
                    sw.WriteLine("export { " + modelName + " } from './" + modelName + "';");
                }
            }

        }
        public override void WriteModelFile(KeyValuePair<string, DefinitionData> def)
        {

            string fileName = "PWA" + (def.Key).RemoveBracketsFromString() + ".ts";
            string filePath = "C:\\Git\\PI-Web-API-Client-Angular\\projects\\piwebapi-angular\\src\\models\\" + fileName;
            DefinitionData definitionData = def.Value;
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                string importTypeList = string.Empty;
             
                foreach (var prop in definitionData.properties)
                {
                    string propType = GetProperty(prop.Value.type, prop.Value.items, prop.Value.@ref, prop.Value.additionalProperties, prop.Value.properties);
                    if (propType.Contains("PWA"))
                    {
                        propType = GetPWAProperty(propType);
                        importTypeList += $"{propType}, ";
                    }
                }
                if (importTypeList.Length > 0)
                {
                    writer.WriteLine("import { " + importTypeList.Substring(0, importTypeList.Length -2) + " } from './models';");
                    writer.WriteLine("");
                }         
                writer.WriteLine("export class PWA" + (def.Key).RemoveBracketsFromString() + " {");
                string constrInputs = string.Empty;
                foreach (var prop in definitionData.properties)
                {
                    string propType = GetProperty(prop.Value.type, prop.Value.items, prop.Value.@ref, prop.Value.additionalProperties, prop.Value.properties);
                    writer.WriteLine("\tpublic {0}?: {1};", prop.Key, propType);
                    if (prop.Key.ToFirstLetterLowerCase() == "class")
                    {
                        constrInputs += string.Format("{0}Value?: {1}, ", prop.Key.ToFirstLetterLowerCase(), propType);
                    }
                    else
                    {
                        constrInputs += string.Format("{0}?: {1}, ", prop.Key.ToFirstLetterLowerCase(), propType);
                    }
                }
                if (constrInputs.Length > 0)
                {
                    constrInputs = constrInputs.Substring(0, constrInputs.Length - 2);
                }

                writer.WriteLine("\tconstructor(" + constrInputs + ") {");
                foreach (var prop in definitionData.properties)
                {
                    if (prop.Key.ToFirstLetterLowerCase() == "class")
                    {
                        writer.WriteLine(string.Format("\t\tif ({0}Value!=null) ", prop.Key.ToFirstLetterLowerCase()) + "{");
                        writer.WriteLine(string.Format("\t\t\tthis.{0}={1}Value", prop.Key, prop.Key.ToFirstLetterLowerCase()));
                        writer.WriteLine("\t\t}");
                    }
                    else
                    {
                        writer.WriteLine(string.Format("\t\tif ({0}!=null) ", prop.Key.ToFirstLetterLowerCase()) + "{");
                        writer.WriteLine(string.Format("\t\t\tthis.{0}={1}", prop.Key, prop.Key.ToFirstLetterLowerCase()));
                        writer.WriteLine("\t\t}");
                    }
                }
                writer.WriteLine("\t}");
                //sw.WriteLine("\t}");
                writer.WriteLine("}");
            }

        }

        private string GetPWAProperty(string type)
        {
            int startIndex = type.IndexOf("PWA");
            if (startIndex == 0)
            {
                return type;
            } 
            else
            {
                int endIndex = type.IndexOf(">");
                if (endIndex < 0)
                {
                    endIndex = type.IndexOf(";");
                }
                return type.Substring(startIndex, endIndex - startIndex);
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
                return "boolean";
            }
            else if (type == "integer")
            {
                return "number";
            }
            else if (type == "number")
            {
                return "number";
            }
            else if (type == "array")
            {
                if (items.type == "object")
                {
                    if ((items.additionalProperties != null) && (items.additionalProperties.type == "object"))
                    {
                        return "Array<{ [key: string]: any; }>";
                    }
                    else
                    {
                        return "Array<any>";
                    }
                }
                return "Array<" + GetProperty(items.type, null, items.@ref, additional, properties) + ">";
            }
            else if (type == "object")
            {
                if ((properties != null) && (properties.Count > 0))
                {
                    return "{ [key: string]: string; }";
                }
                else if (additional != null)
                {
                    if (additional.@ref != null)
                    {
                        string referenceModel = "PWA" + additional.@ref.Substring(14, additional.@ref.Length - 14).Trim().RemoveBracketsFromString();
                        return "{ [key: string]: "+ referenceModel + "; }";
                    }
                    else if (additional.type != null)
                    {
                        return "{ [key: string]: " + additional.type + "; }";
                    }
                }
                else
                {
                    return "any";
                }
            }
            else if (type == null)
            {
                return "PWA" + reference.Substring(14, reference.Length - 14).Trim().RemoveBracketsFromString();
            }
            throw new Exception("Not found");
        }
    }
}