using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zds.Core;

namespace Zds.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void ListSourceCorrect()
        {
            Repository repository = new();
            repository.AddObjectRecord(
                "test1",
                new ObjectRecord(new Position(0, 0), new List<PathValue>()));
            repository.AddObjectRecord(
                "test2",
                new ObjectRecord(new Position(0, 0), new List<PathValue>()));

            CollectionAssert.AreEquivalent(new [] { "test1", "test2" }, repository.ListSources());
        }
        
        [TestMethod]
        public void ListPathsForSourceCorrect()
        {
            Repository repository = new();
            repository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(1, 3),
                    new List<PathValue>
                    {
                        new("x", "0"),
                        new("y", "1"),
                    }
                )
            );
            repository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(4, 6),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("y", "1"),
                    }
                )
            );
            repository.AddObjectRecord(
                "test2",
                new ObjectRecord(
                    new Position(7, 3),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("z", "1"),
                    }
                )
            );

            CollectionAssert.AreEquivalent(new [] { "x", "y" }, repository.ListPathsForSource("test1"));
        }
        
        [TestMethod]
        public void QueryForPresentValueCorrect()
        {
            Repository repository = new();
            repository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(1, 3),
                    new List<PathValue>
                    {
                        new("x", "0"),
                        new("y", "1"),
                    }
                )
            );
            repository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(4, 6),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("y", "1"),
                    }
                )
            );
            repository.AddObjectRecord(
                "test2",
                new ObjectRecord(
                    new Position(7, 3),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("z", "1"),
                    }
                )
            );

            CollectionAssert.AreEquivalent(
                new Position[] { new (1, 3), new (4, 6) },
                repository.QuerySource("test1", "y", "1")
            );
        }
        
        
        [TestMethod]
        public void QueryForMissingValueCorrect()
        {
            Repository repository = new();
            repository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(1, 3),
                    new List<PathValue>
                    {
                        new("x", "0"),
                        new("y", "1"),
                    }
                )
            );
            repository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(4, 6),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("y", "1"),
                    }
                )
            );
            repository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(7, 3),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("z", "1"),
                    }
                )
            );

            CollectionAssert.AreEquivalent(
                new Position[] { new (7, 3) },
                repository.QuerySource("test1", "y", null)
            );
        }
    }
}