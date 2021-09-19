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
        
        public ObjectGraphQueryHandler(ObjectRepository objectRepository, RelationsRepository relationsRepository)
        {
            _objectRepository = objectRepository;
            _relationsRepository = relationsRepository;
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
                    List<ObjectGraphQuery> relatedQueries = _relationsRepository.ComputeRelatedQueries(query.Source, obj);
                    List<JObject> relatedObjects = relatedQueries
                        .Select(q => _objectRepository.QuerySource(q.Source, q.Path, q.Value))
                        .SelectMany(match => match)
                        .Select(match => GetObjectFromFile(match.Source, match.Position))
                        .ToList();
                    queryResults.Add(new(obj, relatedObjects));
                }

                return queryResults;
            }

            return new Page<ObjectGraph>(matches.Count, pageSize, GeneratePageResults);
        }
        
        private JObject GetObjectFromFile(string source, Position position)
        {
            FileStream stream = File.Open(source, FileMode.Open);
            return JsonLoader.GetObjectStartingAtPosition(stream, position);
        }
    }

    public record Page<T>
    {
        private int _size;
        private readonly Func<int, int, List<T>> _generatePageResults;
        
        public Page(int total, int size, Func<int, int, List<T>> generatePageResults)
        {
            Total = total;
            _size = size;
            _generatePageResults = generatePageResults;
            Start = 0;
            End = Math.Min(Total, _size);
            Results = _generatePageResults(Start, End - Start);
        }

        public int Start { get; init; }
        public int End { get; init; }
        public int Total { get; }
        public List<T> Results { get; init; }
        
        public Page<T>? Prev()
        {
            int prevStart = Math.Max(0, Start - _size);
            int prevEnd = Math.Min(Total, Start);
            if (prevEnd <= prevStart) return null;
            return this with {
                Start = prevStart,
                End = prevEnd,
                Results = _generatePageResults(prevStart, prevEnd - prevStart)
            };
        }

        public Page<T>? Next()
        {
            int nextStart = End;
            int nextEnd = Math.Min(Total, End + _size);
            if (nextStart >= nextEnd) return null;
            return this with {
                Start = nextStart,
                End = nextEnd,
                Results = _generatePageResults(nextStart, nextEnd - nextStart)
            };
        }
    }
}
