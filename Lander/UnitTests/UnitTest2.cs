using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArtificialNeuralNetwork;

namespace UnitTests
{
    [TestClass]
    public class NeuralNetTests
    {
        [TestMethod]
        public void Net1()
        {
            NeuralNetwork net = new NeuralNetwork();
            net.InputCount = 2;
            net.OutputCount = 2;

        }
    }
}
