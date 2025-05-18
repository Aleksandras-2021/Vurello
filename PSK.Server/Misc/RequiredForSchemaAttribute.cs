using System;

namespace PSK.Server.Misc
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredForSchemaAttribute : Attribute
    {
    }
}
