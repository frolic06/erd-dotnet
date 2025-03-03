using erd_dotnet;
using Xunit;
using FluentAssertions;

namespace erd_dotnet_Test;

public class ErdParserTest
{
    string erdText = """
        # Comment
        [Person]
        *name
        height
        weight
        birth
        +location_id

        [Location]
        *id
        city
        state
        country

        Person *--1 Location
        """;

    [Fact]
    public void TestErdParser()
    {
        var erd = new ErdParser();
        var result = erd.ParseFromText(erdText.Split("\n"));
        result.Entities.Count.Should().Be(2);
        result.Entities[0].Title.Should().Be("Person");
        result.Entities[0].Fields.Count.Should().Be(5);
        result.Entities[0].Fields[0].Should().BeEquivalentTo(new erd_dotnet.Attribute("*name"));

        result.Entities[1].Title.Should().Be("Location");
        result.Entities[1].Fields.Count.Should().Be(4);
        result.Entities[1].Fields[0].Should().BeEquivalentTo(new erd_dotnet.Attribute("*id"));

        result.Relationships.Count.Should().Be(1);
        result.Relationships[0].Should().BeEquivalentTo(new Relationship() { Name1 = "Person", Label1 = "*", Name2 = "Location", Label2 = "1" });
    }
}
