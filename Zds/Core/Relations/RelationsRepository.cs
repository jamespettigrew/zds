using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Zds.Core.Queries;

namespace Zds.Core.Relations
{
    public class RelationsException : Exception
    {
        public RelationsException(string message) : base(message) { } 
        public RelationsException(string message, Exception innerException) : base(message, innerException) { }
    }
    
    public record Relation(Key From, Key To);
    public record Key(string Source, string Path);
    
    public class RelationsRepository
    {
        private Dictionary<string, Dictionary<string, List<Key>>> _sourceRelationLookup = new();

        public void AddRelations(IEnumerable<Relation> relations)
        {
            foreach (Relation relation in relations)
            {
                // Forward relationship
                _sourceRelationLookup.TryAdd(relation.From.Source, new Dictionary<string, List<Key>>());
                _sourceRelationLookup[relation.From.Source].TryAdd(relation.From.Path, new List<Key>());
                var tos = _sourceRelationLookup[relation.From.Source][relation.From.Path];
                if (!tos.Contains(relation.To)) tos.Add(relation.To);
                
                // Inverse relationship
                _sourceRelationLookup.TryAdd(relation.To.Source, new Dictionary<string, List<Key>>());
                _sourceRelationLookup[relation.To.Source].TryAdd(relation.To.Path, new List<Key>());
                var froms = _sourceRelationLookup[relation.To.Source][relation.To.Path];
                if (!froms.Contains(relation.From)) froms.Add(relation.From);
            }
        }
        
        public List<ObjectGraphQuery> ComputeRelatedQueries(string source, JObject obj)
        {
            List<ObjectGraphQuery> relatedQueries = new();
            if (_sourceRelationLookup.TryGetValue(source, out Dictionary<string, List<Key>>? sourceRelations))
            {
                foreach (string fromPath in sourceRelations.Keys)
                {
                    var value = obj.Value<string?>(fromPath);
                    if (value == null) continue;
                    
                    foreach (Key to in sourceRelations[fromPath])
                    {
                        relatedQueries.Add(new ObjectGraphQuery(to.Source, to.Path, value));
                    }
                }
            }

            return relatedQueries;
        }
    }
}