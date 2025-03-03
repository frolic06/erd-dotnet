namespace erd_dotnet;

public class Entity
{
    public string Title { get; set; }
    public List<Attribute> Fields { get; set; }

    public Entity()
    {
        Fields = new List<Attribute>();
        Title = string.Empty;
    }
    public Entity(string title, List<Attribute> fields)
    {
        Title = title;
        Fields = fields;
    }
    public void AddOrUpdateAttribute(Attribute attribute)
    {
        var field = Fields.FirstOrDefault(f => f.Name == attribute.Name);
        if (field == null)
        {
            Fields.Add(attribute);
        }
        else
        {
            field.Label = attribute.Label;
            field.IsPK = attribute.IsPK;
            field.IsFK = attribute.IsFK;
        }
    }
}

public class Attribute
{
    public string Name { get; set; }
    public string Label { get; set; }
    public bool IsPK { get; set; }
    public bool IsFK { get; set; }

    public Attribute(string name, string label = "")
    {
        Name = name;
        Label = label;

        if (name.StartsWith("*+") || name.StartsWith("+*"))
        {
            IsPK = true;
            IsFK = true;
            Name = name.Substring(2);
        }
        else
        {
            if (name.StartsWith("*"))
            {
                IsPK = true;
                Name = name.Substring(1);
            }
            if (name.StartsWith("+"))
            {
                IsFK = true;
                Name = name.Substring(1);
            }
        }
    }

    public Attribute(string name, bool isPK, bool isFK)
    {
        Name = name;
        Label = "";
        IsPK = isPK;
        IsFK = isFK;
    }
}