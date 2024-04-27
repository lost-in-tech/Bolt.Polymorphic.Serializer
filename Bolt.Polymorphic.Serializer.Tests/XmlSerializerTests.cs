using Bolt.Common.Extensions;
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

        var sut = fixture.GetXmlSerializer();
        
        var givenSerialized = await sut.Serialize(givenInput, CancellationToken.None);
        
        var got = sut.Deserialize<TestObject>(givenSerialized);
        
        fixture.GetJsonSerializer().Serialize(got).ShouldMatchApprovedDefault();
    }

    [Fact]
    public void Should_deserialized_from_simple_property_that_provided_as_child_node()
    {
        var sut = fixture.GetXmlSerializer();

        var got = sut.Deserialize<Simple>("<Simple><Text>Hello World!</Text></Simple>");

        got.ShouldNotBeNull();
        got.Text.ShouldBe("Hello World!");
    }
    
    private class Simple
    {
        public string Text { get; set; }
    }

    private TestObject BuildInput()
    {
        return new TestObject
        {
            StrValue = "this is string",
            Sub = new SubObject
            {
                StrValue = "this is sub string",
                ArrayOfValues = new []{"Test1","test2"}
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
            },
            Colour = Colour.Green,
            SectionNames = new []{"section-one","section-two"},
            SectionIntValues = new []{1,2}
        };
    }
    
    public class TestObject
    {
        public string StrValue { get; set; }
        
        public SubObject Sub { get; set; }
        
        public IElement[] Elements { get; set; }
        public IElement Btn { get; set; }
        public Colour Colour { get; set; }
    
        public string[] SectionNames { get; set; }
        public int[] SectionIntValues { get; set; }
        public Dictionary<string,string> SampleNameValues { get; set; }
    }
    
    public class SubObject
    {
        public string StrValue { get; set; }
        public string[] ArrayOfValues { get; set; }
    }
    
    public enum Colour
    {
        Red,
        Green,
        Blue
    }
}