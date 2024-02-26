using Bolt.Polymorphic.Serializer.Tests.Fixtures;
using Bolt.Polymorphic.Serializer.Tests.Helpers;

namespace Bolt.Polymorphic.Serializer.Tests;

public class JsonSerializerTests(SerializerFixture fixture) : IClassFixture<SerializerFixture>
{
    private readonly SerializerFixture _fixture = fixture;

    [Fact]
    public void Should_serialize()
    {
        var givenInput = BuildInput();
        var sut = fixture.GetJsonSerializer();
        var got = sut.Serialize(givenInput);
        got.ShouldMatchApprovedDefault();
    }

    [Fact]
    public void Should_deserialize()
    {
        var givenInput = BuildInput();
        var sut = fixture.GetJsonSerializer();
        var gotSerialized = sut.Serialize(givenInput);
        var gotDeserialized = sut.Deserialize<TestObject>(gotSerialized);
        _fixture.GetJsonSerializer().Serialize(gotDeserialized).ShouldMatchApprovedDefault();
    }
    
    private TestObject BuildInput()
    {
        return new TestObject
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
            },
            Colour = Colour.Green
        };
    }
    
    public class TestObject
    {
        public string StrValue { get; set; }
        
        public SubObject Sub { get; set; }
        
        public IElement[] Elements { get; set; }
        public IElement Btn { get; set; }
        
        public Colour Colour { get; set; }
    }
    
    public class SubObject
    {
        public string StrValue { get; set; }
    }
    
    public enum Colour
    {
        Red,
        Green,
        Blue
    }
}