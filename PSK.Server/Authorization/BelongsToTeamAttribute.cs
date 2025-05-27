using Microsoft.AspNetCore.Mvc;
using System;

namespace PSK.Server.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BelongsToTeamAttribute : TypeFilterAttribute
    {
        public BelongsToTeamAttribute() : base(typeof(BelongsToTeamFilter))
        {
            Arguments = new object[] { };
        }
    }
}
