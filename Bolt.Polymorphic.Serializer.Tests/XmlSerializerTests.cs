using Bolt.Common.Extensions;
using Bolt.Polymorphic.Serializer.Tests.Fixtures;
using Bolt.Polymorphic.Serializer.Xml;

namespace Bolt.Polymorphic.Serializer.Tests;

public class XmlSerializerTests(SerializerFixture fixture) : IClassFixture<SerializerFixture>
{
    private readonly SerializerFixture _fixture = fixture;

    [Fact]
    public async Task Should_serialize()
    {
        var root = new Root();
        var d = new List<IElement>();
        d.Add(new Button{ Name = "btnSave", Text = "Save" });
        d.Add(new Label{ For = "txtFirstName", Text = "First Name"});
        d.Add(new Stack
        {
            Elements = new []
            {
                new Paragraph
                {
                    Text = "This is paragraph"
                }
            }
        });
        root.Elements = d.ToArray();

        var xmlSerializer = fixture.GetXmlSerializer();
        
        var gotStr = await xmlSerializer.Serialize(root, CancellationToken.None);
        
        using var data = ToMemoryStream(gotStr);
        var got = xmlSerializer.Deserialize<Root>(data);

        var a = got;
    }
    
    
    public static MemoryStream ToMemoryStream(string source)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(source);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}