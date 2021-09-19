using System;
using Spectre.Console;
using Zds.Core;
using Zds.Core.Queries;

namespace Zds.Cli
{
    public class BuildQueryCommand
    {
        private readonly Repository _repository;

        public BuildQueryCommand(Repository repository)
        {
            _repository = repository;
        }

        public ObjectGraphQuery Execute()
        {
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
                .AddChoices(_repository.ListSources());
            string selectedSource = AnsiConsole.Prompt(prompt);
            AnsiConsole.MarkupLine($"Source selected: [{Theme.PrimaryColour}]{selectedSource}[/]");

            return selectedSource;
        }
        
        private string PromptPath(string source)
        {
            var prompt = new SelectionPrompt<string>()
                .Title($"Which [{Theme.PrimaryColour}]path[/] would you like to search?")
                .MoreChoicesText("[grey](Press UP and DOWN to reveal more.)[/]")
                .AddChoices(_repository.ListPathsForSource(source));
            string selectedPath = AnsiConsole.Prompt(prompt);
            AnsiConsole.MarkupLine($"Path selected: [{Theme.PrimaryColour}]{selectedPath}[/]");

            return selectedPath;
        }

        private string? PromptSearchValue()
        {
            var prompt = new TextPrompt<string?>($"Search [{Theme.PrimaryColour}]value[/]:")
                .AllowEmpty();
            return AnsiConsole.Prompt(prompt);
        }
    }
}