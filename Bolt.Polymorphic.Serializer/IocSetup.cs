using System.Reflection;
using Bolt.Polymorphic.Serializer.Json;
using Bolt.Polymorphic.Serializer.Xml;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddSingleton(options.JsonSettings ?? new JsonSerializationSettings());
        services.AddSingleton<ITypeRegistry>(_ => typeRegistry);
        services.AddSingleton<IJsonOptionsProvider,JsonOptionsProvider>();
        services.AddSingleton<IJsonSerializer, JsonSerializer>();

        services.AddSingleton<IXmlSerializer, XmlSerializer>();
        services.AddSingleton<XmlDeserializerHelper>();
        
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