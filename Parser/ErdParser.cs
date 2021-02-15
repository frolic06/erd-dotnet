using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace erd_dotnet
{
    class ErdParser
    {
        private Erd erd;
        private Entity entity;

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
                        entity.Fields.Add(new Attribute(l));
                    }

                }
                else { AddCurrentEntity(); }
            }
            AddCurrentEntity();

            return erd;
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
                throw new System.Exception($"Invalid table name, missing ']' in {line}");
            }
            return res;
        }

        private static Entity ParseEntity(string line)
        {
            return new Entity() { Title = line.Substring(1, line.Length - 2) };
        }

        private static Relationship ParseRelationship(string line)
        {
            var parts = line.Split("--");
            if (parts.Length != 2)
            {
                throw new System.Exception($"Invalid relationship {line}");
            }
            var part1 = SplitWhitespace(parts[0]);
            var part2 = SplitWhitespace(parts[1]);

            var relationship = new Relationship();
            relationship.Name1 = part1.first;
            relationship.Label1 = GetRelationshipLabel(part1.last);
            relationship.Name2 = part2.last;
            relationship.Label2 = GetRelationshipLabel(part2.first);

            return relationship;
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
            var substrings = Regex.Split(text, @"\s+");
            return (substrings[0], substrings[1]);
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
}