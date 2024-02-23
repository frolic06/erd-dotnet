using Microsoft.VisualBasic.FileIO;

namespace erd_dotnet;

class ErdParser
{
    private Erd erd;
    private Entity? entity;

    public ErdParser()
    {
        erd = new Erd();
        entity = null;
    }

    public Erd Parse(IEnumerable<string> text)
    {

        entity = null;

        foreach (var line in text)
        {
            var l = line.Trim();
            if (!IsComment(l))
            {
                if (IsEntity(l))
                {
                    AddCurrentEntity();
                    entity = ParseEntity(l);
                }
                else if (IsRelationship(line))
                {
                    erd.Relationships.Add(ParseRelationship(line));
                }
                else if (entity != null)
                {
                    entity.Fields.Add(new Attribute(RemoveQuotes(l)));
                }

            }
            else { AddCurrentEntity(); }
        }
        AddCurrentEntity();

        return erd;
    }

    private static string RemoveQuotes(string text)
    {
        return text.Replace("'", "\"").Replace("`", "\"");
    }

    private static bool IsComment(string line)
    {
        return line.StartsWith('#') || string.IsNullOrEmpty(line);
    }

    private static bool IsRelationship(string line)
    {
        return line.Contains("--");
    }

    private static bool IsEntity(string line)
    {
        var res = line.StartsWith('[');
        if (res && !line.Contains(']'))
        {
            throw new Exception($"Invalid table name, missing ']' in {line}");
        }
        return res;
    }

    private static Entity ParseEntity(string line)
    {
        return new Entity() { Title = RemoveQuotes(line.Substring(1, line.Length - 2)) };
    }

    private static Relationship ParseRelationship(string line)
    {
        var parts = line.Split("--");
        if (parts.Length != 2)
        {
            throw new Exception($"Invalid relationship {line}");
        }
        var part1 = SplitWhitespace(parts[0]);
        var part2 = SplitWhitespace(parts[1]);

        return new Relationship()
        {
            Name1 = "\"" + part1.first + "\"",
            Label1 = GetRelationshipLabel(part1.last),
            Name2 = "\"" + part2.last + "\"",
            Label2 = GetRelationshipLabel(part2.first),
        };
    }

    private static string GetRelationshipLabel(string text)
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
    private static (string first, string last) SplitWhitespace(string text)
    {
        string first = string.Empty;
        string last = string.Empty;
        using (var parser = new TextFieldParser(new StringReader(RemoveQuotes(text))))
        {
            parser.HasFieldsEnclosedInQuotes = true;
            parser.Delimiters = new[] { " " };
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields is not null)
                {
                    foreach (var field in fields)
                    {
                        if (string.IsNullOrEmpty(first))
                        {
                            first = field;
                        }
                        else
                        {
                            last = field;
                        }
                    }
                }
            }
        }
        return (first, last);
    }

    private void AddCurrentEntity()
    {
        if (entity != null)
        {
            erd.Entities.Add(entity);
            entity = null;
        }
    }
}
