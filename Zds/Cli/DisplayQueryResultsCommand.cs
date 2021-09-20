using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;
using Zds.Core.Queries;

namespace Zds.Cli
{
    public class DisplayQueryResultsCommand
    {
        private readonly ObjectGraphQueryHandler _queryHandler;
        
        public DisplayQueryResultsCommand(ObjectGraphQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }
        
        public void Execute(ObjectGraphQuery query)
        {
            Page<ObjectGraph> page = _queryHandler.Execute(query, 3);
            
            Rule r = new($"[{Theme.PrimaryColour}]Query Results[/]") {Alignment = Justify.Left};
            AnsiConsole.Render(r);
            AnsiConsole.Render(GenerateRenderableForPage(page));
            
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
                            AnsiConsole.Render(GenerateRenderableForPage(page));
                        }
                        break;
                    case {Key: ConsoleKey.RightArrow}:
                        var nextPage = page.Next();
                        if (nextPage != null)
                        {
                            page = nextPage;
                            AnsiConsole.Render(GenerateRenderableForPage(page));
                        }
                        break;
                    default:
                        displayingResults = false;
                        break;
                }
            }
        }

        private IRenderable GenerateRenderableForPage(Page<ObjectGraph> page)
        {
            var table = new Table { Border = TableBorder.Rounded };
            table.AddColumn("Page");

            List<ObjectGraph> results = page.Results;
            for (int i = 0; i < results.Count; i++)
            {
                ObjectGraph match = results[i];
                Tree tree = new(new Text($"{page.Start + i + 1}", Style.Parse($"{Theme.PrimaryColour}")));
                string escaped = Markup.Escape(match.Obj.ToString());
                var panel = new Panel(escaped)
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = Style.Parse($"{Theme.PrimaryColour}")
                };
                tree.AddNode(panel);

                Tree relatedNode = new(new Text("Related"))
                {
                    Style = Style.Parse($"{Theme.SecondaryColour}")
                };
                foreach (JObject relatedObj in match.RelatedObjects)
                {
                    relatedNode.AddNode(
                        new Panel(Markup.Escape(relatedObj.ToString()))
                        {
                            Border = BoxBorder.Rounded,
                            BorderStyle = Style.Parse($"{Theme.SecondaryColour}")
                        }
                    );
                }
                tree.AddNode(relatedNode);
                table.AddRow(tree);
            }

            var footer = new Table
            {
                Border = TableBorder.None,
                BorderStyle = Style.Parse($"{Theme.PrimaryColour}")
            };
            footer.AddColumn("");
            footer.HideHeaders();
            string metadata = page.Total == 0
                ? "No results"
                : $"Displaying results {page.Start + 1}-{page.End} / {page.Total}";
            footer.AddRow(new Text(metadata));
            if (page.HasPrev) footer.AddRow(new Text("Prev (←)"));
            if (page.HasNext) footer.AddRow(new Text("Next (→)"));
            footer.AddRow(new Text("New Search (Any other key)"));
            table.AddRow(footer);
            
            return table;
        }
    }
}
