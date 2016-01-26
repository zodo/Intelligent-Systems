namespace NumberRecognition.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NumberRecognition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NetworkTest
    {

        [TestMethod]
        public void NeuronInitialize_CorrectlyInitialize()
        {
            // Arrange
            var neuron = new Neuron(42);
            // Act
            var inputsCount = neuron.Inputs;
            var weightsCount = neuron.Weights.Count;
            var deltaCount = neuron.Deltas.Count;
            // Assert
            Assert.AreEqual(42, inputsCount);
            Assert.AreEqual(42, weightsCount);
            Assert.AreEqual(42, deltaCount);
        }

        [TestMethod]
        public void Layer()
        {
            // Arrange
            var network = new Web(2, 2);
            var tuple = new Tuple<byte[], byte[]>(new byte[] { 0, 1 }, new byte[] { 1, 0 });
            var tuples = Enumerable.Range(0, 100).Select(x => tuple);
            // Act
            network.Teach(tuples, 1);
            var answer = network.Recognize(new byte[] { 0, 1 });
            // Assert
            Assert.IsTrue(1 - answer.First() < 0.05);
        }


    }
}