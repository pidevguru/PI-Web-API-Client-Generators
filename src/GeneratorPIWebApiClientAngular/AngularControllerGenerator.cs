using GeneratorPIWebApiClient.Core;
using GeneratorPIWebApiClient.Core.Extensions;
using GeneratorPIWebApiClient.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneratorPIWebApiClientAngular
{
    public class AngularControllerGenerator : BaseControllerGenerator
    {
        public override void WriteControllerFile(string controllerName, List<HttpMethodData> httpMethods)
        {
            string modelName = controllerName + "ControllerClient";
            string fileName = controllerName + "ControllerClient.ts";
            string filePath = "C:\\Git\\PI-Web-API-Client-Angular\\projects\\piwebapi-angular\\src\\controllers\\" + fileName;
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("import { Inject, Injectable, Optional } from '@angular/core';");
                sw.WriteLine("import { HttpClient, HttpHeaders, HttpParams, HttpResponse, HttpEvent  } from '@angular/common/http';");
                sw.WriteLine("import { Observable } from 'rxjs';");
                string importTypeList = string.Empty;

                List<string> propTypeList = new List<string>();
                foreach (var httpVerb in httpMethods)
                {
                    propTypeList.AddRange(GetPWAProperties(httpVerb));

                }

                foreach (string propType in propTypeList.Distinct().OrderBy(s => s))
                {
                    importTypeList += $"{propType}, ";
                }
                if (importTypeList.Length > 0)
                {
                    sw.WriteLine("import { " + importTypeList.Substring(0, importTypeList.Length - 2) + "} from '../models/models';");
                    sw.WriteLine("");
                }

                sw.WriteLine("");
                sw.WriteLine("export class " + modelName + " {");
                sw.WriteLine("");
                sw.WriteLine("\tprivate basePath : string;");
                sw.WriteLine("\tprivate defaultHeaders : any;");
                sw.WriteLine("\tprivate withCredentials : boolean;");
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine("\tconstructor(protected http: HttpClient, basePath: string, defaultHeaders : any, withCredentials: boolean) {");
                sw.WriteLine("\t\tthis.basePath = basePath;");
                sw.WriteLine("\t\tthis.defaultHeaders = defaultHeaders;");
                sw.WriteLine("\t\tthis.withCredentials = withCredentials;");
                sw.WriteLine("\t}");
                sw.WriteLine("");

                foreach (var httpVerb in httpMethods)
                {
                    sw.WriteLine("");
                    WriteInternalMethods(sw, httpVerb);
                    sw.WriteLine("");
                }
                sw.WriteLine("}");
            }
        }

        private List<string> GetPWAProperties(HttpMethodData httpMethod)
        {
            List<string> list = new List<string>();
            string response = GetResponse(httpMethod);
            list.Add(response);

            foreach (Parameter parameter in httpMethod.parameters)
            {
                list.Add(GetType(parameter));
            }

            return list
                .Where(s => s.Contains("PWA"))
                .Select(s => GetPWAProperty(s))
                .Distinct()
                .OrderBy(s => s)
                .ToList();
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
                if (endIndex < 0)
                {
                    endIndex = type.IndexOf("}");
                }
                return type.Substring(startIndex, endIndex - startIndex).Trim();
            }
        }

        protected void WriteInternalMethods(StreamWriter sw, HttpMethodData httpMethod)
        {
            string response = GetResponse(httpMethod);
            string parameters = GetParameters(httpMethod.parameters);
            string action = httpMethod.function.ToFirstLetterLowerCase();

            //sw.WriteLine("\t\t}");
            sw.WriteLine("");
            string s = string.Format("\tpublic {0}({1}) : Observable<{2}>", action, parameters, response);
            sw.WriteLine(s + " {");

            int parameterPathCount = httpMethod.parameters.Where(p => p.@in == "path").Count();
            sw.Write($"\t\tconst localVarPath = this.basePath + '{httpMethod.path.Split('#')[0]}'");
            if (parameterPathCount > 0)
            {
                sw.WriteLine("");
                foreach (Parameter parameter in httpMethod.parameters)
                {
                    if (parameter.@in == "path")
                    {
                        sw.Write("\t\t\t.replace('{' + '" + parameter.name.ToFirstLetterLowerCase() + "' + '}', String(" + parameter.name.ToFirstLetterLowerCase() + "))");
                        parameterPathCount--;
                        if (parameterPathCount == 0)
                        {
                            sw.WriteLine(";");
                        }
                        else
                        {
                            sw.WriteLine("");
                        }
                    }
                }
            }
            else
            {
                sw.WriteLine(";");
            }
            sw.WriteLine("");
            sw.WriteLine("\t\tlet queryParameters = new HttpParams();");
            sw.WriteLine("\t\tlet headers = this.defaultHeaders;");
            sw.WriteLine("");
            foreach (var parameter in httpMethod.parameters.Where(p => p.required == true))
            {
                sw.WriteLine("\t\t\tif (" + parameter.name + " === null || " + parameter.name + " === undefined) {");
                sw.WriteLine(string.Format("\t\t\tthrow new Error('Required parameter {0} was null or undefined when calling {1}.');", parameter.name, action));
                sw.WriteLine("\t\t}");
                sw.WriteLine("");
            }



            string bodyVariableName = "null";
            foreach (Parameter parameter in httpMethod.parameters)
            {
                if (parameter.@in == "query")
                {
                    sw.WriteLine("\t\tif ((" + parameter.name + " !== undefined) && (" + parameter.name + " !== null)) {");
                    if (parameter.collectionFormat == "multi")
                    {
                        sw.WriteLine("\t\t\tfor (let item of " + parameter.name + ") {");
                        sw.WriteLine(string.Format("\t\t\t\tqueryParameters = queryParameters.append('{0}', item);", parameter.name));
                        sw.WriteLine("\t\t\t}");
                    }
                    else
                    {
                        sw.WriteLine(string.Format("\t\t\tqueryParameters = queryParameters.set('{0}', <any>{0});", parameter.name));
                    }
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("");
                }
                else if (parameter.@in == "body")
                {
                    bodyVariableName = parameter.name;
                }
            }


            if ((httpMethod.httpVerb.ToLower() == "get") || (httpMethod.httpVerb.ToLower() == "delete"))
            {
                sw.WriteLine($"\t\treturn this.http.{httpMethod.httpVerb.ToLower()}<{response}>(localVarPath, ");
            }
            else
            {
                sw.WriteLine($"\t\treturn this.http.{httpMethod.httpVerb.ToLower()}<{response}>(localVarPath, {bodyVariableName}, ");
            }
            sw.WriteLine("\t\t{");
            sw.WriteLine($"\t\t\tparams: queryParameters,");
            sw.WriteLine($"\t\t\twithCredentials: this.withCredentials,");
            sw.WriteLine($"\t\t\theaders: headers,");
            sw.WriteLine($"\t\t\tobserve: 'body',");
            sw.WriteLine($"\t\t\treportProgress: false,");
            sw.WriteLine("\t\t});");
            sw.WriteLine("\t}");
        }

        private string GetParameters(List<Parameter> parametersList)
        {
            string p = string.Empty;
            foreach (Parameter parameter in parametersList.Where(p => p.required == true))
            {
                p = p + string.Format("{1}: {0}, ", GetType(parameter), parameter.name.ToFirstLetterLowerCase());
            }
            foreach (Parameter parameter in parametersList.Where(p => p.required == false))
            {
                p = p + string.Format("{1}?: {0}, ", GetType(parameter), parameter.name.ToFirstLetterLowerCase());
            }
            if (p.Length > 2)
            {
                p = p.Substring(0, p.Length - 2);
            }
            return p;
        }

        private string GetType(Parameter parameter)
        {
            string returnType = null;
            string parameterType = parameter.type;
            if (parameterType == "string")
            {
                returnType = "string";
            }
            else if (parameterType == "number")
            {
                returnType = "number";
            }
            else if (parameterType == "boolean")
            {
                returnType = "boolean";
            }
            else if (parameterType == "integer")
            {
                returnType = "number";
            }
            else if (parameterType == "array")
            {
                if (parameter.collectionFormat == "multi")
                {
                    returnType = "Array<string>";
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (parameterType == "object")
            {
                throw new Exception();
            }
            else if (parameterType == null)
            {
                if (parameter.schema == null)
                {
                    throw new Exception();
                }
                else if ((parameter.schema.type == "array") && (string.IsNullOrEmpty(parameter.schema.items.@ref)) == false)
                {
                    returnType = string.Format("Array<{0}>", parameter.schema.items.@ref);
                }
                else if (parameter.schema.@ref != null)
                {
                    returnType = parameter.schema.@ref;
                }
                else if ((parameter.schema.type == "object") && (parameter.schema.additionalProperties == null))
                {
                    returnType = "any";
                }
                else if ((parameter.schema.type == "object") && (parameter.schema.additionalProperties != null) && (parameter.schema.additionalProperties.@ref != null))
                {
                    returnType = "{ [key: string]: " + parameter.schema.additionalProperties.@ref + "; }";
                }
                else
                {
                    throw new Exception();
                }
            }
            return returnType.Replace("#/definitions/", "PWA");
        }

        protected string GetResponse(HttpMethodData httpMethod)
        {
            string response = string.Empty;
            ResponseData httpResponse = httpMethod.responses.First().Value;
            if ((httpResponse.schema != null) && (httpResponse.schema.@ref != null))
            {
                response = string.Format("{0}", httpResponse.schema.@ref.Replace("#/definitions/", "PWA").RemoveBracketsFromString());
            }
            else if (httpResponse.schema == null)
            {
                response = "any";
            }
            else if ((httpResponse.schema != null) && (httpResponse.schema.additionalProperties != null) && (httpResponse.schema.type == "object") && (httpResponse.schema.additionalProperties.@ref != null))
            {
                response = "{[key: string]: " + httpResponse.schema.additionalProperties.@ref.Replace("#/definitions/", "PWA").RemoveBracketsFromString() + " }";
            }
            else if ((httpResponse.schema != null) && (httpResponse.schema.additionalProperties != null) && (httpResponse.schema.type == "object") && (httpResponse.schema.additionalProperties.@ref == null))
            {
                response = "{[key: string]: any }";
            }
            else if ((httpResponse.schema != null) && (httpResponse.schema.additionalProperties == null) && (httpResponse.schema.type == "object"))
            {
                response = "any";
            }
            else
            {
                throw new Exception();
            }
            return response;
        }
    }
}