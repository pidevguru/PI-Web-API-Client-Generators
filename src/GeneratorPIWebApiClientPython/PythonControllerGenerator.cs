using GeneratorPIWebApiClient.Core;
using GeneratorPIWebApiClient.Core.Extensions;
using GeneratorPIWebApiClient.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneratorPIWebApiClientPython
{
    public class PythonControllerGenerator : BaseControllerGenerator
    {

        public override void WriteControllerFile(string controllerName, List<HttpMethodData> httpMethods)
        {
            string modelName = controllerName + "ControllerClient";
            string fileName = controllerName.ToPythonVariableName() + "_controller_client.py";
            string filePath = "C:\\Git\\PI-Web-API-Client-Python\\pidevguru\\piwebapi\\controllers\\" + fileName;
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("from __future__ import absolute_import");
                writer.WriteLine("from six import iteritems");
                writer.WriteLine("");
                writer.WriteLine("");
                writer.WriteLine(string.Format("class {0}(object):", modelName));
                writer.WriteLine("    def __init__(self, api_client):");
                writer.WriteLine("        self.api_client = api_client");

                foreach (var httpMethod in httpMethods)
                {
                    writer.WriteLine("");
                    WriteInternalMethods(writer, httpMethod, false);                   
                    writer.WriteLine("");
                    WriteInternalMethods(writer, httpMethod, true);
                }
            }
        }

        protected void WriteInternalMethods(StreamWriter sw, HttpMethodData httpMethod, bool withHttp)
        {
            string response = GetResponse(httpMethod);
            string parametersWithOptionals = GetParameters(httpMethod.parameters, false, true);
            string parametersRequired = GetParameters(httpMethod.parameters, false, false);
            string parametersQuotes = GetParameters(httpMethod.parameters, true);
            string action = GetAction(httpMethod.function.ToFirstLetterLowerCase().ToPythonVariableName());
            action = withHttp == false ? action : action + "_with_http";

            if (response != "None")
            {
                response = $"'{response}'";
            }

            if (string.IsNullOrEmpty(parametersRequired) == false)
            {
                parametersRequired = parametersRequired + ", ";
            }
            if (string.IsNullOrEmpty(parametersWithOptionals) == false)
            {
                parametersWithOptionals = parametersWithOptionals + ", ";
            }


            sw.WriteLine($"    def {action}(self, {parametersWithOptionals}**kwargs):");
            if (withHttp == false)
            {

                sw.WriteLine("        kwargs['_return_http_data_only'] = True");
                sw.WriteLine("        if kwargs.get('callback'):");
                sw.WriteLine($"            return self.{action}_with_http({parametersRequired}**kwargs)");
                sw.WriteLine("        else:");
                sw.WriteLine($"            data = self.{action}_with_http({parametersRequired}**kwargs)");
                sw.WriteLine("            return data");
            }
            else
            {
                sw.WriteLine($"        all_params = list([{parametersQuotes}])");
                sw.WriteLine("        all_params.append('callback')");
                sw.WriteLine("        all_params.append('_return_http_data_only')");
                sw.WriteLine("        all_params.append('_preload_content')");
                sw.WriteLine("        all_params.append('_request_timeout')");
                sw.WriteLine("");
                sw.WriteLine("        params = locals()");
                sw.WriteLine("        for key, val in iteritems(params['kwargs']):");
                sw.WriteLine("            if key not in all_params:");
                sw.WriteLine("                raise TypeError(");
                sw.WriteLine("                    \"Got an unexpected keyword argument '%s'\"");
                sw.WriteLine($"                    \" to method {action}\" % key");
                sw.WriteLine("                )");
                sw.WriteLine("            params[key] = val");
                sw.WriteLine("        del params['kwargs']");
                sw.WriteLine("");
                foreach (var parameter in httpMethod.parameters.Where(p => p.required == true))
                {
                    sw.WriteLine($"        if ('{parameter.name.ToPythonVariableName()}' not in params) or (params['{parameter.name.ToPythonVariableName()}'] is None):");
                    sw.WriteLine($"            raise ValueError(\"Missing the required parameter `{parameter.name.ToPythonVariableName()}`\")");
                    sw.WriteLine("");
                }         
                sw.WriteLine("        collection_formats = {}");
                sw.WriteLine("");
                sw.WriteLine("        query_params = {}");
                sw.WriteLine("");
                sw.WriteLine("        path_params = {}");
                sw.WriteLine("");
                sw.WriteLine("        header_params = {}");
                sw.WriteLine("");
                sw.WriteLine("        form_params = []");
                sw.WriteLine("        local_var_files = {}");
                sw.WriteLine("");
                sw.WriteLine("        body_params = None");

                foreach (Parameter parameter in httpMethod.parameters)
                {

                    if (parameter.@in == "path")
                    {
                        sw.WriteLine(string.Format("        if '{0}' in params:", parameter.name.ToPythonVariableName()));
                        sw.WriteLine(string.Format("            if params['{0}'] is not None:", parameter.name.ToPythonVariableName()));
                        sw.WriteLine(string.Format("                path_params['{0}'] = params['{1}']", parameter.name.ToFirstLetterLowerCase(), parameter.name.ToPythonVariableName()));
                    }
                    if (parameter.@in == "query")
                    {
                        sw.WriteLine(string.Format("        if '{0}' in params:", parameter.name.ToPythonVariableName()));
                        sw.WriteLine(string.Format("            if params['{0}'] is not None:", parameter.name.ToPythonVariableName()));
                        sw.WriteLine(string.Format("                query_params['{0}'] = params['{1}']", parameter.name.ToFirstLetterLowerCase(), parameter.name.ToPythonVariableName()));
                        if (parameter.collectionFormat != null)
                        {
                            sw.WriteLine(string.Format("                collection_formats['{0}'] = 'multi'", parameter.name.ToFirstLetterLowerCase()));
                        }

                    }
                    else if (parameter.@in == "body")
                    {
                        sw.WriteLine(string.Format("        if '{0}' in params:", parameter.name.ToPythonVariableName()));
                        sw.WriteLine(string.Format("            body_params = params['{1}']", parameter.name.ToFirstLetterLowerCase(), parameter.name.ToPythonVariableName()));
                    }
                }


                sw.WriteLine("");
                sw.WriteLine("        header_params['Accept'] = self.api_client.\\");
                sw.WriteLine("            select_header_accept(['application/json', 'text/json', 'text/html', 'application/x-ms-application'])");
                sw.WriteLine("");
                sw.WriteLine("        header_params['Content-Type'] = self.api_client.\\");
                sw.WriteLine("            select_header_content_type([])");
                sw.WriteLine("");
                sw.WriteLine($"        return self.api_client.call_api('{httpMethod.path.Split('#')[0]}',");
                sw.WriteLine($"                                        '{httpMethod.httpVerb.ToUpper()}',");
                sw.WriteLine("                                        path_params,");
                sw.WriteLine("                                        query_params,");
                sw.WriteLine("                                        header_params,");
                sw.WriteLine("                                        body=body_params,");
                sw.WriteLine("                                        post_params=form_params,");
                sw.WriteLine("                                        files=local_var_files,");
                sw.WriteLine($"                                        response_type={response},");
                sw.WriteLine("                                        callback=params.get('callback'),");
                sw.WriteLine("                                        _return_http_data_only=params.get('_return_http_data_only'),");
                sw.WriteLine("                                        _preload_content=params.get('_preload_content', True),");
                sw.WriteLine("                                        _request_timeout=params.get('_request_timeout'),");
                sw.WriteLine("                                        collection_formats=collection_formats)");
            }
        }

        private string GetAction(string name)
        {
            if (name == "import")
            {
                return "import_data";
            }
            return name;
        }

        protected string GetParameters(List<Parameter> parametersList, bool includeSingleQuote, bool useOptionalArray = false)
        {
            string parameters = string.Empty;
            List<string> parametersNamesRequired = new List<string>();
            List<string> parametersNamesOptional = new List<string>();
            foreach (Parameter parameter in parametersList.Where(p => p.required == true))
            {
                parameters = parameters + string.Format("{0} {1}, ", ConvertToPythonType(parameter), parameter.name.ToPythonVariableName());
                parametersNamesRequired.Add(parameter.name.ToPythonVariableName());
            }
            foreach (Parameter parameter in parametersList.Where(p => p.required == false))
            {
                parameters = parameters + string.Format("{0} {1}, ", ConvertToPythonType(parameter), parameter.name.ToPythonVariableName());
                if (useOptionalArray == false)
                {
                    parametersNamesRequired.Add(parameter.name.ToPythonVariableName());
                }
                else
                {
                    parametersNamesOptional.Add(parameter.name.ToPythonVariableName());
                }
            }


            if (includeSingleQuote == false)
            {
                parameters = string.Empty;
                foreach (var param in parametersNamesRequired)
                {
                    parameters = parameters + string.Format("{0}, ", param.ToPythonVariableName());
                }

                foreach (var param in parametersNamesOptional)
                {
                    parameters = parameters + string.Format("{0}=None, ", param.ToPythonVariableName());
                }
            }
            else
            {
                parameters = string.Empty;
                foreach (var param in parametersNamesRequired)
                {
                    parameters = parameters + string.Format("'{0}', ", param.ToPythonVariableName());
                }

                foreach (var param in parametersNamesOptional)
                {
                    parameters = parameters + string.Format("'{0}', ", param.ToPythonVariableName());
                }
            }
            if (parameters.Length > 2)
            {
                parameters = parameters.Substring(0, parameters.Length - 2);
            }
            return parameters;
        }

        private string ConvertToPythonType(Parameter parameter)
        {
            string pythonType = null;
            string type = parameter.type;
            if (type == "string")
            {
                pythonType = "str";
            }
            else if (type == "number")
            {
                pythonType = "float";
            }
            else if (type == "boolean")
            {
                pythonType = "bool";
            }
            else if (type == "integer")
            {
                pythonType = "int";
            }
            else if (type == "array")
            {
                if (parameter.collectionFormat == "multi")
                {
                    pythonType = "list[str]";
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (type == "object")
            {
                throw new Exception();
            }
            else if (type == null)
            {
                if (parameter.schema == null)
                {
                    throw new Exception();
                }
                else if ((parameter.schema.type == "array") && (string.IsNullOrEmpty(parameter.schema.items.@ref)) == false)
                {
                    pythonType = string.Format("list[{0}]", parameter.schema.items.@ref);
                }
                else if (parameter.schema.@ref != null)
                {
                    pythonType = parameter.schema.@ref;
                }
                else if ((parameter.schema.type == "object") && (parameter.schema.additionalProperties == null))
                {
                    pythonType = "object";
                }
                else if ((parameter.schema.type == "object") && (parameter.schema.additionalProperties != null) && (parameter.schema.additionalProperties.@ref != null))
                {
                    pythonType = string.Format("dict(str, {0})", parameter.schema.additionalProperties.@ref);
                }
                else
                {
                    throw new Exception();
                }
            }
            return pythonType;
        }

        protected string GetResponse(HttpMethodData httpVerb)
        {
            string response = string.Empty;
            ResponseData httpResponse = httpVerb.responses.First().Value;
            if ((httpResponse.schema != null) && (httpResponse.schema.@ref != null))
            {
                response = httpResponse.schema.@ref.Replace("#/definitions/", "PWA").RemoveBracketsFromString();
            }
            else if (httpResponse.schema == null)
            {
                response = "None";
            }
            else if ((httpResponse.schema != null) && (httpResponse.schema.additionalProperties != null) && (httpResponse.schema.type == "object") && (httpResponse.schema.additionalProperties.@ref != null))
            {
                response = string.Format("dict(str, {0})", httpResponse.schema.additionalProperties.@ref.Replace("#/definitions/", "PWA").RemoveBracketsFromString());
            }
            else if ((httpResponse.schema != null) && (httpResponse.schema.additionalProperties != null) && (httpResponse.schema.type == "object") && (httpResponse.schema.additionalProperties.@ref == null))
            {
                response = string.Format("dict(str, {0})", "object");
            }
            else if ((httpResponse.schema != null) && (httpResponse.schema.additionalProperties == null) && (httpResponse.schema.type == "object"))
            {
                response = "object";
            }
            else
            {
                throw new Exception();
            }
            return response;
        }
    }
}