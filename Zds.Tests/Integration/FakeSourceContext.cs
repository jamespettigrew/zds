using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Zds.Core;

namespace Zds.Tests.Integration
{
    public record FakeSource(string Source, string Content);
    
    public class FakeSourceContext : ISourceContext
    {
        private readonly List<FakeSource> _sources;
        
        public FakeSourceContext(List<FakeSource> sources)
        {
            _sources = sources;
        }
        
        public List<string> ListSources()
        {
            return _sources.Select(s => s.Source).ToList();
        }

        public Stream? StreamSource(string sourceName)
        {
            FakeSource? source = _sources.FirstOrDefault(s => s.Source == sourceName);
            if (source == null) return null;
            byte[] streamData = Encoding.UTF8.GetBytes(source.Content);
            return new MemoryStream(streamData);
        }
    }
}