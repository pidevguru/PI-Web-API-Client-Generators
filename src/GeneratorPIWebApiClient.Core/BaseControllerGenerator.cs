using GeneratorPIWebApiClient.Core.Models;
using System.Collections.Generic;

namespace GeneratorPIWebApiClient.Core
{
    public class BaseControllerGenerator
    {
        public void Generate(PIWebApiSwaggerSpec piWebApiSwaggerSpec)
        {

            Dictionary<string, List<HttpMethodData>> pathDic = new Dictionary<string, List<HttpMethodData>>();
            foreach (var path in piWebApiSwaggerSpec.paths)
            {
                foreach (var item in path.Value)
                {
                    var httpMethodData = item.Value;
                    string[] arr = httpMethodData.operationId.Split("_");
                    string controllerName = arr[0];
                    if (!pathDic.ContainsKey(controllerName))
                    {
                        pathDic[controllerName] = new List<HttpMethodData>();
                    }
                    string methodName = arr[1];
                    httpMethodData.httpVerb = item.Key;
                    httpMethodData.path = path.Key;
                    httpMethodData.function = methodName;
                    pathDic[controllerName].Add(httpMethodData);

                }
            }
            foreach (var controller in pathDic)
            {
                WriteControllerFile(controller.Key, controller.Value);
            }
        }

        public virtual void WriteControllerFile(string controllerName, List<HttpMethodData> httpMethods)
        {
        }
    }
}