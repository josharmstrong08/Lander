// -----------------------------------------------------------------------
// <copyright file="LanderIndividual.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Lander
{
    using GeneticAlgorithm;
    using System;

    /// <summary>
    /// An individual that represents a lander controlled by a neural network. 
    /// </summary>
    public class LanderIndividual : IIndividual
    {
        private List<double> weights;

        /// <summary>
        /// Initialize a new <see cref="LanderIndividual"/> instance with random values.
        /// </summary>
        public LanderIndividual()
        {
            throw new NotImplementedException();
        }

        public void Mutate()
        {
            throw new NotImplementedException();
        }

        public IIndividual Crossover(IIndividual mate)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public double Fitness
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CalculateFitness()
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IIndividual other)
        {
            throw new NotImplementedException();
        }

        public IIndividualSettings Settings
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
