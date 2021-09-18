using Spectre.Console;
using Zds.Core;

namespace Zds.Cli
{
    public class SelectSourceCommand
    {
        private readonly Repository _repository;
        
        public SelectSourceCommand(Repository repository)
        {
            _repository = repository;
        }
        
        public void Execute()
        {
            var prompt = new SelectionPrompt<string>()
                .Title($"Which [{Theme.PrimaryColour}]source[/] would you like to [{Theme.PrimaryColour}]search[/]?")
                .MoreChoicesText("[grey](Press UP and DOWN to reveal more.)[/]")
                .AddChoices(_repository.ListSources());
            string selectedSource = AnsiConsole.Prompt(prompt);
            AnsiConsole.MarkupLine($"Source selected: [{Theme.PrimaryColour}]{0}[/]", selectedSource);
            
            new SelectPathCommand(_repository).Execute(selectedSource);
        }
    }
}
