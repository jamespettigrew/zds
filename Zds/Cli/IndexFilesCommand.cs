using System.Collections.Generic;
using System.IO;
using Spectre.Console;
using Zds.Core;

namespace Zds.Cli
{
    public class IndexFilesCommand
    {
        private readonly Repository _repository;
        
        public IndexFilesCommand(Repository repository)
        {
            _repository = repository;
        }
        
        public void Execute()
        {
            AnsiConsole.Render(new Rule($"[{Theme.PrimaryColour}]Indexing[/]")
            {
                Alignment = Justify.Left
            });
            
            AnsiConsole.Status()
                .Start("Indexing files...", ctx =>
                {
                    IEnumerable<string> filenames = Directory.EnumerateFiles($"{Directory.GetCurrentDirectory()}/Samples");
                    foreach (string filename in filenames)
                    {
                        FileStream stream = File.Open(filename, FileMode.Open);
                        IEnumerable<ObjectRecord> records = JsonLoader.EnumerateObjects(stream);
                        foreach (var record in records)
                        {
                            _repository.AddObjectRecord(filename, record);
                        }
                        AnsiConsole.MarkupLine($"Indexed {filename}");
                    }
                });
            
            AnsiConsole.Render(new Rule($"[{Theme.PrimaryColour}]Build Query[/]")
            {
                Alignment = Justify.Left
            });
            new SelectSourceCommand(_repository).Execute();
        }
    }
}