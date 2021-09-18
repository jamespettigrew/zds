using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;
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

            Page page = new (matches.Count, 3);
            AnsiConsole.Render(GenerateRenderableForPage(selectedSource, matches, page));
            
            bool displayingResults = true;
            while (displayingResults)
            {
                switch (Console.ReadKey())
                {
                    case {Key: ConsoleKey.LeftArrow}:
                        var prevPage = page.Prev();
                        if (prevPage != null)
                        {
                            page = prevPage;
                            AnsiConsole.Render(GenerateRenderableForPage(selectedSource, matches, page));
                        }
                        break;
                    case {Key: ConsoleKey.RightArrow}:
                        var nextPage = page.Next();
                        if (nextPage != null)
                        {
                            page = nextPage;
                            AnsiConsole.Render(GenerateRenderableForPage(selectedSource, matches, page));
                        }
                        break;
                    default:
                        displayingResults = false;
                        break;
                }
            }

            new SelectSourceCommand(_repository).Execute();
        }

        private record Page
        {
            private int _total;
            private int _size;
            
            public Page(int total, int size)
            {
                _total = total;
                _size = size;
                Start = 0;
                End = Math.Min(_total, _size);
            }

            public int Start { get; init; }
            public int End { get; init; }
            
            public Page? Prev()
            {
                int prevStart = Math.Max(0, Start - _size);
                int prevEnd = Math.Min(_total, Start);
                if (prevEnd <= prevStart) return null;
                return this with {
                    Start = prevStart,
                    End = prevEnd
                };
            }

            public Page? Next()
            {
                int nextStart = End;
                int nextEnd = Math.Min(_total, End + _size);
                if (nextStart >= nextEnd) return null;
                return this with {
                    Start = nextStart,
                    End = nextEnd
                };
            }
        }
        
        private IRenderable GenerateRenderableForPage(string source, List<Position> matches, Page page)
        {
            var table = new Table { Border = TableBorder.Rounded };
            table.AddColumn("Page");

            for (int i = page.Start; i < page.End; i++)
            {
                var tree = new Tree(new Text($"{i + 1}", Style.Parse("darkorange")))
                {
                   Style = Style.Parse("darkorange")
                };
                
                FileStream stream = File.Open(source, FileMode.Open);
                JObject jobj = JsonLoader.GetObjectStartingAtPosition(stream, matches[i]);
                string escaped = Markup.Escape(jobj.ToString());
                var panel = new Panel(escaped)
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = Style.Parse("darkorange")
                };
                tree.AddNode(panel);
                table.AddRow(tree);
            }

            var footer = new Table
            {
                Border = TableBorder.None,
                BorderStyle = Style.Parse("darkorange")
            };
            footer.AddColumn("");
            footer.HideHeaders();
            footer.AddRow(new Text($"Displaying results {page.Start + 1}-{page.End} / {matches.Count}"));
            if (page.Prev() != null) footer.AddRow(new Text("Prev (←)"));
            if (page.Next() != null) footer.AddRow(new Text("Next (→)"));
            footer.AddRow(new Text("New Search (Any other key)"));
            table.AddRow(footer);
            
            return table;
        }
    }
}
