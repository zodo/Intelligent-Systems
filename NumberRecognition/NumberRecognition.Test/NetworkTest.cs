namespace NumberRecognition.Test
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NetworkTest
    {

        [TestMethod]
        public void XorNetworkCalculation()
        {
            // Arrange
            var network = new Network(2, 10, 1);

            var datasets = new List<DataSet>
            {
                new DataSet(new double[] { 0, 0 }, new double[] { 0 }),
                new DataSet(new double[] { 0, 1 }, new double[] { 1 }),
                new DataSet(new double[] { 1, 0 }, new double[] { 1 }),
                new DataSet(new double[] { 1, 1 }, new double[] { 0 })
            };

            // Act
            network.Train(datasets, 0.05);
            var answer = network.Compute(new double[] {1, 1});
            // Assert
            Assert.IsTrue(answer.First() < 0.1);
        }
    }
}