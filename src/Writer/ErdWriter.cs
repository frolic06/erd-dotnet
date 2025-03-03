namespace erd_dotnet;

public class ErdWriter
{
    private Erd erd;

    public ErdWriter(Erd erd)
    {
        this.erd = erd;
    }
    public List<string> GetStrings()
    {
        var erdFile = new List<string>
        {
            "header {size: \"20\", color: \"#3366ff\"}",
            "entity {bgcolor: \"#ececfc\", size: \"20\"}",
            ""
        };
        foreach (var entity in erd.Entities)
        {
            erdFile.Add($"[{entity.Title}]");
            foreach (var field in entity.Fields)
            {
                var prefix = "";
                if (field.IsPK)
                {
                    prefix += "*";
                }
                if (field.IsFK)
                {
                    prefix += "+";
                }
                erdFile.Add($"{prefix}{field.Name}");
            }
            erdFile.Add("");
        }
        if (erd.Relationships.Count > 0)
        {
            erdFile.AddRange([
                "# Each relationship must be between exactly two entities, which need not",
                "# be distinct. Each entity in the relationship has exactly one of four",
                "# possible cardinalities:",
                "#",
                "# Cardinality    Syntax",
                "# 0 or 1         ?",
                "# exactly 1      1",
                "# 0 or more      *",
                "# 1 or more      +",
            ""
            ]);
        }
        foreach (var relationship in erd.Relationships)
        {
            erdFile.Add($"{relationship.Name1} {relationship.Label1}--{relationship.Label2} {relationship.Name2}");
        }
        return erdFile;
    }
}