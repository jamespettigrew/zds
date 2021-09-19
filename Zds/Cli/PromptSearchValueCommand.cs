using Spectre.Console;
using Zds.Core;
using Zds.Core.Queries;

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
                new TextPrompt<string?>($"[grey][[Leave empty to search for null/missing values]][/] [{Theme.PrimaryColour}]Search term:[/]")
                    .AllowEmpty();
            string? searchTerm = AnsiConsole.Prompt(prompt);

            new DisplayQueryResultsCommand(new ObjectGraphQueryHandler(_repository, new RelationManager())).Execute(selectedSource, selectedPath, searchTerm);
        }
    }
}
