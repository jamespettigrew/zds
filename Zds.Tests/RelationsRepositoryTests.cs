using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zds.Core;
using Zds.Core.Queries;
using Zds.Core.Relations;

namespace Zds.Tests
{
    [TestClass]
    public class RelationsRepositoryTests
    {
        [TestMethod]
        public void RelatedQueriesCorrect()
        {
            RelationsRepository repository = new();
            CollectionAssert.AreEquivalent(
                new List<ObjectGraphQuery>(),
                repository.ComputeRelatedQueries("test", new PathValue[] { })
            );

            List<Relation> relations = new List<Relation>
            {
                new (new Key("test", "a"), new Key("test2", "b"))
            };
            repository.AddRelations(relations);
            var pathValues = new PathValue[]
            {
                new ("a", "1"),
                new ("a", "2"),
                new ("b", "3")
            };
            
            CollectionAssert.AreEquivalent(
                new List<ObjectGraphQuery>(),
                repository.ComputeRelatedQueries("other", pathValues)
            );
            CollectionAssert.AreEquivalent(
                new List<ObjectGraphQuery>
                {
                    new ("test2", "b", "1"),
                    new ("test2", "b", "2")
                },
                repository.ComputeRelatedQueries("test", pathValues)
            );
        }
    }
}