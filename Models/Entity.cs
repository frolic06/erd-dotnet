using System.Collections.Generic;

namespace erd_dotnet
{
    class Entity
    {
        public string Title { get; set; }
        public List<Attribute> Fields { get; set; }

        public Entity()
        {
            Fields = new List<Attribute>();
        }
        public Entity(string title, List<Attribute> fields)
        {
            Title = title;
            Fields = fields;
        }
    }

    class Attribute
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
    }
}