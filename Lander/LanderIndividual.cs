﻿// -----------------------------------------------------------------------
// <copyright file="LanderIndividual.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace LanderSimulator
{
    using ArtificialNeuralNetwork;
    using GeneticAlgorithm;
    using Model;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An individual that represents a lander controlled by a neural network. 
    /// </summary>
    public class LanderIndividual : IIndividual
    {
        /// <summary>
        /// Local copy of the neural net weights
        /// </summary>
        private List<double> weights;

        /// <summary>
        /// Neural net controlling the lander
        /// </summary>
        private NeuralNetwork neuralNet;

        /// <summary>
        /// Private backing field the Settings property
        /// </summary>
        private LanderIndividualSettings settings;

        private const int weightMin = -10;

        private const int weightMax = 10;

        /// <summary>
        /// Initialize a new <see cref="LanderIndividual"/> instance with Random values.
        /// </summary>
        public LanderIndividual(Random random)
        {
            // NOTE: network topology defined here
            this.neuralNet = new NeuralNetwork();
            this.neuralNet.InputCount = 7;
            this.neuralNet.OutputCount = 2;
            this.neuralNet.AddHiddenLayer(5);
            this.RandomGenerator = random;

            // Initialize the weights with random values between -1.0 and 1.0
            this.weights = this.neuralNet.GetAllWeights();
            for (int i = 0; i < this.weights.Count; i++)
            {
                this.weights[i] = this.RandomGenerator.NextDouble() * (weightMax - weightMin) - (weightMax - weightMin) / 2;
            }
            this.neuralNet.SetAllWeights(this.weights);
        }

        /// <summary>
        /// Mutates the weights of the neural net by adding a small delta value to one of the weights.
        /// </summary>
        public void Mutate()
        {
            this.weights = this.neuralNet.GetAllWeights();
            for (int i = 0; i < this.weights.Count / (double)100; i++)
            {
                ////this.weights[this.RandomGenerator.Next(this.weights.Count)] += this.RandomGenerator.NextDouble() * (weightMax - weightMin) - (weightMax - weightMin) / 2;
                this.weights[this.RandomGenerator.Next(this.weights.Count)] += this.RandomGenerator.NextDouble() * 0.2 - 0.1;
                ////this.weights[Random.Next(this.weights.Count)] += this.Random.NextDouble() * 2.0 - 0.5;
            }
            this.neuralNet.SetAllWeights(this.weights);
        }

        /// <summary>
        /// Performs crossover to generate a new child. The crossover type is specifed in the
        /// Settings property.
        /// </summary>
        /// <param name="mate">The individual to crossover with.</param>
        /// <returns>A new child individual</returns>
        public IIndividual Crossover(IIndividual mate)
        {
            if (mate.GetType() != typeof(LanderIndividual))
            {
                throw new ArgumentException("Mate must be a LanderIndividual");
            }

            // Get the weights in a easier to use form.
            this.weights = this.neuralNet.GetAllWeights();
            List<double> mateWeights = ((LanderIndividual)mate).neuralNet.GetAllWeights();

            // Create the child weights with a copy of this individual's weights initially
            List<double> childWeights = new List<double>(this.weights);

            switch (this.settings.CrossoverAlgorithm)
            {
                case LanderIndividualSettings.CrossoverType.OnePoint:
                    // Copy the mate's weights into the first section
                    int crossoverPoint = this.RandomGenerator.Next(this.weights.Count);
                    for (var i = 0; i < crossoverPoint; i++)
                    {
                        childWeights[i] = mateWeights[i];
                    }
                    break;
                case LanderIndividualSettings.CrossoverType.TwoPoint:
                    // Copy the mate's weights into the first section
                    int firstPoint = this.RandomGenerator.Next(this.weights.Count);
                    int secondPoint = this.RandomGenerator.Next(this.weights.Count);

                    if (secondPoint < firstPoint)
                    {
                        int temp = firstPoint;
                        firstPoint = secondPoint;
                        secondPoint = temp;
                    }

                    for (var i = 0; i < firstPoint; i++)
                    {
                        childWeights[i] = mateWeights[i];
                    }

                    for (var i = secondPoint; i < childWeights.Count; i++)
                    {
                        childWeights[i] = mateWeights[i];
                    }
                    break;
                case LanderIndividualSettings.CrossoverType.Uniform:
                    for (int i = 0; i < childWeights.Count; i++)
                    {
                        if (RandomGenerator.NextDouble() < 0.5)
                        {
                            childWeights[i] = mateWeights[i];
                        }
                    }
                    break;
                default:
                    throw new ArgumentException("Lander individual crossover type not supported");
            }

            // Create the new child
            LanderIndividual child = new LanderIndividual(this.RandomGenerator);
            child.Settings = this.Settings;
            child.neuralNet.SetAllWeights(childWeights);

            return child;
        }

        /// <summary>
        /// Creates a string representation of the individual.
        /// </summary>
        /// <returns>A string representation of the individual.</returns>
        public override string ToString()
        {
            this.weights = this.neuralNet.GetAllWeights();
            string output = "";
            foreach (var weight in weights)
            {
                output += weight + " ";
            }

            return output;
        }

        /// <summary>
        /// Gets or sets the fitness
        /// </summary>
        public double Fitness { get; set; }

        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        public Random RandomGenerator { get; set; }

        /// <summary>
        /// Updates the Fitness property by running a landing simulation.
        /// TODO implement running multiple simulations for varying environments.
        /// </summary>
        public void CalculateFitness()
        {
            List<double> inputs = new List<double>();
            Model.Environment environment = new Model.Environment();
            Lander lander = new Lander(environment, this.settings.StartingFuel, this.settings.StartingHorizontal, this.settings.StartingHeight);
            IList<double> output;

            for (int i = 0; i < 7; i++)
            {
                inputs.Add(0);
            }

            this.Fitness = 0;

            //for (double varWind = -1.0; varWind < 1.0; varWind += 0.1)
            //{
            //    this.settings.LanderEnvironment.WindSpeed = varWind;
            //    lander.Enviroment.WindSpeed = varWind;
                for (double varGravity = 1; varGravity < 3; varGravity += 0.5)
                {
                    //this.settings.LanderEnvironment.Gravity = varGravity;
                    lander.Enviroment.Gravity = varGravity;
                    lander.Reset();
                    do
                    {
                        inputs[0] = lander.PositionX;
                        inputs[1] = lander.PositionY;
                        inputs[2] = lander.VelocityX;
                        inputs[3] = lander.VelocityY;
                        inputs[4] = lander.Enviroment.WindSpeed;    // this.settings.LanderEnvironment.WindSpeed;
                        inputs[5] = lander.Enviroment.Gravity;      // varGravity; // this.settings.LanderEnvironment.Gravity;
                        inputs[6] = lander.Fuel;

                        output = this.neuralNet.Run(inputs);
                        lander.Burn = output[0];
                        lander.Thrust = output[1];

                        lander.Update();
                        //this.settings.LanderEnvironment.Update();
                    } while (lander.Status == LanderStatus.Flying);

                    this.Fitness += lander.CalculateFitness();
                }
            //}
        }

        /// <summary>
        /// Returns a deep clone of the individual.
        /// </summary>
        /// <returns>A deep clone of the individual.</returns>
        public object Clone()
        {
            LanderIndividual clone = new LanderIndividual(this.RandomGenerator);

            clone.neuralNet = new NeuralNetwork();
            clone.settings = this.settings;

            // NOTE: network topology defined here
            this.weights = this.neuralNet.GetAllWeights();
            clone.neuralNet.InputCount = this.neuralNet.InputCount;
            clone.neuralNet.OutputCount = this.neuralNet.OutputCount;
            clone.neuralNet.AddHiddenLayer(5);
            clone.neuralNet.SetAllWeights(this.weights);

            return clone;
        }

        /// <summary>
        /// Compares two individual by their fitness values. Lower fitness go first.
        /// </summary>
        /// <param name="other">The individual to compare to.</param>
        /// <returns>An integer specifying whether this individual is less than, equal to, or greater than other.</returns>
        public int CompareTo(IIndividual other)
        {
            if (other == null) return 1;

            return this.Fitness.CompareTo(other.Fitness);
        }

        /// <summary>
        /// Gets or sets this individual's settings.
        /// </summary>
        public IIndividualSettings Settings
        {
            get
            {
                return this.settings;
            }

            set
            {
                if (value.GetType() != typeof(LanderIndividualSettings))
                {
                    // TODO This check should be done at compile time and not run time
                    throw new ArgumentException("Settings type must be LanderIndividualSettings");
                }
                else
                {
                    this.settings = (LanderIndividualSettings)value;
                }
            }
        }

        /// <summary>
        /// Gets a reference to the neural network in this individual.
        /// </summary>
        public NeuralNetwork CurrentNeuralNetwork
        {
            get
            {
                return this.neuralNet;
            }
        }

        /// <summary>
        /// Compares genotypes
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double CompareGenotype(IIndividual other)
        {
            List<double> otherweights = ((LanderIndividual)other).neuralNet.GetAllWeights();
            this.weights = this.neuralNet.GetAllWeights();

            double difference = 0;

            for (int i = 0; i < this.weights.Count; i++)
            {
                difference += Math.Abs(this.weights[i] - otherweights[i]);
            }

            return difference;
        }
    }
}
