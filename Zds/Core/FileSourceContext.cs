using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Zds.Core
{
    public interface ISourceContext
    {
        List<string> ListSources();
        Stream? StreamSource(string source);
    }
    
    public class FileSourceContext : ISourceContext
    {
        private readonly string _path;
        public FileSourceContext(string path)
        {
            _path = path;
        }

        private List<string> FilePaths => Directory.GetFiles(_path, "*.json").ToList();
        
        public List<string> ListSources() => FilePaths
            .Select(path => Path.GetFileName(path))
            .ToList();

        public Stream? StreamSource(string source)
        {
            string? path = FilePaths.FirstOrDefault(p => Path.GetFileName(p) == source);
            if (path == null) return null;

            return File.OpenRead(path);
        }
    }
}