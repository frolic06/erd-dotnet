using System.Linq;
namespace erd_dotnet;

public class Erd
{
    public Erd()
    {
        Entities = new List<Entity>();
        Relationships = new List<Relationship>();
    }
    public Entity AddOrUpdateEntity(string title)
    {
        var entity = Entities.FirstOrDefault(e => e.Title == title);
        if (entity == null)
        {
            entity = new Entity() { Title = title };
            Entities.Add(entity);
        }
        return entity;
    }

    public void AddMissingRelationships(Relationship relationship)
    {
        if (!Relationships.Any(r => (r.Name1 == relationship.Name1 && r.Name2 == relationship.Name2) 
                                    || (r.Name1 == relationship.Name2 && r.Name2 == relationship.Name1)))
        {
            Relationships.Add(relationship);
        }
    }

    public List<Entity> Entities { get; set; }
    public List<Relationship> Relationships { get; set; }
}
