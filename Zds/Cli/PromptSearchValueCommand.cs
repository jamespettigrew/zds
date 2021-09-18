using Spectre.Console;
using Zds.Core;

namespace Zds.Cli
{
    public class PromptSearchValueCommand
    {
        private readonly Repository _repository;
        
        public PromptSearchValueCommand(Repository repository)
        {
            _repository = repository;
        }
        
        public void Execute(string selectedSource, string selectedPath)
        {
            var prompt =
                new TextPrompt<string?>(
                        "[grey][[Leave empty to search for null/missing values]][/] [darkorange]Search term:[/]")
                    .AllowEmpty();
            string? searchTerm = AnsiConsole.Prompt(prompt);

            new DisplayQueryResultsCommand(_repository).Execute(selectedSource, selectedPath, searchTerm);
        }
    }
}
