using System;
using Zds.Cli;
using Zds.Core;
using Zds.Core.Queries;

namespace Zds
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository repository = new ();
            new IndexFilesCommand(repository).Execute();
            while (true)
            {
                ObjectGraphQuery query = new BuildQueryCommand(repository).Execute();
                new DisplayQueryResultsCommand(new ObjectGraphQueryHandler(repository, new RelationManager())).Execute(query);
            }
        }
    }
}