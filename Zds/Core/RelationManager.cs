using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Zds.Core.Queries;

namespace Zds.Core
{
    public record Relation(Key From, Key To);
    public record Key(string Source, string Path);
    
    public class RelationManager
    {
        private readonly Dictionary<string, Dictionary<string, List<Key>>> _relations = new()
        {
            ["/misc/repos/zds/Samples/tickets.json"] = new Dictionary<string, List<Key>>
            {
                ["assignee_id"] = new List<Key>()
                {
                    new Key("/misc/repos/zds/Samples/users.json", "_id")
                }
            }
        };
        
        public List<ObjectGraphQuery> ComputeRelatedQueries(string source, JObject obj)
        {
            List<ObjectGraphQuery> relatedQueries = new();
            var sourceRelations = _relations[source];
            foreach (string fromPath in sourceRelations.Keys)
            {
                var value = obj.Value<string?>(fromPath);
                if (value == null) continue;
                
                foreach (Key to in sourceRelations[fromPath])
                {
                    relatedQueries.Add(new ObjectGraphQuery(to.Source, to.Path, value));
                }
            }

            return relatedQueries;
        }
    }
}