using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Zds.Core.Queries;

namespace Zds.Core.Relations
{
    public class RelationsException : Exception
    {
        public RelationsException(string message) : base(message) { } 
        public RelationsException(string message, Exception innerException) : base(message, innerException) { }
    }
    
    public record Relation(Key From,  Key To);
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
        
        public List<ObjectGraphQuery> ComputeRelatedQueries(string source, IEnumerable<PathValue> pathValues)
        {
            List<ObjectGraphQuery> relatedQueries = new();

            Dictionary<string, List<string>> pathValueLookup = new();
            foreach (PathValue pv in pathValues)
            {
                pathValueLookup.TryAdd(pv.Path, new List<string>());
                pathValueLookup[pv.Path].Add(pv.Value);
            }
            
            if (_sourceRelationLookup.TryGetValue(source, out Dictionary<string, List<Key>>? sourceRelations))
            {
                foreach (string fromPath in sourceRelations.Keys)
                {
                    if (!pathValueLookup.ContainsKey(fromPath)) continue;
                    
                    foreach (Key to in sourceRelations[fromPath])
                    {
                        var queries = pathValueLookup[fromPath]
                            .Select(value => new ObjectGraphQuery(to.Source, to.Path, value));
                        relatedQueries.AddRange(queries);
                    }
                }
            }

            return relatedQueries;
        }
    }
}