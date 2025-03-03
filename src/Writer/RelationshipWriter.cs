namespace erd_dotnet;

class RelationshipWriter
{
    private static string GetLabel(string text)
    {
        return text switch
        {
            "?" => "{0,1}",
            "1" => "1",
            "*" => "0..N",
            "+" => "1..N",
            _ => ""
        };
    }

    private static string AddQuotes(string text)
    {
        return "\"" + text + "\"";
    }

    public static List<string> BuildString(Relationship relationship)
    {
        return new List<string>
            {
                $"{AddQuotes(relationship.Name1)} -- {AddQuotes(relationship.Name2)}" +
                $"[headlabel=<<FONT>{GetLabel(relationship.Label2)}</FONT>>,taillabel=<<FONT>{GetLabel(relationship.Label1)}</FONT>>];"
            };
    }
}
