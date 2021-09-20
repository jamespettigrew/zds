using System;
using System.Collections.Generic;

namespace Zds.Core.Queries
{
    public record Page<T>
    {
        private readonly int _size;
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

        public int Start { get; private init; }
        public int End { get; private init; }
        public int Total { get; }
        public List<T> Results { get; private init; }


        private int NextStart => End;
        private int NextEnd => Math.Min(Total, End + _size);
        public bool HasNext => NextStart < NextEnd;

        private int PrevStart => Math.Max(0, Start - _size);
        private int PrevEnd => Math.Min(Total, Start);
        public bool HasPrev => PrevEnd > PrevStart;
        
        public Page<T>? Prev()
        {
            if (!HasPrev) return null;
            return this with {
                Start = PrevStart,
                End = PrevEnd,
                Results = _generatePageResults(PrevStart, PrevEnd - PrevStart)
            };
        }

        public Page<T>? Next()
        {
            if (!HasNext) return null;
            return this with {
                Start = NextStart,
                End = NextEnd,
                Results = _generatePageResults(NextStart, NextEnd - NextStart)
            };
        }
    }
}