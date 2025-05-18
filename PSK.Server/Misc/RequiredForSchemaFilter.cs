using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace PSK.Server.Misc
{
    public class RequiredForSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var props = context.Type.GetProperties();
            var requiredForSchema = new List<string>();

            foreach (var prop in props)
            {
                // Find the OpenAPI property name (respecting JsonPropertyName if present)
                var jsonAttr = prop.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>();
                var propName = jsonAttr != null
                    ? jsonAttr.Name
                    : char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);

                if (prop.GetCustomAttribute<RequiredForSchemaAttribute>() != null && schema.Properties.ContainsKey(propName))
                {
                    requiredForSchema.Add(propName);
                }
            }

            if (requiredForSchema.Count > 0)
            {
                if (schema.Required == null)
                    schema.Required = new HashSet<string>();
                foreach (var req in requiredForSchema)
                {
                    if (!schema.Required.Contains(req))
                        schema.Required.Add(req);
                }
            }
        }
    }
}
