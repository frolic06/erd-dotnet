using System.Collections.Generic;

namespace erd_dotnet
{
    class RelationshipWriter
    {
        public static List<string> BuildString(Relationship relationship)
        {
            return new List<string> 
            { 
                $"{relationship.Name1} -- {relationship.Name2}" +
                $"[headlabel=<<FONT>{relationship.Label2}</FONT>>,taillabel=<<FONT>{relationship.Label1}</FONT>>];"
            };
        }
    }
}