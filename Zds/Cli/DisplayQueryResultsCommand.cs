using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Zds.Core;

namespace Zds.Cli
{
    public class DisplayQueryResultsCommand
    {
        private readonly Repository _repository;
        
        public DisplayQueryResultsCommand(Repository repository)
        {
            _repository = repository;
        }
        
        public void Execute(string selectedSource, string selectedPath, string? value)
        {
            List<Position> matches = _repository.QuerySource(selectedSource, selectedPath, value);
            Rule r = new($"[darkorange]Matches:[/] {matches.Count}");
            r.Alignment = Justify.Left;
            AnsiConsole.Render(r);

            for (int i = 0; i < matches.Count; i++)
            {
                FileStream stream = File.Open(selectedSource, FileMode.Open);
                JObject jobj = JsonLoader.GetObjectStartingAtPosition(stream, matches[i]);
                string escaped = Markup.Escape(jobj.ToString());
                var panel = new Panel(escaped);
                panel.Header = new PanelHeader($"[darkorange]Match[/] {i + 1}/{matches.Count}");
                AnsiConsole.Render(panel);
            }

            new SelectSourceCommand(_repository).Execute();
        }
    }
}
