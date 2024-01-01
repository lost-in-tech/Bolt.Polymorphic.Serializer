using Bolt.Polymorphic.Serializer.Tests.Fixtures;

namespace Bolt.Polymorphic.Serializer.Tests;

public class JsonSerializerTests(SerializerFixture fixture) : IClassFixture<SerializerFixture>
{
    private readonly SerializerFixture _fixture = fixture;

    [Fact]
    public void Should_serialize()
    {
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

        var got = fixture.GetJsonSerializer().Serialize(d);

        var s = got;
    }
}