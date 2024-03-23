using System.Reflection;
using Bolt.Polymorphic.Serializer.Json;
using Bolt.Polymorphic.Serializer.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.Polymorphic.Serializer;

public static class IocSetup
{
    public static IServiceCollection AddPolymorphicSerializer(
        this IServiceCollection services,
        Action<PolymorphicSerializerOptions> builder)
    {
        var options = new PolymorphicSerializerOptions();
        builder.Invoke(options);
        
        var typeRegistry = new TypeRegistry();
        foreach (var type in options.SupportedTypes)
        {
            typeRegistry.Register(type.Assembly, type.Types);
        }

        services.TryAddSingleton(options.JsonSettings ?? new JsonSerializationSettings());
        services.TryAddSingleton<ITypeRegistry>(_ => typeRegistry);
        services.TryAddSingleton<IJsonOptionsProvider,JsonOptionsProvider>();
        services.TryAddSingleton<IJsonSerializer, JsonSerializer>();

        services.TryAddSingleton<IXmlSerializer, XmlSerializer>();
        services.TryAddSingleton<XmlDeserializerHelper>();
        
        return services;
    }
}

public record PolymorphicSerializerOptions
{
    private readonly List<(Assembly, Type[])> _supportedTypes = new();

    public PolymorphicSerializerOptions AddSupportedType(Assembly assembly, params Type[] types)
    {
        _supportedTypes.Add((assembly, types));
        return this;
    }

    internal IEnumerable<(Assembly Assembly, Type[] Types)> SupportedTypes => _supportedTypes;
    
    public JsonSerializationSettings? JsonSettings { get; set; }
}

public record JsonSerializationSettings
{
    public bool WriteIndented { get; init; } = false;
}