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
                .Title("Which [darkorange]path[/] would you like to [darkorange]search[/]?")
                .MoreChoicesText("[grey](Press UP and DOWN to reveal more.)[/]")
                .AddChoices(_repository.ListPathsForSource(selectedSource));
            string selectedPath = AnsiConsole.Prompt(prompt);
            AnsiConsole.MarkupLine("Path selected: [darkorange]{0}[/]", selectedPath);
            
            new PromptSearchValueCommand(_repository).Execute(selectedSource, selectedPath);
        }
    }
}
