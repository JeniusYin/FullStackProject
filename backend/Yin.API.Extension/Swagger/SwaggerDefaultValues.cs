using Yin.Infrastructure.Model;
using Yin.Infrastructure.SwaggerAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Yin.API.Extension.Swagger
{
    public class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
            // 同时标记 Obsolete 和 Deprecated = true
            operation.Deprecated = operation.Deprecated && apiDescription.IsDeprecated();
            var apiVersionStr = apiDescription.GetApiVersion()?.ToString() ?? string.Empty;
            var currentApi = apiDescription.RelativePath;
            if (operation.Deprecated)
            {
                var obs = apiDescription.ActionDescriptor.EndpointMetadata.OfType<ObsoleteAttribute>().FirstOrDefault();
                if (obs != null)
                {
                    var obsData = $"{obs.Message}|".Split("|", StringSplitOptions.RemoveEmptyEntries);
                    if (obsData.Length == 2)
                    {
                        if (operation.Extensions == null)
                        {
                            operation.Extensions = new Dictionary<string, IOpenApiExtension>()
                            {
                                {"deprecatedMessage",new OpenApiString(obs.Message?.Replace("|", "")) },
                                {"deprecatedCanContinue",new OpenApiBoolean(!obs.IsError) },
                                {"deprecatedNewApi",new OpenApiString(obsData[1].TrimStart('/')) }
                            };
                        }
                        else
                        {
                            operation.Extensions.Add("deprecatedMessage", new OpenApiString(obs.Message?.Replace("|", "")));
                            operation.Extensions.Add("deprecatedCanContinue", new OpenApiBoolean(!obs.IsError));
                            operation.Extensions.Add("deprecatedNewApi", new OpenApiString(obsData[1].TrimStart('/')));
                        }
                    }
                }
            }
            operation.Extensions.Add("nowApiPath", new OpenApiString(currentApi));
            operation.Extensions.Add("nowApiVersion", new OpenApiString(apiVersionStr));
            SetExtensions<SwaggerRoleDataAttribute>();
            SetExtensions<SwaggerScopeDataAttribute>();

            // 权限附加
            var attr = apiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().FirstOrDefault();
            if (attr != null)
            {
                operation.Extensions.Add(SwaggerConst.RoleName, new OpenApiString(attr.Roles));
                if (attr.Roles?.IndexOf(nameof(UserRoleType.Admin), StringComparison.Ordinal) > -1)
                {
                    if (operation.Tags?.Any() == true)
                    {
                        var name = Enum.GetName(typeof(SwaggerScopeEnum), SwaggerScopeEnum.管理端);
                        var tag = context.ApiDescription.ActionDescriptor.EndpointMetadata
                            .OfType<SwaggerTagAttribute>().FirstOrDefault();
                        if (tag != null)
                        {
                            if (!operation.Tags[0].Name.Contains(name))
                                operation.Tags[0].Name = name + "-" + tag.Description;
                        }
                    }
                }
            }

            // scope list 附加
            var scopeList = apiDescription.ActionDescriptor.EndpointMetadata.OfType<SwaggerScopeListDataAttribute>();
            if (scopeList?.Any() == true)
            {
                operation.Extensions.Add(SwaggerConst.ScopeList, scopeList.Aggregate(new OpenApiArray(), (t, o) =>
                {
                    t.Add(new OpenApiString(o.Value));
                    return t;
                }));
            }

            if (operation.Parameters == null)
            {
                return;
            }

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                {
                    parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
                }

                parameter.Required |= description.IsRequired;
            }

            void SetExtensions<T>() where T : SwaggerExtensionDataAttribute
            {
                var attr = apiDescription.ActionDescriptor.EndpointMetadata.OfType<T>().FirstOrDefault();
                if (attr != null)
                {
                    operation.Extensions.Add(attr.Key, new OpenApiString(attr.Value));
                }
            }
        }
    }
}
