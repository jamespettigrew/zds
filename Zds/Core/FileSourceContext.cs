using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Zds.Core
{
    public interface ISourceContext
    {
        List<string> ListSources();
        Stream StreamSource(string source);
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

        public Stream StreamSource(string source)
        {
            string path = FilePaths.First(p => Path.GetFileName(p) == source);
            return File.OpenRead(path);
        }
    }
}