using GeneratorPIWebApiClient.Core;
using GeneratorPIWebApiClient.Core.Models;
using GeneratorPIWebApiClient.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeneratorPIWebApiClientJava
{
    public class JavaDefinitionGenerator : BaseDefinitionGenerator
    {
        public override void WriteModelFile(KeyValuePair<string, DefinitionData> def)
        {
            string fileName = "PWA" + (def.Key).RemoveBracketsFromString() + ".java";
            string filePath = "C:\\Git\\PI-Web-API-Client-Java\\src\\main\\java\\pidevguru\\piwebapi\\models\\" + fileName;
            DefinitionData definitionData = def.Value;
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("package pidevguru.piwebapi.models;");
                writer.WriteLine("import java.util.Objects;");
                writer.WriteLine("import com.google.gson.annotations.SerializedName;");
                writer.WriteLine("import io.swagger.annotations.ApiModelProperty;");
                writer.WriteLine("import java.util.ArrayList;");
                writer.WriteLine("import java.util.List;");
                writer.WriteLine("import java.util.HashMap;");
                writer.WriteLine("import java.util.Map;");
                writer.WriteLine("import pidevguru.piwebapi.models.*;");
                writer.WriteLine("");

                writer.WriteLine($"public class PWA{(def.Key).RemoveBracketsFromString()}" + " {");


                foreach (var prop in definitionData.properties)
                {
                    if (prop.Key.ToLower() != "class")
                    {
                        writer.WriteLine("\t@SerializedName(\"" + prop.Key + "\")");
                        writer.WriteLine("\tprivate " + GetProperty(prop.Value.type, prop.Value.items, prop.Value.@ref, prop.Value.additionalProperties, prop.Value.properties).RemoveBracketsFromString() + " " + prop.Key.ToFirstLetterLowerCase() + " = null;");
                        writer.WriteLine("");
                    }
                }
                writer.WriteLine($"\tpublic PWA{(def.Key).RemoveBracketsFromString()}() " + " {");
                writer.WriteLine("\t}");
                writer.WriteLine("");
                foreach (var prop in definitionData.properties)
                {
                    if (prop.Key.ToLower() != "class")
                    {
                        string property = GetProperty(prop.Value.type, prop.Value.items, prop.Value.@ref, prop.Value.additionalProperties, prop.Value.properties).RemoveBracketsFromString();
                        writer.WriteLine($"\tpublic void set{prop.Key}({property} {prop.Key.ToFirstLetterLowerCase()})" + " { this." + prop.Key.ToFirstLetterLowerCase() + " = " + prop.Key.ToFirstLetterLowerCase() + ";}");
                        writer.WriteLine("");
                        writer.WriteLine($"\tpublic {property} get{prop.Key}() " + "{ return this." + prop.Key.ToFirstLetterLowerCase() + "; }");
                        writer.WriteLine("");
                    }
                }
                writer.WriteLine("}");
            }
        }

        private string GetProperty(string type, Items items, string reference, PropertyData additional, Dictionary<string, PropertyData> properties)
        {
            if (type == "string")
            {
                return "String";
            }
            else if (type == "boolean")
            {
                return "Boolean";
            }
            else if (type == "integer")
            {
                return "Integer";
            }
            else if (type == "number")
            {
                return "Double";
            }
            else if (type == "array")
            {
                if (items.type == "object")
                {
                    if ((items.additionalProperties != null) && (items.additionalProperties.type == "object"))
                    {
                        return "List<Map<String, Object>>";
                    }
                    else
                    {
                        return "List<Object>";
                    }
                }
                return "List<" + GetProperty(items.type, null, items.@ref, additional, properties) + ">";
            }
            else if (type == "object")
            {
                if ((properties != null) && (properties.Count > 0))
                {
                    return "Map<String, String>";
                }
                else if (additional != null)
                {
                    if (additional.@ref != null)
                    {
                        string referenceModel = "PWA" + additional.@ref.Substring(14, additional.@ref.Length - 14).Trim();
                        return string.Format("Map<String, {0}>", referenceModel);
                    }
                    else if (additional.type != null)
                    {
                        return string.Format("Map<String, {0}>", GetProperty(additional.type, null, null, null, null));
                    }
                }
                else
                {
                    return "Object";
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
