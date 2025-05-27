using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;
using System;

namespace PSK.Server.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class HasPermissionAttribute : TypeFilterAttribute
    {
        public HasPermissionAttribute(PermissionName permission) : base(typeof(HasPermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }
}