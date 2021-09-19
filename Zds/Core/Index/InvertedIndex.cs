using System;
using System.Collections.Generic;
using System.Linq;

namespace Zds.Core.Index
{
    public class InvertedIndex
    {
        private readonly List<Position> _positions = new();
        private readonly Dictionary<string, Dictionary<string, List<Position>>> _pathValueIndex = new ();

        public List<string> ListPaths() => _pathValueIndex.Keys.ToList();
        public List<Position> ListPositions() => _positions.ToList();

        public List<Position> ListPositionsIndexedAtPath(string path) => _pathValueIndex
            .GetValueOrDefault(path, new Dictionary<string, List<Position>>())
            .Values
            .SelectMany(x => x)
            .ToList();
        
        public void Index(PathValue pathValue, Position position)
        {
            _positions.Add(position);
            _pathValueIndex.TryAdd(pathValue.Path, new Dictionary<string, List<Position>>());
            _pathValueIndex[pathValue.Path].TryAdd(pathValue.Value, new List<Position>());
            _pathValueIndex[pathValue.Path][pathValue.Value].Add(position);
        }

        public List<Position> Query(PathValue pathValue) => _pathValueIndex
            .GetValueOrDefault(pathValue.Path, new())
            .GetValueOrDefault(pathValue.Value, new ());
    }
}