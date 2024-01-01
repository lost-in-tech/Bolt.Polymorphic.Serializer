using Bolt.Polymorphic.Serializer.Tests.Fixtures;
using Bolt.Polymorphic.Serializer.Tests.Helpers;
using Bolt.Polymorphic.Serializer.Xml;
namespace Bolt.Polymorphic.Serializer.Tests;

public class XmlSerializerTests(SerializerFixture fixture) : IClassFixture<SerializerFixture>
{
    [Fact]
    public async Task Should_serialize()
    {
        var givenInput = BuildInput();

        var got = await fixture.GetXmlSerializer().Serialize(givenInput, CancellationToken.None);
        
        got!.ShouldMatchApprovedDefault();
    }

    [Fact]
    public async Task Should_deserialize()
    {
        var givenInput = BuildInput();

        var givenSerialized = await fixture.GetXmlSerializer().Serialize(givenInput, CancellationToken.None);

        var sut = fixture.GetXmlSerializer();
        
        var got = fixture.GetXmlSerializer().Deserialize<Root>(givenSerialized);
        
        fixture.GetJsonSerializer().Serialize(got).ShouldMatchApprovedDefault();
    }

    private Root BuildInput()
    {
        return new Root
        {
            StrValue = "this is string",
            Sub = new SubObject
            {
                StrValue = "this is sub string"
            },
            Btn = new Button
            {
                Text = "this is button property",
                Name = "btnSearch"
            },
            Elements = new IElement[]
            {
                new Button
                {
                    Text = "This is button",
                    Name = "btnSave"
                },
                
                new Label
                {
                    Text = "This is label",
                    For = "btnSave"
                }
            }
        };
    }
    
    public class Root
    {
        public string StrValue { get; set; }
        
        public SubObject Sub { get; set; }
        
        public IElement[] Elements { get; set; }
        public IElement Btn { get; set; }
    }
    
    public class SubObject
    {
        public string StrValue { get; set; }
    }
}