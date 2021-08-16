using NJsonSchema;
using NSwag;
using NSwag.Generation.AspNetCore;

namespace CoreTemplate.API.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class NSwagExtensions
    {
        /// <summary>
        /// 在OpenApi中给接口自动添加头信息
        /// </summary>
        /// <param name="c"></param>
        public static void AddCustomHeaders(this AspNetCoreOpenApiDocumentGeneratorSettings c)
        {
            #region 自动添加头信息到Controller
            c.AddOperationFilter(t =>
            {
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "CityId",
                    Kind = OpenApiParameterKind.Header,
                    Description = "城市Id",
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = "local"
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "BrokerID",
                    Kind = OpenApiParameterKind.Header,
                    Description = "经纪人Id",
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = "a5a56a75-fc0c-4eae-b33a-a113d8eafece"
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "BrokerName",
                    Description = "经纪人",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = System.Net.WebUtility.UrlEncode("吖頭")
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "CompanyID",
                    Description = "公司Id",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = "01d152c8-88cc-4418-849f-346d03cac0ef"
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "CompanyName",
                    Description = "公司",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = System.Net.WebUtility.UrlEncode("哈尔滨市信息房地产有限公司")
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "DepartmentId",
                    Description = "部门Id",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = "fa522676-78b9-41ce-88b1-f34e4d07cfcd"
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "DepartmentName",
                    Description = "部门",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = System.Net.WebUtility.UrlEncode("销售部")
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "BigRegionId",
                    Description = "大区Id",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = "bb319ff2-48af-49b4-b43b-a17ea3cc078a"
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "BigRegionName",
                    Description = "大区",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = System.Net.WebUtility.UrlEncode("群力一区")
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "RegionId",
                    Description = "区域Id",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = "30528db3-e5d0-442a-a864-ed09eda3cca5"
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "RegionName",
                    Description = "区域",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = System.Net.WebUtility.UrlEncode("群力一区一部")
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "StoreID",
                    Description = "门店Id",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = "0f34380a-8a2d-44d8-9caf-860fe7092f2b"
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "StoreName",
                    Description = "门店",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = System.Net.WebUtility.UrlEncode("新城店")
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "GroupId",
                    Description = "店组Id",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = "3ce5a5ad-1b0c-40f1-877a-fc2a5580ae6f"
                });
                t.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "GroupName",
                    Description = "店组",
                    Kind = OpenApiParameterKind.Header,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                    Default = System.Net.WebUtility.UrlEncode("新城店一组")
                });
                return true;
            });
            #endregion
        }
    }


}
