using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zds.Core.Queries;

namespace Zds.Tests
{
    [TestClass]
    public class PageTests
    {
        private static readonly int[] TestCollection = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};

        private static readonly Func<int, int, List<int>> GenerateRange =
            (start, count) => TestCollection[start..(start + count)].ToList();
        
        [TestMethod]
        public void HasNextCorrect()
        {
            var page = new Page<int>(TestCollection.Length, 3, GenerateRange);
            Assert.IsTrue(page.HasNext);
            page = page.Next();
            page = page!.Next();
            page = page!.Next();
            Assert.IsFalse(page!.HasNext);
        }
        
        [TestMethod]
        public void HasPrevCorrect()
        {
            var page = new Page<int>(TestCollection.Length, 3, GenerateRange);
            Assert.IsFalse(page.HasPrev);
            page = page.Next();
            Assert.IsTrue(page!.HasPrev);
        }
        
        [TestMethod]
        public void NextCorrect()
        {
            var page = new Page<int>(TestCollection.Length, 3, GenerateRange);
            CollectionAssert.AreEquivalent(TestCollection[..3], page.Results);
            
            page = page.Next();
            CollectionAssert.AreEquivalent(TestCollection[3..6], page!.Results);
            
            page = page.Next();
            CollectionAssert.AreEquivalent(TestCollection[6..9], page!.Results);
            
            page = page.Next();
            CollectionAssert.AreEquivalent(TestCollection[9..], page!.Results);
        }
        
        [TestMethod]
        public void PrevCorrect()
        {
            var page = new Page<int>(TestCollection.Length, 3, GenerateRange);
            page = page.Next();
            page = page!.Next();
            page = page!.Next();
            CollectionAssert.AreEquivalent(TestCollection[9..], page!.Results);
            
            page = page!.Prev();
            CollectionAssert.AreEquivalent(TestCollection[6..9], page!.Results);
            page = page!.Prev();
            CollectionAssert.AreEquivalent(TestCollection[3..6], page!.Results);
            page = page!.Prev();
            CollectionAssert.AreEquivalent(TestCollection[..3], page!.Results);
            page = page!.Prev();
            Assert.IsNull(page);
        }
    }
}