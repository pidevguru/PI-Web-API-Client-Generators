using GeneratorPIWebApiClient.Core;
using GeneratorPIWebApiClient.Core.Extensions;
using GeneratorPIWebApiClient.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeneratorPIWebApiClientDotNet
{
    public class DotNetControllerGenerator : BaseControllerGenerator
    {
        public override void WriteControllerFile(string controllerName, List<HttpMethodData> httpMethods)
        {
            string fileName = controllerName + "ControllerClient.cs";
            string filePath = "C:\\Git\\PI-Web-API-Client-DotNet\\src\\PIDevGuru.PIWebApiClient\\Controllers\\" + fileName;
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("using PIDevGuru.PIWebApiClient.Client;");
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.Threading.Tasks;");
                writer.WriteLine("using PIDevGuru.PIWebApiClient.Exceptions;");
                writer.WriteLine("using PIDevGuru.PIWebApiClient.Models;");
                writer.WriteLine("using Newtonsoft.Json;");
                writer.WriteLine("using System.Net.Http;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("");
                writer.WriteLine("namespace PIDevGuru.PIWebApiClient.Controllers");
                writer.WriteLine("{");
                writer.WriteLine($"\tpublic class {controllerName}ControllerClient");
                writer.WriteLine("\t{");

                writer.WriteLine("\t\tprivate HttpApiClient httpApiClient;");
                writer.WriteLine($"\t\tpublic {controllerName}ControllerClient(HttpApiClient httpApiClient)");
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\tthis.httpApiClient = httpApiClient;");
                writer.WriteLine("\t\t}");

                foreach (var httpMethod in httpMethods)
                {
                    //Async
                    this.WriteInternalSummary(writer, controllerName, httpMethod, true);
                    writer.WriteLine("\t\tpublic " + GetAsyncResponseWithHttp(httpMethod.responses) + " " + httpMethod.function + "WithHttpAsync(" + GetMethodProperties(httpMethod) + ")");
                    writer.WriteLine("\t\t{");
                    CheckRequiredParameters(writer, httpMethod);
                    WritePathAsync(writer, httpMethod);
                    writer.WriteLine("\t\t}");
                    writer.WriteLine("");
                    this.WriteInternalSummary(writer, controllerName, httpMethod, false);
                    writer.WriteLine("\t\tpublic " + GetAsyncResponse(httpMethod.responses) + " " + httpMethod.function + "Async(" + GetMethodProperties(httpMethod) + ")");
                    writer.WriteLine("\t\t{");
                    writer.WriteLine("\t\t\treturn (await this." + httpMethod.function + "WithHttpAsync(" + GetMethodProperties(httpMethod, false) + ")).Data;");
                    writer.WriteLine("\t\t}");
                    writer.WriteLine("");

                    //Sync
                    this.WriteInternalSummary(writer, controllerName, httpMethod, true);
                    writer.WriteLine("\t\tpublic " + GetSyncResponseWithHttp(httpMethod.responses) + " " + httpMethod.function + "WithHttp(" + GetMethodProperties(httpMethod) + ")");
                    writer.WriteLine("\t\t{");
                    CheckRequiredParameters(writer, httpMethod);
                    WritePathSync(writer, httpMethod);
                    writer.WriteLine("\t\t}");
                    writer.WriteLine("");
                    this.WriteInternalSummary(writer, controllerName, httpMethod, false);
                    writer.WriteLine("\t\tpublic " + GetSyncResponse(httpMethod.responses) + " " + httpMethod.function + "(" + GetMethodProperties(httpMethod) + ")");
                    writer.WriteLine("\t\t{");
                    writer.WriteLine("\t\t\treturn (this." + httpMethod.function + "WithHttp(" + GetMethodProperties(httpMethod, false) + ")).Data;");
                    writer.WriteLine("\t\t}");
                    writer.WriteLine("");
                }
                writer.WriteLine("\t}");
                writer.WriteLine("}");
            }
        }

        private void WriteInternalSummary(StreamWriter sw, string controllerName, HttpMethodData httpVerb, bool withHttpinfo)
        {
            sw.WriteLine("\t\t/// <summary>");
            sw.WriteLine("\t\t/// {0}", httpVerb.summary);
            sw.WriteLine("\t\t/// </summary>");
            sw.WriteLine("\t\t/// <remarks>");
            sw.WriteLine("\t\t/// {0}", httpVerb.description);
            sw.WriteLine("\t\t/// </remarks>");
            sw.WriteLine("\t\t/// <exception cref=\"PIDevGuru.PIWebApiClient.Exceptions.HttpApiException\">Thrown when fails to make API call</exception>");
            foreach (Parameter param in httpVerb.parameters)
            {
                sw.WriteLine(string.Format("\t\t/// <param name=\"{0}\">{1}</param>", param.name, param.description));
            }
            if (withHttpinfo)
            {
                sw.WriteLine(string.Format("\t\t/// <returns>{0}</returns>", "HttpApiResponse<" + GetResponse(httpVerb.responses) + ">"));
            }
            else
            {
                sw.WriteLine(string.Format("\t\t/// <returns>{0}</returns>", GetResponse(httpVerb.responses)));
            }
        }

        private void WritePathAsync(StreamWriter writer, HttpMethodData httpMethod)
        {
            writer.WriteLine($"\t\t\tstring serviceUrl = \"{httpMethod.path.Split('#')[0]}\";");
            writer.WriteLine($"\t\t\tUrlBuilder urlBuilder = new UrlBuilder(serviceUrl);");
            foreach (Parameter parameter in httpMethod.parameters)
            {
                if (parameter.@in == "query")
                {
                    writer.WriteLine($"\t\t\turlBuilder.AddQueryParameter(\"{parameter.name}\", {parameter.name});");
                }
                else if (parameter.@in == "path")
                {
                    writer.WriteLine($"\t\t\turlBuilder.AddPathParameter(\"{parameter.name}\", {parameter.name});");
                }
                else if (parameter.@in == "body")
                {
                    writer.WriteLine($"\t\t\turlBuilder.AddBody({parameter.name});");
                }
            }
            writer.WriteLine($"\t\t\turlBuilder.HttpMethod = new HttpMethod(\"{httpMethod.httpVerb.ToUpper()}\");");
            writer.WriteLine("");
            writer.WriteLine($"\t\t\tHttpResponseMessage httpResponse = await this.httpApiClient.MakeHttpRequestAsync(urlBuilder);");
            writer.WriteLine("");
            writer.WriteLine($"\t\t\tint httpStatusCode = (int)httpResponse.StatusCode;");
            writer.WriteLine($"\t\t\tstring httpContent = await httpResponse.Content.ReadAsStringAsync();");
            writer.WriteLine("");
            writer.WriteLine($"\t\t\tException exception = await ExceptionFactory.DefaultExceptionFactory(\"AttributeGetCategories\", httpResponse);");
            writer.WriteLine($"\t\t\tif (exception != null) throw exception;");
            writer.WriteLine("");
            writer.WriteLine("\t\t\t" + GetResponse(httpMethod.responses) + " response = JsonConvert.DeserializeObject<" + GetResponse(httpMethod.responses) + ">(httpContent);");
            writer.WriteLine("\t\t\treturn new HttpApiResponse<" + GetResponse(httpMethod.responses) + ">(httpStatusCode, httpResponse.Headers.ToDictionary(x => x.Key, x => x.Value.First().ToString()), response);");
        }

        private void WritePathSync(StreamWriter writer, HttpMethodData httpMethod)
        {
            writer.WriteLine($"\t\t\tstring serviceUrl = \"{httpMethod.path.Split('#')[0]}\";");
            writer.WriteLine($"\t\t\tUrlBuilder urlBuilder = new UrlBuilder(serviceUrl);");
            foreach (Parameter parameter in httpMethod.parameters)
            {
                if (parameter.@in == "query")
                {
                    writer.WriteLine($"\t\t\turlBuilder.AddQueryParameter(\"{parameter.name}\", {parameter.name});");
                }
                else if (parameter.@in == "path")
                {
                    writer.WriteLine($"\t\t\turlBuilder.AddPathParameter(\"{parameter.name}\", {parameter.name});");
                }
                else if (parameter.@in == "body")
                {
                    writer.WriteLine($"\t\t\turlBuilder.AddBody({parameter.name});");
                }
            }
            writer.WriteLine($"\t\t\turlBuilder.HttpMethod = new HttpMethod(\"{httpMethod.httpVerb.ToUpper()}\");");
            writer.WriteLine("");
            writer.WriteLine($"\t\t\tHttpResponseMessage httpResponse = this.httpApiClient.MakeHttpRequest(urlBuilder);");
            writer.WriteLine("");
            writer.WriteLine($"\t\t\tint httpStatusCode = (int)httpResponse.StatusCode;");
            writer.WriteLine($"\t\t\tstring httpContent = httpResponse.Content.ReadAsStringAsync().Result;");
            writer.WriteLine("");
            writer.WriteLine($"\t\t\tException exception = ExceptionFactory.DefaultExceptionFactory(\"AttributeGetCategories\", httpResponse).Result;");
            writer.WriteLine($"\t\t\tif (exception != null) throw exception;");
            writer.WriteLine("");
            writer.WriteLine("\t\t\t" + GetResponse(httpMethod.responses) + " response = JsonConvert.DeserializeObject<" + GetResponse(httpMethod.responses) + ">(httpContent);");
            writer.WriteLine("\t\t\treturn new HttpApiResponse<" + GetResponse(httpMethod.responses) + ">(httpStatusCode, httpResponse.Headers.ToDictionary(x => x.Key, x => x.Value.First().ToString()), response);");
        }

        private void CheckRequiredParameters(StreamWriter writer, HttpMethodData httpMethod)
        {
            foreach (var item in httpMethod.parameters)
            {
                if (item.required)
                {
                    writer.WriteLine($"\t\t\tif ({item.name} == null)");
                    writer.WriteLine("\t\t\t{");
                    writer.WriteLine("\t\t\t\tthrow new HttpApiException(400, \"Missing required parameter \'" + item.name + "\'\");");
                    writer.WriteLine("\t\t\t}");
                    writer.WriteLine();
                }
            }
        }

        private string GetAsyncResponseWithHttp(Dictionary<string, ResponseData> responses)
        {
            foreach (var response in responses)
            {
                int key = Convert.ToInt32(response.Key);
                if (key < 300 && responses[key.ToString()].schema != null)
                {
                    return "async Task<HttpApiResponse<" + (GetProperty(null, null, responses[key.ToString()].schema).RemoveBracketsFromString()) + ">>";
                }
            }
            return "async Task<HttpApiResponse<object>>";
            
        }

        private string GetSyncResponseWithHttp(Dictionary<string, ResponseData> responses)
        {
            foreach (var response in responses)
            {
                int key = Convert.ToInt32(response.Key);
                if (key < 300 && responses[key.ToString()].schema != null)
                {
                    return "HttpApiResponse<" + (GetProperty(null, null, responses[key.ToString()].schema).RemoveBracketsFromString()) + ">";
                }
            }
            return "HttpApiResponse<object>";

        }

        private string GetSyncResponse(Dictionary<string, ResponseData> responses)
        {
            foreach (var response in responses)
            {
                int key = Convert.ToInt32(response.Key);
                if (key < 300 && responses[key.ToString()].schema != null)
                {
                    return (GetProperty(null, null, responses[key.ToString()].schema).RemoveBracketsFromString());
                }
            }
            return "object";
        }

        private string GetAsyncResponse(Dictionary<string, ResponseData> responses)
        {
            foreach (var response in responses)
            {
                int key = Convert.ToInt32(response.Key);
                if (key < 300 && responses[key.ToString()].schema != null)
                {
                    return "async Task<" + (GetProperty(null, null, responses[key.ToString()].schema).RemoveBracketsFromString()) + ">";
                }
            }
            return "async Task<object>";
        }

        private string GetResponse(Dictionary<string, ResponseData> responses)
        {
            foreach (var response in responses)
            {
                int key = Convert.ToInt32(response.Key);
                if (key < 300 && responses[key.ToString()].schema != null)
                {
                    return GetProperty(null, null, responses[key.ToString()].schema).RemoveBracketsFromString();
                }
            }
            return "object";
        }

        private string GetMethodProperties(HttpMethodData httpMethod, bool includeTypes = true)
        {
            string inputs = string.Empty;
            foreach (var item in httpMethod.parameters)
            {
                if (item.required)
                {
                    if (includeTypes)
                    {
                        inputs += GetProperty(item.type, item.items, item.schema).RemoveBracketsFromString() + " " + item.name + ", ";
                    }
                    else
                    {
                        inputs += item.name + ", ";
                    }
                }
            }
            foreach (var item in httpMethod.parameters)
            {
                if (!item.required)
                {
                    if (includeTypes)
                    {
                        string property = GetProperty(item.type, item.items, item.schema).RemoveBracketsFromString();
                        if (property == "string" || property == "List<string>")
                        {
                            inputs += property + " " + item.name + " = null, ";
                        }
                        else
                        {
                            inputs += property + "? " + item.name + " = null, ";
                        }
                    }
                    else
                    {
                        inputs += item.name + ", ";
                    }

                }
            }
            if (inputs.Length > 2)
            {
                inputs = inputs.Substring(0, inputs.Length - 2);
            }
            return inputs;
        }

        private string GetProperty(string type, Items items, Schema schema)
        {
            if (type == "string")
            {
                return "string";
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
                return "double";
            }
            else if (type == "array" && items.type != null)
            {
                return "List<" + GetProperty(items.type, null, null) + ">";
            }
            else if (type == "array" && items.@ref != null)
            {
                string refernce = "PWA" + items.@ref.Substring(14, items.@ref.Length - 14).Trim().RemoveBracketsFromString();
                return "List<" + refernce + ">";
            }
            else if (type == "object")
            {
                return "object";
            }
            else if (type == null && schema.@ref != null)
            {
                return "PWA" + schema.@ref.Substring(14, schema.@ref.Length - 14).Trim();
            }
            else if (type == null && schema.type != null && schema.additionalProperties != null && schema.additionalProperties.@ref != null)
            {
                string reference = "PWA" + schema.additionalProperties.@ref.Substring(14, schema.additionalProperties.@ref.Length - 14).Trim().RemoveBracketsFromString();
                return "Dictionary<string, " + reference + ">";
            }
            else if (type == null && schema.type != null && schema.additionalProperties != null && schema.additionalProperties.type != null)
            {
                return "Dictionary<string, " + GetProperty(schema.additionalProperties.type, null, null) + ">";
            }
            else if (type == null && schema.type != null)
            {
                return GetProperty(schema.type, schema.items, null).RemoveBracketsFromString();
            }
            throw new Exception("Not found");
        }
    }
}
