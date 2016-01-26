using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecisionTree.Test
{
    using System;
    using System.Collections.Generic;

    using Controller;

    using Attribute = ViewModel.Attribute;

    [TestClass]
    public class ControllerTest
    {
        [TestMethod]
        public void MountTree_AttributeOnlyGoal_ReturnAttrValue()
        {
            // Arrange
            var attr = new Attribute("Test", "g");
            attr.Values.AddRange(new[] { "TestValue", "TestValue2" });
            var solver = new DecisionTreeSolver(new List<Attribute> { attr });
            // Act
            var node = solver.MountTree();
            // Assert
            Assert.AreEqual(node.Attribute.Name, "Test");
        }

        [TestMethod]
        public void MountTree_GoalAttrOnlyOneValue_ReturnAttrValue()
        {
            // Arrange
            var attr = new Attribute("Test", "d");
            attr.Values.AddRange(new[] { "TestValue" });
            var attr2 = new Attribute("TestGoal", "g");
            attr2.Values.AddRange(new[] { "TestValue" });
            var solver = new DecisionTreeSolver(new List<Attribute> { attr, attr2 });
            // Act
            var node = solver.MountTree();
            // Assert
            Assert.AreEqual(node.Attribute.Name, "TestGoal");
        }

        [TestMethod]
        public void GoalValuesCount_ReturnsValuesCount()
        {
            // Arrange
            var solveMethods = new DTreeSolveMethods();
            var attr = new Attribute("TestAttr", "g") { Values = { "Test", "Test2", "Test2" } };
            // Act
            var count = solveMethods.GoalValuesCount(new List<Attribute> { attr });
            // Assert
            Assert.AreEqual(count["Test"], 1);
            Assert.AreEqual(count["Test2"], 2);
        }

        [TestMethod]
        public void GetMostCommonValue_ReturnsMostCommonValue()
        {
            // Arrange
            var solveMethods = new DTreeSolveMethods();
            var attr = new Attribute("TestAttr", "g") { Values = { "Test", "Test2", "Test2" } };
            // Act
            var value = solveMethods.GetMostFrequentlyValue(new List<Attribute> { attr });
            // Assert
            Assert.AreEqual("Test2", value);
        }

        [TestMethod]
        public void CalcEntropy_ReturnsEntropy()
        {
            // Arrange
            var dic = new Dictionary<string, int> { { "No", 5 }, { "Yes", 6 } };
            var solveMethods = new DTreeSolveMethods();
            // Act
            var entropy = solveMethods.GetEntropy(dic);
            // Assert
            Assert.IsTrue(Math.Abs(0.99403021147695647 - entropy) < 0.001);
        }
    }
}
