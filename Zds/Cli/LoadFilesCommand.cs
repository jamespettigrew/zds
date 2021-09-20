using System.Collections.Generic;
using System.IO;
using Spectre.Console;
using Zds.Core;
using Zds.Core.Index;
using Zds.Core.Relations;

namespace Zds.Cli
{
    public class LoadFilesCommand
    {
        private readonly Options _options;
        private readonly ObjectRepository _objectRepository;
        private readonly RelationsRepository _relationsRepository;
        private readonly ISourceContext _sourceContext;
        
        public LoadFilesCommand(
            Options options,
            ObjectRepository objectRepository,
            RelationsRepository relationsRepository,
            ISourceContext sourceContext)
        {
            _options = options;
            _objectRepository = objectRepository;
            _relationsRepository = relationsRepository;
            _sourceContext = sourceContext;
        }
        
        public void Execute()
        {
            Rule rule = new ($"[{Theme.PrimaryColour}]Loading[/]") { Alignment = Justify.Left };
            AnsiConsole.Render(rule);
            AnsiConsole.Status().Start("Indexing files...", _ => LoadDataFiles());
            LoadRelations();
        }

        private void LoadDataFiles()
        {
            List<string> sources = _sourceContext.ListSources();
            foreach (string source in sources)
            {
                try
                {
                    using Stream? stream = _sourceContext.StreamSource(source);
                    if (stream == null) continue;
                    
                    IEnumerable<ObjectRecord> records = JsonLoader.EnumerateObjects(stream);
                    foreach (var record in records)
                    {
                        _objectRepository.AddObjectRecord(source, record);
                    }

                    AnsiConsole.MarkupLine($"Indexed {source}");
                }
                catch (JsonLoaderException e)
                {
                    AnsiConsole.MarkupLine($"[yellow]WARNING[/] error loading {source}.");
                    AnsiConsole.WriteException(e);
                }
            }
        }

        private void LoadRelations()
        {
            try
            {
                if (_options.RelationsPath != null)
                {
                    List<Relation> relations = RelationsLoader.Load(_options.RelationsPath);
                    _relationsRepository.AddRelations(relations);
                    AnsiConsole.MarkupLine($"Loaded relations: {_options.RelationsPath}");
                }
                else
                {
                    AnsiConsole.MarkupLine("No relations provided.");
                }
            }
            catch (RelationsException e)
            {
                AnsiConsole.MarkupLine("[yellow]WARNING[/] error loading relations.");
                AnsiConsole.WriteException(e);
            }
        }
    }
}