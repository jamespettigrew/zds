using System;
using Spectre.Console;
using Zds.Core;
using Zds.Core.Index;
using Zds.Core.Queries;

namespace Zds.Cli
{
    public class BuildQueryCommand
    {
        private readonly ObjectRepository _objectRepository;

        public BuildQueryCommand(ObjectRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        public ObjectGraphQuery Execute()
        {
            AnsiConsole.Render(new Rule($"[{Theme.PrimaryColour}]Build Query[/]")
            {
                Alignment = Justify.Left
            });
            
            string source = PromptSource();
            string path = PromptPath(source);
            string? value = PromptSearchValue();
            
            return new ObjectGraphQuery(source, path, value);
        }
        
        private string PromptSource()
        {
            var prompt = new SelectionPrompt<string>()
                .Title($"Which [{Theme.PrimaryColour}]source[/] would you like to search?")
                .MoreChoicesText("[grey](Press UP and DOWN to reveal more.)[/]")
                .AddChoices(_objectRepository.ListSources());
            string selectedSource = AnsiConsole.Prompt(prompt);
            AnsiConsole.MarkupLine($"Source selected: [{Theme.PrimaryColour}]{selectedSource}[/]");

            return selectedSource;
        }
        
        private string PromptPath(string source)
        {
            var prompt = new SelectionPrompt<string>()
                .Title($"Which [{Theme.PrimaryColour}]path[/] would you like to search?")
                .MoreChoicesText("[grey](Press UP and DOWN to reveal more.)[/]")
                .AddChoices(_objectRepository.ListPathsForSource(source));
            string selectedPath = AnsiConsole.Prompt(prompt);
            AnsiConsole.MarkupLine($"Path selected: [{Theme.PrimaryColour}]{selectedPath}[/]");

            return selectedPath;
        }

        private string? PromptSearchValue()
        {
            var prompt = new TextPrompt<string?>($"Search [{Theme.PrimaryColour}]value[/]:")
                .AllowEmpty()
                .DefaultValue(null)
                .HideDefaultValue();
            return AnsiConsole.Prompt(prompt);
        }
    }
}