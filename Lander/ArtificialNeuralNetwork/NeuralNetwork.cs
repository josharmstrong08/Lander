using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtificialNeuralNetwork
{
    public class NeuralNetwork
    {
        private int inputCount;

        private int outputCount;

        private int hiddenLayerCount;

        private List<DenseMatrix> weights;

        public NeuralNetwork()
        {
            weights = new List<DenseMatrix>();
        }

        /// <summary>
        /// Runs the neural net with the current weights and returns the result.
        /// </summary>
        /// <param name="inputs">The inputs to the neural net</param>
        /// <returns>The outputs from the neural net</returns>
        public ICollection<double> Run(ICollection<double> inputs)
        {
            if (inputs.Count != this.InputCount)
            {
                throw new InvalidOperationException("Input count must match the net's input count");
            }

            return null;
        }

        /// <summary>
        /// Set the number of nodes in a given hidden layer
        /// </summary>
        /// <param name="nodecount"></param>
        public void SetHiddenLayerNodeCount(int nodecount)
        {

        }

        /// <summary>
        /// Gets or sets the number of input nodes to the neural network.
        /// </summary>
        public int InputCount
        {
            get { return inputCount; }
            set { inputCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of output nodes to the neural network
        /// </summary>
        public int OutputCount
        {
            get { return outputCount; }
            set { outputCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of hidden layers in the neural network. 
        /// </summary>
        public int HiddenLayerCount
        {
            get { return hiddenLayerCount; }
            set
            {
                if (this.hiddenLayerCount != value)
                {
                    hiddenLayerCount = value;
                    while (this.weights.Count < this.hiddenLayerCount)
                    {
                        this.weights.Add(new DenseMatrix(0));
                    }
                    while (this.weights.Count > this.hiddenLayerCount)
                    {
                        this.weights.RemoveAt(this.weights.Count - 1);
                    }
                }

            }
        }

    }
}
