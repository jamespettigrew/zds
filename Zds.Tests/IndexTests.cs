using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zds.Tests
{
    [TestClass]
    public class IndexTests
    {
        private List<(PathValue PathValue, Position Position)> _testData = new ()
        {
            (new("x", "0"), new(1, 1)),
            (new("y", "1"), new(3, 1)),
            (new("x", "0"), new(10, 3))
        };
        
        [TestMethod]
        public void ListPathsCorrect()
        {
            InvertedIndex index = new ();
            Assert.AreEqual(0, index.ListPaths().Count);

            foreach ((PathValue pv, Position p) in _testData)
            {
                index.Index(pv, p);
            }
            
            Assert.AreEqual(
                _testData.Select(x => x.PathValue.Path).Distinct().Count(),
                index.ListPaths().Count
            );
        }
        
        [TestMethod]
        public void ListPositionsCorrect()
        {
            InvertedIndex index = new ();
            Assert.AreEqual(0, index.ListPositions().Count);

            foreach ((PathValue pv, Position p) in _testData)
            {
                index.Index(pv, p);
            }
            
            CollectionAssert.AreEquivalent(
                _testData.Select(x => x.Position).ToList(),
                index.ListPositions()
            );
        }
        
        [TestMethod]
        public void ListPositionsIndexedAtPathCorrect()
        {
            InvertedIndex index = new ();
            foreach ((PathValue pv, Position p) in _testData)
            {
                index.Index(pv, p);
            }
            
            CollectionAssert.AreEquivalent(
                _testData.Where(x=> x.PathValue.Path == "x").Select(x => x.Position).ToList(),
                index.ListPositionsIndexedAtPath("x")
            );
        }
        
        [TestMethod]
        [DataRow("x", "0")]
        [DataRow("y", "1")]
        [DataRow("z", "25")]
        public void QueryCorrect(string path, string value)
        {
            InvertedIndex index = new ();

            foreach ((PathValue pv, Position p) in _testData)
            {
                index.Index(pv, p);
            }
            PathValue query = new(path, value);
            
            List<Position> matches = index.Query(query);
            CollectionAssert.AreEquivalent(
                _testData
                    .Where(x => x.PathValue == query)
                    .Select(x => x.Position)
                    .ToList(),
                matches
            );
        }
    }
}