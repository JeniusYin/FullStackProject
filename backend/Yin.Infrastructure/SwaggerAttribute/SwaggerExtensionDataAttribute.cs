using System;
using System.Collections.Generic;
using System.Linq;

namespace Yin.Infrastructure.SwaggerAttribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerExtensionDataAttribute : Attribute
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }
    /// <summary>
    /// 接口权限
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SwaggerRoleDataAttribute : SwaggerExtensionDataAttribute
    {
        /// <summary>
        /// 接口权限
        /// 例如：PersonalUser
        /// </summary>
        /// <param name="value">例如：PersonalUser</param>
        public SwaggerRoleDataAttribute(string value)
        {
            Key = SwaggerConst.RoleName;
            Value = SwaggerConst.RoleValue(value);
        }
    }

    /// <summary>
    /// 使用范围
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SwaggerScopeDataAttribute : SwaggerExtensionDataAttribute
    {
        /// <summary>
        /// 适用范围，对个用 、隔开、管理端、自动任务
        /// 例如：APP、PC、Mobile
        /// </summary>
        /// <param name="scope">例如：APP、PC、Mobile、管理端、自动任务</param>
        public SwaggerScopeDataAttribute(string[] scope)
        {
            Key = SwaggerConst.ScopeName;
            Value = string.Join(",", scope);
        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class SwaggerScopeListDataAttribute : SwaggerExtensionDataAttribute
    {
        /// <summary>
        /// 适用范围，对个用 、隔开、管理端、自动任务
        /// 例如：APP、PC、Mobile
        /// </summary>
        /// <param name="scope">例如：APP、PC、Mobile、管理端、自动任务</param>
        public SwaggerScopeListDataAttribute(params SwaggerScopeEnum[] scope)
        {
            Key = SwaggerConst.ScopeList;
            Value = string.Join(",", scope?.Select(s => Enum.GetName(typeof(SwaggerScopeEnum), s)) ?? new List<string>());
        }
    }


    public enum SwaggerScopeEnum
    {
        App,
        Pc,
        Mobile,
        管理端,
        后台任务
    }

    public class SwaggerConst
    {
        public const string RoleName = "Role";

        public const string ScopeName = "Scope";

        public const string ScopeList = "ScopeList";

        public static string RoleValue(string role) => $"Roles = {role}";


    }
}
