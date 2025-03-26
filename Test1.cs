/*

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Pb_Sci_Etape_1;

namespace ProbSciANATests
{
    [TestClass]
    public class GrapheTests
    {
        [TestMethod]
        public void TestEstConnexe()
        {
            // Arrange
            var adjacence = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2, 3 } },
                { 2, new List<int> { 1, 3 } },
                { 3, new List<int> { 1, 2 } }
            };

            // Act
            bool result = Program.EstConnexe(adjacence);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestEstCycle()
        {
            // Arrange
            var adjacence = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2, 3 } },
                { 2, new List<int> { 1, 3 } },
                { 3, new List<int> { 1, 2 } }
            };

            // Act
            bool result = Program.EstCycle(adjacence);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestEstPasConnexe()
        {
            // Arrange
            var adjacence = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2 } },
                { 2, new List<int> { 1 } },
                { 3, new List<int> { 4 } },
                { 4, new List<int> { 3 } }
            };

            // Act
            bool result = Program.EstConnexe(adjacence);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEstPasCycle()
        {
            // Arrange
            var adjacence = new Dictionary<int, List<int>>()
            {
                { 1, new List<int> { 2 } },
                { 2, new List<int> { 1, 3 } },
                { 3, new List<int> { 2 } }
            };

            // Act
            bool result = Program.EstCycle(adjacence);

            // Assert
            Assert.IsFalse(result);
        }
    }
}

*/