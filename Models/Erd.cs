using System.Collections.Generic;

namespace erd_dotnet
{
    class Erd
    {
        public Erd()
        {
            Entities = new List<Entity>();
            Relationships = new List<Relationship>();
        }
        public List<Entity> Entities { get; set; }
        public List<Relationship> Relationships { get; set; }
    }
}