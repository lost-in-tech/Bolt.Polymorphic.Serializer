using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bolt.Polymorphic.Serializer.Json;

public interface IJsonOptionsProvider
{
    JsonSerializerOptions Get();
}

internal sealed class JsonOptionsProvider : IJsonOptionsProvider
{
    private readonly ITypeRegistry _registry;
    private readonly Lazy<JsonSerializerOptions> _lazyOptions;

    public JsonOptionsProvider(ITypeRegistry registry)
    {
        this._registry = registry;
        _lazyOptions = new Lazy<JsonSerializerOptions>(Build);
    }

    private JsonSerializerOptions Build()
    {
        var opt = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        opt.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        var converterType = typeof(PolymorphicJsonConverter<>);
        
        foreach (var scannedType in _registry.TypesRegistered)
        {
            var constructedType = converterType.MakeGenericType(scannedType);

            if (Activator.CreateInstance(constructedType, _registry) is JsonConverter cvt)
            {
                opt.Converters.Add(cvt);
            }
        }
        
        return opt;
    }

    public JsonSerializerOptions Get() => _lazyOptions.Value;
}