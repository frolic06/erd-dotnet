using erd_dotnet;
using Xunit;
using FluentAssertions;

namespace erd_dotnet_Test;

public class ErdWriterTest
{
    [Fact]
    public void TestErdWriter()
    {
        var erd = new Erd();
        var person = new Entity() { Title = "Person", Fields = new() { new("*name"), new("height"), new("weight"), new("birth"), new("+location_id") } };
        erd.Entities.Add(person);
        var location = new Entity() { Title = "Location", Fields = new() { new("*id"), new("city"), new("state"), new("country") } };
        erd.Entities.Add(location);
        erd.Relationships.Add(new Relationship() { Name1 = "Person", Label1 = "*", Name2 = "Location", Label2 = "1" });

        var writer = new ErdWriter(erd);
        var result = writer.GetStrings();
        result.Should().BeEquivalentTo(new List<string>()
        {
            "header {size: \"20\", color: \"#3366ff\"}",
            "entity {bgcolor: \"#ececfc\", size: \"20\"}",
            "",
            "[Person]",
            "*name",
            "height",
            "weight",
            "birth",
            "+location_id",
            "",
            "[Location]",
            "*id",
            "city",
            "state",
            "country",
            "",
            "# Each relationship must be between exactly two entities, which need not",
            "# be distinct. Each entity in the relationship has exactly one of four",
            "# possible cardinalities:",
            "#",
            "# Cardinality    Syntax",
            "# 0 or 1         ?",
            "# exactly 1      1",
            "# 0 or more      *",
            "# 1 or more      +",
            "",
            "Person *--1 Location"
        });
    }

    [Fact]
    public void TestErdWriterWithNoData()
    {
        var erd = new Erd();
        var writer = new ErdWriter(erd);
        var result = writer.GetStrings();
        result.Should().BeEquivalentTo(new List<string>()
        {
            "header {size: \"20\", color: \"#3366ff\"}",
            "entity {bgcolor: \"#ececfc\", size: \"20\"}",
            ""
        });
    }

    [Fact]
    public void TestErdUpdated()
    {
        var erd = new Erd();
        var person = new Entity() { Title = "Person", Fields = new() { new("*name"), new("+location_id") } };
        erd.Entities.Add(person);
        erd.Relationships.Add(new Relationship() { Name1 = "Person", Label1 = "*", Name2 = "Location", Label2 = "1" });
        
        // update an existing entity
        var person2 = erd.AddOrUpdateEntity("Person");
        person2.AddOrUpdateAttribute(new erd_dotnet.Attribute("height") { IsPK = true });

        // add a new entity
        var location = erd.AddOrUpdateEntity("Location");
        location.AddOrUpdateAttribute(new erd_dotnet.Attribute("city") { IsPK = true, IsFK = true });

        // try to add a relationship that already exists
        erd.AddMissingRelationships(new Relationship() { Name1 = "Location", Label1 = "*", Name2 = "Person", Label2 = "*" });

        // add a new relationship
        erd.AddMissingRelationships(new Relationship() { Name1 = "Location", Label1 = "*", Name2 = "Address", Label2 = "1" });

        var writer = new ErdWriter(erd);
        var result = writer.GetStrings();
        result.Should().BeEquivalentTo(new List<string>()
        {
            "header {size: \"20\", color: \"#3366ff\"}",
            "entity {bgcolor: \"#ececfc\", size: \"20\"}",
            "",
            "[Person]",
            "*name",
            "+location_id",
            "*height",
            "",
            "[Location]",
            "*+city",
            "",
            "# Each relationship must be between exactly two entities, which need not",
            "# be distinct. Each entity in the relationship has exactly one of four",
            "# possible cardinalities:",
            "#",
            "# Cardinality    Syntax",
            "# 0 or 1         ?",
            "# exactly 1      1",
            "# 0 or more      *",
            "# 1 or more      +",
            "",
            "Person *--1 Location",
            "Location *--1 Address"
        });
    }
}