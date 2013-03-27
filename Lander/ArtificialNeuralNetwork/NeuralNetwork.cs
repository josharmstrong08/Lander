using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Diagnostics;

    namespace ArtificialNeuralNetwork
{
    public class NeuralNetwork
    {
        private int inputCount;

        private int outputCount;

        private DenseMatrix outputweights;

        private List<DenseMatrix> weights;

        public NeuralNetwork()
        {
            weights = new List<DenseMatrix>();
            this.InputCount = 1;
            this.OutputCount = 1;
        }

        private double CalculateActivation(double input)
        {
            return 1 / (1 + Math.Pow(Constants.E, -input));
        }

        /// <summary>
        /// Runs the neural net with the current weights and returns the result.
        /// </summary>
        /// <param name="inputs">The inputs to the neural net</param>
        /// <returns>The outputs from the neural net</returns>
        public IList<double> Run(IList<double> inputs)
        {
            if (inputs.Count != this.InputCount)
            {
                throw new ArgumentException("The number of items in 'inputs' must  match InputCount");
            }

            // Set up the inputs
            DenseMatrix current = new DenseMatrix(1, inputs.Count);
            for (var i = 0; i < inputs.Count; i++)
            {
                current[0, i] = inputs[i];
            }

            //Debug.WriteLine("Input: ");
            //Debug.WriteLine(current.ToString());

            foreach (var matrix in this.weights)
            {
                //Debug.WriteLine("Hidden layer:");
                //Debug.WriteLine(current.ToString());
                //Debug.WriteLine("*");
                //Debug.WriteLine(matrix.ToString());
                //Debug.WriteLine("=");
                current = current * matrix;
                //Debug.WriteLine(current.ToString());

                if (current.RowCount != 1)
                {
                    throw new Exception("Row count is != 1 (" + current.RowCount + ")");
                }

                for (var row = 0; row < current.RowCount; row++)
                {
                    for (var col = 0; col < current.ColumnCount; col++)
                    {
                        current[row, col] = this.CalculateActivation(current[row, col]);
                    }
                }
            }

            
            //Debug.WriteLine("Output layer:");
            //Debug.WriteLine(current.ToString());
            //Debug.WriteLine("*");
            //Debug.WriteLine(this.outputweights.ToString());
            //Debug.WriteLine("=");
            current = current * this.outputweights;
            //Debug.WriteLine(current.ToString());

            if (current.RowCount != 1)

            {
                throw new Exception("Row count is != 1 (" + current.RowCount + ")");
            }
            List<double> output = new List<double>();
            for (var col = 0; col < current.ColumnCount; col++)
            {
                output.Add(current[0, col]);
            }
            return output;
        }

        /// <summary>
        /// Adds a new hidden layer right behind the output layer. 
        /// </summary>
        /// <param name="nodecount"></param>
        public void AddHiddenLayer(int nodecount)
        {
            if (this.weights.Count == 0)
            {
                weights.Add(new DenseMatrix(this.inputCount, nodecount));
            }
            else
            {
                weights.Add(new DenseMatrix(weights[weights.Count - 1].ColumnCount, nodecount));
            }
            this.outputweights = new DenseMatrix(this.weights[this.weights.Count - 1].ColumnCount, this.OutputCount);
        }

        /// <summary>
        /// Removes all hidden layers. 
        /// </summary>
        public void ClearHiddenAllLayers()
        {
            weights.Clear();
            weights.Add(new DenseMatrix(1));
        }

        /// <summary>
        /// Gets or sets the number of input nodes to the neural network.
        /// </summary>
        public int InputCount
        {
            get { return inputCount; }
            set { this.inputCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of output nodes to the neural network.
        /// </summary>
        public int OutputCount
        {
            get 
            { 
                return outputCount; 
            }

            set 
            { 
                outputCount = value;
                if (this.weights.Count == 0)
                {
                    this.outputweights = new DenseMatrix(this.InputCount, value);
                }
                else
                {
                    this.outputweights = new DenseMatrix(this.weights[this.weights.Count - 1].ColumnCount, value);
                }
            }
        }
    }
}
