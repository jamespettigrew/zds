using System.Collections.Generic;
using System.Linq;

namespace Zds.Core.Index
{
    public record Match(string Source, string Path, Position Position);
    
    public class ObjectRepository
    {
        private readonly Dictionary<string, InvertedIndex> _sourceIndexes = new();
        public List<string> ListSources() => _sourceIndexes.Keys.ToList();

        public void AddObjectRecord(string source, ObjectRecord record)
        {
            _sourceIndexes.TryAdd(source, new InvertedIndex());
            foreach (PathValue pv in record.PathValues)
            {
                _sourceIndexes[source].Index(pv, record.Position);
            }
        }

        public List<string> ListPathsForSource(string source)
        {
            if (_sourceIndexes.TryGetValue(source, out InvertedIndex? index))
            {
                return index.ListPaths();
            }

            return new List<string>();
        }
        
        public List<Match> QuerySource(string source, string path, string? value)
        {
            if (_sourceIndexes.TryGetValue(source, out InvertedIndex? index))
            {
                // A null value implies we wish to find objects with no value at the specified path.
                IEnumerable<Position> matchingPositions = value != null ?
                    index.Query(new PathValue(path, value)) :
                    index.ListPositions().Except(index.ListPositionsIndexedAtPath(path));
                
                return matchingPositions
                    .Select(position => new Match(source, path, position))
                    .ToList();
            }

            return new List<Match>();
        }
    }
}