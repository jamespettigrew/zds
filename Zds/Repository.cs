using System.Collections.Generic;
using System.Linq;

namespace Zds
{
    public class Repository
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
        
        public List<Position> QuerySource(string source, string path, string? value)
        {
            if (_sourceIndexes.TryGetValue(source, out InvertedIndex? index))
            {
                if (value != null) return index.Query(new PathValue(path, value));

                // A null value implies we wish to find objects with no value at the specified path.
                return index
                    .ListPositions()
                    .Except(index.ListPositionsIndexedAtPath(path))
                    .ToList();
            }

            return new List<Position>();
        }
    }
}