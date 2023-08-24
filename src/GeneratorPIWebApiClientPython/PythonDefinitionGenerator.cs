using GeneratorPIWebApiClient.Core;
using GeneratorPIWebApiClient.Core.Extensions;
using GeneratorPIWebApiClient.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeneratorPIWebApiClientPython
{
    public class PythonDefinitionGenerator : BaseDefinitionGenerator
    {
        public override void WriteModelFile(KeyValuePair<string, DefinitionData> def)
        {
            string fileName = (def.Key).ToPythonFileName() + ".py";
            string filePath = "C:\\Git\\PI-Web-API-Client-Python\\pidevguru\\piwebapi\\models\\" + fileName;
            DefinitionData definitionData = def.Value;
            string modelName = def.Key;
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("from pprint import pformat");
                sw.WriteLine("from six import iteritems");
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine("class " + modelName.ToPIName() + "(object):");
                sw.WriteLine("    swagger_types = {");

                foreach (var prop in definitionData.properties)
                {
                    if (prop.Key.ToPythonVariableName() != "self" && prop.Key.ToPythonVariableName() != "class")
                    {
                        string propType = GetProperty(prop.Value.type, prop.Value.items, prop.Value.@ref, prop.Value.additionalProperties, prop.Value.properties);
                        sw.WriteLine("        '{0}': '{1}',", prop.Key.ToPythonVariableName(), propType);
                    }
                }
                sw.WriteLine("    }");
                sw.WriteLine("");
                sw.WriteLine("    attribute_map = {");
                foreach (var prop in definitionData.properties)
                {
                    if (prop.Key.ToPythonVariableName() != "self" && prop.Key.ToPythonVariableName() != "class")
                    {
                        sw.WriteLine("        '{0}': '{1}',", prop.Key.ToPythonVariableName(), prop.Key);
                    }
                }
                sw.WriteLine("    }");

                if (definitionData.properties.Count > 0)
                {
                    string constrInputs = string.Empty;
                    foreach (var prop in definitionData.properties)
                    {
                        if (prop.Key.ToPythonVariableName() != "self" && prop.Key.ToPythonVariableName() != "class")
                        {
                            constrInputs += prop.Key.ToPythonVariableName() + "=None, ";
                        }
                    }

                    if (constrInputs.Length > 2)
                    {
                        constrInputs = ", " + constrInputs.Substring(0, constrInputs.Length - 2);
                        sw.WriteLine("");
                        sw.WriteLine($"    def __init__(self{constrInputs}):");
                    }

                    sw.WriteLine("");
                }
                foreach (var prop in definitionData.properties)
                {
                    if (prop.Key.ToPythonVariableName() != "self" && prop.Key.ToPythonVariableName() != "class")
                    {
                        sw.WriteLine(string.Format("        self._{0} = None", prop.Key.ToPythonVariableName()));
                    } 
                }
                sw.WriteLine("");
                foreach (var prop in definitionData.properties)
                {
                    if (prop.Key.ToPythonVariableName() != "self" && prop.Key.ToPythonVariableName() != "class")
                    {
                        sw.WriteLine(string.Format("        if {0} is not None:", prop.Key.ToPythonVariableName()));
                        sw.WriteLine(string.Format("            self.{0} = {0}", prop.Key.ToPythonVariableName()));
                    }
                }


                foreach (var prop in definitionData.properties)
                {
                    if (prop.Key.ToPythonVariableName() != "self" && prop.Key.ToPythonVariableName() != "class")
                    {
                        sw.WriteLine("");
                        sw.WriteLine("    @property");
                        sw.WriteLine(string.Format("    def {0}(self):", prop.Key.ToPythonVariableName()));
                        sw.WriteLine(string.Format("        return self._{0}", prop.Key.ToPythonVariableName()));
                        sw.WriteLine("");
                        sw.WriteLine(string.Format("    @{0}.setter", prop.Key.ToPythonVariableName()));
                        sw.WriteLine(string.Format("    def {0}(self, {0}):", prop.Key.ToPythonVariableName()));
                        sw.WriteLine(string.Format("        self._{0} = {0}", prop.Key.ToPythonVariableName()));
                    }
                }


                sw.WriteLine("");


                sw.WriteLine("    def to_dict(self):");
                sw.WriteLine("        result = {}");
                sw.WriteLine("        for attr, _ in iteritems(self.swagger_types):");
                sw.WriteLine("            value = getattr(self, attr)");
                sw.WriteLine("            if isinstance(value, list):");
                sw.WriteLine("                result[attr] = list(map(");
                sw.WriteLine("                    lambda x: x.to_dict() if hasattr(x, \"to_dict\") else x,");
                sw.WriteLine("                    value");
                sw.WriteLine("                ))");
                sw.WriteLine("            elif hasattr(value, \"to_dict\"):");
                sw.WriteLine("                result[attr] = value.to_dict()");
                sw.WriteLine("            elif isinstance(value, dict):");
                sw.WriteLine("                result[attr] = dict(map(");
                sw.WriteLine("                    lambda item: (item[0], item[1].to_dict())");
                sw.WriteLine("                    if hasattr(item[1], \"to_dict\") else item,");
                sw.WriteLine("                    value.items()");
                sw.WriteLine("                ))");
                sw.WriteLine("            else:");
                sw.WriteLine("                result[attr] = value");
                sw.WriteLine("        return result");






                sw.WriteLine("");
                sw.WriteLine("    def to_str(self):");
                sw.WriteLine("        return pformat(self.to_dict())");
                sw.WriteLine("");
                sw.WriteLine("    def __repr__(self):");
                sw.WriteLine("        return self.to_str()");
                sw.WriteLine("");
                sw.WriteLine("    def __ne__(self, other):");
                sw.WriteLine("        return not self == other");
                sw.WriteLine("");
                sw.WriteLine("    def __eq__(self, other):");
                sw.WriteLine($"        if not isinstance(other, {modelName.ToPIName()}):");
                sw.WriteLine("            return False");
                sw.WriteLine("        return self.__dict__ == other.__dict__");
                sw.WriteLine("");

            }
        }    
        

        private string GetProperty(string type, Items items, string reference, PropertyData additional, Dictionary<string, PropertyData> properties)
        {
            if (type == "string")
            {
                return "str";
            }
            else if (type == "boolean")
            {
                return "bool";
            }
            else if (type == "integer")
            {
                return "int";
            }
            else if (type == "number")
            {
                return "float";
            }
            else if (type == "array")
            {
                if (items.type == "object")
                {
                    if ((items.additionalProperties != null) && (items.additionalProperties.type == "object"))
                    {
                        return "list[dict(str, object)]";
                    }
                    else
                    {
                        return "list[object]";
                    }
                }
                return "list[" + GetProperty(items.type, null, items.@ref, additional, properties) + "]";
            }
            else if (type == "object")
            {
                if ((properties != null) && (properties.Count > 0))
                {
                    return "dict(str, str)";
                }
                else if (additional != null)
                {
                    if (additional.@ref != null)
                    {
                        string referenceModel = "PWA" + additional.@ref.Substring(14, additional.@ref.Length - 14).Trim();
                        return string.Format("dict(str, {0})", referenceModel);
                    }
                    else if (additional.type != null)
                    {
                        return string.Format("dict(str, {0})", GetProperty(additional.type, null, null, null, null));
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
