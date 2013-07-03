// -----------------------------------------------------------------------
// <copyright file="NeuralNetwork.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ArtificialNeuralNetwork
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using MathNet.Numerics;
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    /// Provides functions to create and run a simple multi-layer artificial neural network.
    /// Weights can be modified after initialization via the <see cref="GetAllWeights"/> and 
    /// <see cref="SetAllWeights"/> functions. 
    /// </summary>
    public class NeuralNetwork
    {
        /// <summary>
        /// Private backing field for the InputCount property.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// Private backing field for the OutputCount property.
        /// </summary>
        private int outputCount;

        /// <summary>
        /// The output weight matrix.
        /// </summary>
        private DenseMatrix outputweights;

        /// <summary>
        /// The list of hidden layer weight matrices.
        /// </summary>
        private List<DenseMatrix> weights;

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralNetwork"/> class.
        /// </summary>
        public NeuralNetwork()
        {
            this.weights = new List<DenseMatrix>();
            this.InputCount = 1;
            this.OutputCount = 1;
        }

        /// <summary>
        /// Gets or sets the number of input nodes to the neural network.
        /// </summary>
        public int InputCount
        {
            get { return this.inputCount; }
            set { this.inputCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of output nodes to the neural network.
        /// </summary>
        public int OutputCount
        {
            get 
            { 
                return this.outputCount; 
            }

            set 
            { 
                this.outputCount = value;
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

            ////Debug.WriteLine("Input: ");
            ////Debug.WriteLine(current.ToString());

            // Multiply each hidden lay matrix
            foreach (var matrix in this.weights)
            {
                ////Debug.WriteLine("Hidden layer:");
                ////Debug.WriteLine(current.ToString());
                ////Debug.WriteLine("*");
                ////Debug.WriteLine(matrix.ToString());
                ////Debug.WriteLine("=");
                current = current * matrix;
                ////Debug.WriteLine(current.ToString());

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

            ////Debug.WriteLine("Output layer:");
            ////Debug.WriteLine(current.ToString());
            ////Debug.WriteLine("*");
            ////Debug.WriteLine(this.outputweights.ToString());
            ////Debug.WriteLine("=");

            // Multiply the final output weight matrix
            current = current * this.outputweights;
            ////Debug.WriteLine(current.ToString());

            if (current.RowCount != 1)
            {
                throw new Exception("Row count is != 1 (" + current.RowCount + ")");
            }

            List<double> output = new List<double>();
            for (var col = 0; col < current.ColumnCount; col++)
            {
                output.Add(current[0, col]);

                // It is common for the outputs to go through activation as well, this line
                // will do that. I chose not to do it this way
                ////output.Add(this.CalculateActivation(current[0, col]));
            }
            
            return output;
        }

        /// <summary>
        /// Adds a new hidden layer right behind the output layer. This will 
        /// reset all the weights already set in the output layer; all the layers
        /// should be set up before weights are adjusted anyways. 
        /// </summary>
        /// <param name="nodecount">The number of nodes in the hidden layer.</param>
        public void AddHiddenLayer(int nodecount)
        {
            // The size of the hidden layer matrix varies with the size of the 
            // matrix to it's left (either the input layer or another hidden layer),
            // and the output weight matrix size will always change based on new matrix 
            // size. 
            if (this.weights.Count == 0)
            {
                this.weights.Add(new DenseMatrix(this.inputCount, nodecount));
            }
            else
            {
                this.weights.Add(new DenseMatrix(this.weights[this.weights.Count - 1].ColumnCount, nodecount));
            }

            this.outputweights = new DenseMatrix(this.weights[this.weights.Count - 1].ColumnCount, this.OutputCount);
        }

        /// <summary>
        /// Returns a list of all the weights in the network. These values should modified
        /// and then passed back to <see cref="SetAllWeights"/>. 
        /// </summary>
        /// <returns>A list containing all the weights in the network.</returns>
        public List<double> GetAllWeights()
        {
            List<double> weights = new List<double>();

            foreach (var weightMatrix in this.weights)
            {
                weights.AddRange(weightMatrix.ToColumnWiseArray());
            }

            weights.AddRange(this.outputweights.ToColumnWiseArray());

            return weights;
        }

        /// <summary>
        /// Sets the weights in the network. Generally the weights passed in should originate from
        /// the weights return from <see cref="GetAllWeights"/>.
        /// </summary>
        /// <param name="newWeights">The list of weights to set.</param>
        public void SetAllWeights(List<double> newWeights)
        {
            if (this.weights.Count == 0)
            {
                // There are no hidden layers, just output weights.
                if (newWeights.Count != this.InputCount * this.OutputCount)
                {
                    throw new ArgumentException("The number of weights passed in (" + newWeights.Count + 
                        ") does not match the number of weights in the network (" + (this.InputCount * this.OutputCount) + ").");
                }

                this.outputweights = new DenseMatrix(this.InputCount, this.OutputCount, newWeights.ToArray());
                ////this.outputweights = new DenseMatrix(this.outputweights.RowCount, this.outputweights.ColumnCount, newWeights.ToArray());
            }
            else
            {
                // TODO: Check the number of weights passed in and throw exception as necessary

                int currentIndex = 0;
                int rowCount = 0;
                int colCount = 0;

                // There is at least one hidden layer. More than one requires a loop
                if (this.weights.Count == 1)
                {
                    // Just one layer
                    rowCount = this.InputCount;
                    colCount = this.OutputCount;
                    this.weights[0] = new DenseMatrix(rowCount, colCount, newWeights.GetRange(currentIndex, rowCount * colCount).ToArray());
                    currentIndex += rowCount * colCount;
                }
                else
                {
                    // More than one layer
                    // Do the first layer using the input count
                    rowCount = this.InputCount; 
                    colCount = this.weights[0].ColumnCount;
                    this.weights[0] = new DenseMatrix(rowCount, colCount, newWeights.GetRange(currentIndex, rowCount * colCount).ToArray());
                    currentIndex += rowCount * colCount;

                    // Now do the layers after the first layer
                    for (var i = 1; i < this.weights.Count; i++)
                    {
                        rowCount = this.weights[i].RowCount;
                        colCount = this.weights[i].ColumnCount;
                        this.weights[i] = new DenseMatrix(rowCount, colCount, newWeights.GetRange(currentIndex, rowCount * colCount).ToArray());
                        currentIndex += rowCount * colCount;
                    }
                }

                // Now get the output weights
                rowCount = this.weights[this.weights.Count - 1].ColumnCount;
                colCount = this.OutputCount;
                this.outputweights = new DenseMatrix(rowCount, colCount, newWeights.GetRange(currentIndex, rowCount * colCount).ToArray());
            }
        }

        /// <summary>
        /// Removes all hidden layers. 
        /// </summary>
        public void ClearHiddenAllLayers()
        {
            this.weights.Clear();
            this.weights.Add(new DenseMatrix(1));
        }

        /// <summary>
        /// Sigmoid activation function for a node.
        /// </summary>
        /// <param name="input">The summed and weighted input to the node.</param>
        /// <returns>The output of a node.</returns>
        private double CalculateActivation(double input)
        {
            return 1 / (1 + Math.Pow(Constants.E, -input));
        }

        /// <summary>
        /// Returns a string representation of the neural network by printing out
        /// each matrix in order.
        /// </summary>
        /// <returns>A string representation of the network.</returns>
        public override string ToString()
        {
            string output = "";

            foreach (var matrix in this.weights)
            {

                output += matrix.ToString("f1", new NumberFormatInfo()) + "\n\n";
            }

            output += this.outputweights.ToString("f1", new NumberFormatInfo()) + "\n";
            return output;
        }
    }
}
