using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace DomainNameApi.Internal;

/// <summary>The JSON settings used for request bodies and responses.</summary>
internal static class Json
{
    /// <summary>
    /// camelCase names, case-insensitive reading, lenient value types (null becomes the default, numbers
    /// may come as strings, enums as numbers or names), null lists kept empty, nulls left out of request
    /// bodies, and the request metadata (method, path, safety) never written.
    /// </summary>
    public static readonly JsonSerializerOptions Options = Create();

    private static JsonSerializerOptions Create()
    {
        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(RemoveRequestMetadata);
        resolver.Modifiers.Add(KeepCollectionsWhenNull);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = resolver,
        };
        options.Converters.Add(new LenientConverterFactory());
        return options;
    }

    /// <summary>
    /// A <c>null</c> array in a response leaves the property at its initial value (an empty list), so callers
    /// can always enumerate it.
    /// </summary>
    private static void KeepCollectionsWhenNull(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }
        foreach (var property in typeInfo.Properties)
        {
            var set = property.Set;
            if (set is null || property.PropertyType == typeof(string) ||
                !typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                continue;
            }
            property.Set = (target, value) =>
            {
                if (value is not null)
                {
                    set(target, value);
                }
            };
        }
    }

    /// <summary>
    /// Drops <see cref="BaseRequest.Method"/>, <see cref="BaseRequest.Path"/>, <see cref="BaseRequest.Safety"/>
    /// and <see cref="BaseRequest.IsIdempotent"/> from request bodies, including their overrides in derived classes.
    /// </summary>
    private static void RemoveRequestMetadata(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object || !typeof(BaseRequest).IsAssignableFrom(typeInfo.Type))
        {
            return;
        }
        for (var i = typeInfo.Properties.Count - 1; i >= 0; i--)
        {
            if (typeInfo.Properties[i].AttributeProvider is PropertyInfo property &&
                property.GetMethod?.GetBaseDefinition().DeclaringType == typeof(BaseRequest))
            {
                typeInfo.Properties.RemoveAt(i);
            }
        }
    }
}
