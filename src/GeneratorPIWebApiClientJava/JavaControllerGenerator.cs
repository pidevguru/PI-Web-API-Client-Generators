using GeneratorPIWebApiClient.Core;
using GeneratorPIWebApiClient.Core.Extensions;
using GeneratorPIWebApiClient.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneratorPIWebApiClientJava
{
    public class JavaControllerGenerator : BaseControllerGenerator
    {
        public override void WriteControllerFile(string controllerName, List<HttpMethodData> httpMethods)
        {
            string modelName = controllerName + "ControllerClient";
            string fileName = controllerName + "ControllerClient.java";
            string filePath = "C:\\Git\\PI-Web-API-Client-Java\\src\\main\\java\\pidevguru\\piwebapi\\controllers\\" + fileName;
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("package pidevguru.piwebapi.controllers;");
                sw.WriteLine("import pidevguru.piwebapi.ApiCallback;");
                sw.WriteLine("import pidevguru.piwebapi.ApiClient;");
                sw.WriteLine("import pidevguru.piwebapi.ApiException;");
                sw.WriteLine("import pidevguru.piwebapi.ApiResponse;");
                sw.WriteLine("import pidevguru.piwebapi.Configuration;");
                sw.WriteLine("import pidevguru.piwebapi.Pair;");
                sw.WriteLine("import pidevguru.piwebapi.ProgressRequestBody;");
                sw.WriteLine("import pidevguru.piwebapi.ProgressResponseBody;");
                sw.WriteLine("import pidevguru.piwebapi.models.*;");
                sw.WriteLine("import com.google.gson.reflect.TypeToken;");
                sw.WriteLine("import java.io.IOException;");
                sw.WriteLine("import java.lang.reflect.Type;");
                sw.WriteLine("import java.util.ArrayList;");
                sw.WriteLine("import java.util.HashMap;");
                sw.WriteLine("import java.util.List;");
                sw.WriteLine("import java.util.Map;");

                sw.WriteLine("");
                sw.WriteLine(string.Format("public class {0} {1}", modelName, "{"));
                sw.WriteLine("\tprivate ApiClient apiClient;");
                sw.WriteLine($"\tpublic {modelName}(ApiClient apiClient){"{"}");
                sw.WriteLine("\t\tthis.apiClient = apiClient;");
                sw.WriteLine("\t}");

                sw.WriteLine("\tpublic ApiClient getApiClient() {");
                sw.WriteLine("\t\treturn apiClient;");
                sw.WriteLine("\t}");

                sw.WriteLine("\tpublic void setApiClient(ApiClient apiClient) {");
                sw.WriteLine("\t\tthis.apiClient = apiClient;");
                sw.WriteLine("\t}");

                foreach (var httpVerb in httpMethods)
                {
                    WriteInternalSummary(sw, httpVerb, false, false);
                    WriteInternalMethods(sw, httpVerb, false, false);
                    sw.WriteLine("");
                    WriteInternalSummary(sw, httpVerb, true, false);
                    WriteInternalMethods(sw, httpVerb, true, false);
                    sw.WriteLine("");
                    WriteInternalSummary(sw, httpVerb, false, true);
                    WriteInternalMethods(sw, httpVerb, false, true);
                    sw.WriteLine("");
                    WritePrivateMethodCall(sw, httpVerb);
                    sw.WriteLine("");
                }
                sw.WriteLine("}");
            }
        }

        private void WritePrivateMethodCall(StreamWriter sw, HttpMethodData httpMethod)
        {
            string parameters = GetParameters(httpMethod.parameters, true);
            if (string.IsNullOrEmpty(parameters) == false)
            {
                sw.WriteLine($"\tprivate okhttp3.Call {GetActionName(httpMethod)}Call({parameters}, final ProgressResponseBody.ProgressListener progressListener, final ProgressRequestBody.ProgressRequestListener progressRequestListener) throws ApiException {"{"}");
            }
            else
            {
                sw.WriteLine($"\tprivate okhttp3.Call {GetActionName(httpMethod)}Call(final ProgressResponseBody.ProgressListener progressListener, final ProgressRequestBody.ProgressRequestListener progressRequestListener) throws ApiException {"{"}");
            }
            sw.WriteLine("\t\tObject localVarPostBody =  null;");

            Parameter[] requiredParameters = httpMethod.parameters.Where(p => p.required == true).ToArray();
            for (int i = 0; i < requiredParameters.Count(); i++)
            {
                Parameter parameter = requiredParameters[i];
                sw.WriteLine(string.Format("\t\t// verify the required parameter '{0}' is set", parameter.name));
                sw.WriteLine(string.Format("\t\tif ({0} == null)", parameter.name));
                sw.WriteLine(string.Format("\t\t\tthrow new ApiException(\"Missing required parameter '{0}'\");", parameter.name));
            }

            sw.WriteLine("\t\tString localVarPath = \"{0}\";", httpMethod.path.Split('#')[0]);
            sw.WriteLine("\t\tMap<String, String> localVarHeaderParams = new HashMap<String, String>();");
            sw.WriteLine("\t\tMap<String, Object> localVarFormParams = new HashMap<String, Object>();");
            sw.WriteLine("\t\tList<Pair> localVarQueryParams = new ArrayList<Pair>();");

            sw.WriteLine("");
            sw.WriteLine("\t\tfinal String[] localVarAccepts = " + $"{"{"}" + "\"application/json\", \"text/json\", \"text/html\", \"application/x-ms-application\"" + $"{"};"}");
            sw.WriteLine("");
            sw.WriteLine("\t\tfinal String localVarAccept = apiClient.selectHeaderAccept(localVarAccepts);");
            sw.WriteLine("");
            sw.WriteLine("\t\tif (localVarAccept != null) localVarHeaderParams.put(\"Accept\", localVarAccept);");
            sw.WriteLine("");
            sw.WriteLine("\t\tfinal String[] localVarContentTypes = {\"application/json\", \"text/json\" };");
            sw.WriteLine("");
            sw.WriteLine("\t\tfinal String localVarContentType = apiClient.selectHeaderContentType(localVarContentTypes);");
            sw.WriteLine("\t\tlocalVarHeaderParams.put(\"Content-Type\", localVarContentType);");
            sw.WriteLine("");

            foreach (Parameter parameter in httpMethod.parameters)
            {
                if (parameter.@in == "path")
                {
                    sw.WriteLine("\t\tlocalVarPath = localVarPath.replaceAll(\"\\\\{" + parameter.name + "\\\\}\", " + $"apiClient.escapeString({parameter.name}.toString()));");
                }
                if (parameter.@in == "query")
                {
                    sw.WriteLine($"\t\tif ({parameter.name} != null)");
                    sw.WriteLine($"\t\t\tlocalVarQueryParams.addAll(apiClient.parameterToPairs(\"multi\", \"{parameter.name}\", {parameter.name}));");
                }
                else if (parameter.@in == "body")
                {
                    sw.WriteLine("\t\tlocalVarPostBody =  {0};", parameter.name);
                }
            }
            sw.WriteLine("\t\tif (progressListener != null)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tapiClient.getHttpClient().networkInterceptors().add(new okhttp3.Interceptor() {");
            sw.WriteLine("\t\t\t@Override");
            sw.WriteLine("\t\t\tpublic okhttp3.Response intercept(okhttp3.Interceptor.Chain chain) throws IOException {");
            sw.WriteLine("\t\t\t\tokhttp3.Response originalResponse = chain.proceed(chain.request());");
            sw.WriteLine("\t\t\t\treturn originalResponse.newBuilder()");
            sw.WriteLine("\t\t\t\t.body(new ProgressResponseBody(originalResponse.body(), progressListener))");
            sw.WriteLine("\t\t\t\t.build();");
            sw.WriteLine("\t\t\t\t}");
            sw.WriteLine("\t\t\t});");
            sw.WriteLine("\t\t}");
            sw.WriteLine("\t\tString[] localVarAuthNames = new String[] {\"Basic\" };");
            sw.WriteLine($"\t\treturn apiClient.buildCall(localVarPath, \"{httpMethod.httpVerb.ToUpper()}\", localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarAuthNames, progressRequestListener);");
            sw.WriteLine("\t}");
        }


        protected void WriteInternalSummary(StreamWriter sw, HttpMethodData httpMethod, bool withHttpinfo, bool isAsync)
        {
            string extra = string.Empty;
            if (withHttpinfo == true)
            {
                extra = "(with HTTP information)";
            }
            if (isAsync == true)
            {
                extra = "(asynchronously)";
            }
            sw.WriteLine("\t/**");
            sw.WriteLine("\t * {0} {1}", httpMethod.summary, extra);
            sw.WriteLine("\t *");
            sw.WriteLine("\t */");
        }

        protected void WriteInternalMethods(StreamWriter sw, HttpMethodData httpVerb, bool withHttpinfo, bool isAsync)
        {
            string response = GetResponse(httpVerb, withHttpinfo, isAsync);
            string parameters = GetParameters(httpVerb.parameters, true);
            string action = isAsync == false ? GetActionName(httpVerb) : GetActionName(httpVerb) + "Async";
            action = withHttpinfo == false ? action : action + "WithHttpInfo";

            if (isAsync == true)
            {
                if (string.IsNullOrEmpty(parameters) == false)
                {
                    parameters = parameters + ", final " + ConvertApiResponseToApiCallback(response);
                }
                else
                {
                    parameters = "final " + ConvertApiResponseToApiCallback(response);
                }
                response = "okhttp3.Call";
            }
            string parametersNoType = GetParameters(httpVerb.parameters, false);
            sw.WriteLine($"\tpublic {response} {action}({parameters}) throws ApiException {"{"}");
            if (isAsync == true)
            {
                sw.WriteLine("\t\tProgressResponseBody.ProgressListener progressListener = null;");
                sw.WriteLine("\t\tProgressRequestBody.ProgressRequestListener progressRequestListener = null;");
                sw.WriteLine("\t\tif (callback != null)");
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tprogressListener = new ProgressResponseBody.ProgressListener() {");
                sw.WriteLine("\t\t\t\t@Override");
                sw.WriteLine("\t\t\t\tpublic void update(long bytesRead, long contentLength, boolean done)");
                sw.WriteLine("\t\t\t\t{");
                sw.WriteLine("\t\t\t\t\tcallback.onDownloadProgress(bytesRead, contentLength, done);");
                sw.WriteLine("\t\t\t\t}");
                sw.WriteLine("\t\t\t};");
                sw.WriteLine("\t\t\tprogressRequestListener = new ProgressRequestBody.ProgressRequestListener() {");
                sw.WriteLine("\t\t\t\t@Override");
                sw.WriteLine("\t\t\t\tpublic void onRequestProgress(long bytesWritten, long contentLength, boolean done)");
                sw.WriteLine("\t\t\t\t{");
                sw.WriteLine("\t\t\t\t\tcallback.onUploadProgress(bytesWritten, contentLength, done);");
                sw.WriteLine("\t\t\t\t}");
                sw.WriteLine("\t\t\t};");
                sw.WriteLine("\t\t}");
                if (string.IsNullOrEmpty(parametersNoType) == false)
                {
                    sw.WriteLine($"\t\tokhttp3.Call call = {GetActionName(httpVerb)}Call({parametersNoType}, progressListener, progressRequestListener);");
                }
                else
                {
                    sw.WriteLine($"\t\tokhttp3.Call call = {GetActionName(httpVerb)}Call(progressListener, progressRequestListener);");
                }
                sw.WriteLine("\t\tapiClient.executeAsync(call, callback);");
                sw.WriteLine("\t\treturn call;");
            }
            else if (withHttpinfo == true)
            {
                if (GetResponse(httpVerb, true, false) == "ApiResponse<Void>")
                {
                    if (string.IsNullOrEmpty(parametersNoType) == false)
                    {
                        sw.WriteLine($"\t\tokhttp3.Call call = {GetActionName(httpVerb)}Call({parametersNoType},null,null);");
                    }
                    else
                    {
                        sw.WriteLine($"\t\tokhttp3.Call call = {GetActionName(httpVerb)}Call(null,null);");
                    }
                    sw.WriteLine("\t\treturn apiClient.execute(call);");
                }
                else
                {
                    if (string.IsNullOrEmpty(parametersNoType) == false)
                    {
                        sw.WriteLine($"\t\tokhttp3.Call call = {GetActionName(httpVerb)}Call({parametersNoType},null,null);");
                    }
                    else
                    {
                        sw.WriteLine($"\t\tokhttp3.Call call = {GetActionName(httpVerb)}Call(null,null);");
                    }
                    sw.WriteLine("\t\tType localVarReturnType = new TypeToken<" + GetResponse(httpVerb, false, false) + ">(){}.getType();");
                    sw.WriteLine("\t\treturn apiClient.execute(call, localVarReturnType);");
                }
            }
            else
            {
                if (GetResponse(httpVerb, true, false) == "ApiResponse<Void>")
                {
                    if (string.IsNullOrEmpty(parametersNoType) == false)
                    {
                        sw.WriteLine($"\t\t{action}WithHttpInfo({parametersNoType});");
                    }
                    else
                    {
                        sw.WriteLine($"\t\t{action}WithHttpInfo();");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(parametersNoType) == false)
                    {
                        sw.WriteLine($"\t\t{GetResponse(httpVerb, true, false)} resp = {action}WithHttpInfo({parametersNoType});");
                    }
                    else
                    {
                        sw.WriteLine($"\t\t{GetResponse(httpVerb, true, false)} resp = {action}WithHttpInfo();");
                    }
                    sw.WriteLine("\t\treturn resp.getData();");
                }
            }
            sw.WriteLine("\t}");
        }

        private string ConvertApiResponseToApiCallback(string response)
        {
            return response.Replace("ApiResponse<", "ApiCallback<") + " callback";
        }

        protected string GetResponse(HttpMethodData httpVerb, bool withHttpinfo, bool isAsync)
        {
            string response = string.Empty;
            ResponseData httpResponse = httpVerb.responses.First().Value;
            if ((httpResponse.schema != null) && (httpResponse.schema.@ref != null))
            {
                response = httpResponse.schema.@ref.Replace("#/definitions/", "PWA").RemoveBracketsFromString();
            }
            else if (httpResponse.schema == null)
            {
                response = "Object";
            }
            else if ((httpResponse.schema != null) && (httpResponse.schema.additionalProperties != null) && (httpResponse.schema.type == "object") && (httpResponse.schema.additionalProperties.@ref != null))
            {
                response = string.Format("Map<String, {0}>", httpResponse.schema.additionalProperties.@ref.Replace("#/definitions/", "PWA").RemoveBracketsFromString());
            }
            else if ((httpResponse.schema != null) && (httpResponse.schema.additionalProperties != null) && (httpResponse.schema.type == "object") && (httpResponse.schema.additionalProperties.@ref == null))
            {
                response = string.Format("Map<String, {0}>", "Object");
            }
            else if ((httpResponse.schema != null) && (httpResponse.schema.additionalProperties == null) && (httpResponse.schema.type == "object"))
            {
                response = "Object";
            }
            else
            {
                throw new Exception();
            }

            if (response == "Object")
            {
                response = "void";
            }

            if (isAsync == true)
            {
                withHttpinfo = true;

            }
            if (withHttpinfo == true)
            {
                if (response == "void")
                {
                    response = "ApiResponse<Void>";
                }
                else
                {
                    response = "ApiResponse<" + response + ">";
                }
            }
            return response;
        }

        protected string GetParameters(List<Parameter> parametersList, bool includeType)
        {
            string parameters = string.Empty;
            List<string> parametersNames = new List<string>();
            foreach (Parameter parameter in parametersList.Where(p => p.required == true))
            {
                parameters = parameters + string.Format("{0} {1}, ", GetVariableType(parameter), parameter.name);
                parametersNames.Add(parameter.name);
            }
            foreach (Parameter parameter in parametersList.Where(p => p.required == false))
            {
                parameters = parameters + string.Format("{0} {1}, ", GetVariableType(parameter), parameter.name);
                parametersNames.Add(parameter.name);
            }


            if (includeType == false)
            {
                parameters = string.Empty;
                foreach (var param in parametersNames)
                {
                    parameters = parameters + string.Format("{0}, ", param);
                }
            }
            if (parameters.Length > 2)
            {
                parameters = parameters.Substring(0, parameters.Length - 2);
            }
            return parameters;
        }


        protected string GetVariableType(Parameter parameter)
        {
            string javaType = null;
            string type = parameter.type;
            if (type == "string")
            {
                javaType = "String";
            }
            else if (type == "number")
            {
                javaType = "Double";
            }
            else if (type == "boolean")
            {
                javaType = "Boolean";
            }
            else if (type == "integer")
            {
                javaType = "Integer";
            }
            else if (type == "array")
            {
                if (parameter.collectionFormat == "multi")
                {
                    javaType = "List<String>";
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
                    javaType = string.Format("List<{0}>", parameter.schema.items.@ref.Replace("#/definitions/", "PWA").RemoveBracketsFromString());
                }
                else if (parameter.schema.type == null)
                {
                    javaType = parameter.schema.@ref.Replace("#/definitions/", "PWA").RemoveBracketsFromString();
                }
                else if ((parameter.schema.type == "object") && (parameter.schema.additionalProperties == null))
                {
                    javaType = "Object";
                }
                else if ((parameter.schema.type == "object") && (parameter.schema.additionalProperties != null) && (parameter.schema.additionalProperties.@ref != null))
                {
                    javaType = string.Format("Map<String, {0}>", parameter.schema.additionalProperties.@ref.Replace("#/definitions/", "PWA").RemoveBracketsFromString());
                }
                else
                {
                    throw new Exception();
                }
            }
            return javaType;
        }

        protected string GetActionName(HttpMethodData httpMethod)
        {
            string action = httpMethod.function.ToFirstLetterLowerCase();
            if (action == "import")
            {
                action = "importData";
            }
            return action;
        }
    }
}