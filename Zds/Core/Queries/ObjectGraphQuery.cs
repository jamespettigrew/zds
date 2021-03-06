using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Zds.Core.Index;
using Zds.Core.Relations;

namespace Zds.Core.Queries
{
    public record ObjectGraphQuery(string Source, string Path, string? Value);
    public record ObjectGraph(JObject Obj, List<JObject> RelatedObjects);
    
    public class ObjectGraphQueryHandler
    {
        private readonly ObjectRepository _objectRepository;
        private readonly RelationsRepository _relationsRepository;
        private readonly ISourceContext _sourceContext;
        
        public ObjectGraphQueryHandler(
            ObjectRepository objectRepository,
            RelationsRepository relationsRepository,
            ISourceContext sourceContext)
        {
            _objectRepository = objectRepository;
            _relationsRepository = relationsRepository;
            _sourceContext = sourceContext;
        }
        
        public Page<ObjectGraph> Execute(ObjectGraphQuery query, int pageSize)
        {
            List<Match> matches = _objectRepository.QuerySource(query.Source, query.Path, query.Value);
            List<ObjectGraph> GeneratePageResults(int startOffset, int count)
            {
                List<ObjectGraph> queryResults = new();
                foreach (Match match in matches.GetRange(startOffset, count))
                {
                    JObject obj = GetObjectFromFile(match.Source, match.Position);
                    List<PathValue> pathValues = JsonUtils.Flatten(obj);
                    List<ObjectGraphQuery> relatedQueries = _relationsRepository.ComputeRelatedQueries(query.Source, pathValues);
                    List<JObject> relatedObjects = relatedQueries
                        .Select(q => _objectRepository.QuerySource(q.Source, q.Path, q.Value))
                        .SelectMany(relatedMatches => relatedMatches)
                        .Select(relatedMatch => GetObjectFromFile(relatedMatch.Source, relatedMatch.Position))
                        .ToList();
                    queryResults.Add(new(obj, relatedObjects));
                }

                return queryResults;
            }

            return new Page<ObjectGraph>(matches.Count, pageSize, GeneratePageResults);
        }
        
        private JObject GetObjectFromFile(string source, Position position)
        {
            using Stream stream = _sourceContext.StreamSource(source);
            return JsonUtils.GetObjectStartingAtPosition(stream, position);
        }
    }
}
