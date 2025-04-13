using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace PSK.Server.Misc
{
    public class MetadataSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var props = context.Type.GetProperties();

            foreach (var propSchema in schema.Properties)
            {
                var key = propSchema.Key;
                var propertySchema = propSchema.Value;

                var matchingProperty = props.FirstOrDefault(prop =>
                {
                    var jsonAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                    if (jsonAttr != null)
                        return jsonAttr.Name == key;

                    var defaultName = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);
                    return defaultName == key;
                });

                if (matchingProperty == null)
                    continue;

                var uiMetas = matchingProperty.GetCustomAttributes<MetadataAttribute>(true);
                foreach (var meta in uiMetas)
                {
                    propertySchema.Extensions[$"x-{meta.Key}"] = new Microsoft.OpenApi.Any.OpenApiString(meta.Value);
                }
            }
        }
    }

}
