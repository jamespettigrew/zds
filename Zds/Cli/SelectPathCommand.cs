using Spectre.Console;
using Zds.Core;

namespace Zds.Cli
{
    public class SelectPathCommand
    {
        private readonly Repository _repository;
        
        public SelectPathCommand(Repository repository)
        {
            _repository = repository;
        }
        
        public void Execute(string selectedSource)
        {
            var prompt = new SelectionPrompt<string>()
                .Title($"Which [{Theme.PrimaryColour}]path[/] would you like to [{Theme.PrimaryColour}]search[/]?")
                .MoreChoicesText("[grey](Press UP and DOWN to reveal more.)[/]")
                .AddChoices(_repository.ListPathsForSource(selectedSource));
            string selectedPath = AnsiConsole.Prompt(prompt);
            AnsiConsole.MarkupLine($"Path selected: [{Theme.PrimaryColour}]{0}[/]", selectedPath);
            
            new PromptSearchValueCommand(_repository).Execute(selectedSource, selectedPath);
        }
    }
}
