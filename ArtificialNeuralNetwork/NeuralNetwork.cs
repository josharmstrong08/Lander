using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
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
        /// Returns a list of all the weights in the network. Generally these values should modified
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
        /// <param name="newWeights"></param>
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
                //this.outputweights = new DenseMatrix(this.outputweights.RowCount, this.outputweights.ColumnCount, newWeights.ToArray());
            }
            else
            {
                int currentIndex = 0;
                int rowCount = 0;
                int colCount = 0;

                // There are some hidden layers. 
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
                    this.weights[0] = new DenseMatrix(this.InputCount, this.weights[0])
                    for (var i = 1; i < this.weights.Count; i++)
                    {
                        this.weights[0] = new DenseMatrix()
                    }
                }

                // Now get the output weights
                rowCount = this.weights[this.weights.Count - 1].ColumnCount;
                colCount = this.OutputCount;
                this.outputweights = new DenseMatrix(this.weights[this.weights.Count-1].ColumnCount, this.OutputCount, newWeights.GetRange(currentIndex, ))
            }
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
