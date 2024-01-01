using Bolt.Polymorphic.Serializer.Json;
using Bolt.Polymorphic.Serializer.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace Bolt.Polymorphic.Serializer.Tests.Fixtures;

public class SerializerFixture
{
    private readonly IServiceProvider _sp;
    
    public SerializerFixture()
    {
        var sc = new ServiceCollection();
        sc.AddPolymorphicSerializer(opt =>
        {
            opt.JsonSettings = new JsonSerializationSettings
            {
                WriteIndented = true
            };
            opt.AddSupportedType(typeof(IElement).Assembly, typeof(IElement));
        });
        
        _sp = sc.BuildServiceProvider();
    }

    public IJsonSerializer GetJsonSerializer() => _sp.GetRequiredService<IJsonSerializer>();
    public IXmlSerializer GetXmlSerializer() => _sp.GetRequiredService<IXmlSerializer>();
}


public record Root
{
    public IElement[]? Elements { get; set; }
}

public interface IElement
{
}

public interface ICompositeElement
{
    public IElement[]? Elements { get; set; }
}


public interface IHaveName
{
    public string Name { get; set; }
}

public class Button : IElement, IHaveName
{
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class Paragraph : IElement
{
    public required string Text { get; set; }
}

public class Stack : IElement, ICompositeElement
{
    public IElement[]? Elements { get; set; }
}

public class Label : IElement
{
    public required string For { get; set; }
    public required string Text { get; set; }
}