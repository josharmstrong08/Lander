// -----------------------------------------------------------------------
// <copyright file="IIndividual.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GeneticAlgorithm
{
    using System;

    /// <summary>
    /// Defines the interface for an individual in a population. And individual can implemented
    /// for a specific problem domain using this interface along with the <see cref="IIndividualFactory"/>
    /// interface. Individual must implement the <see cref="IComparable"/> interface by comparing
    /// fitness values so that individuals can be sorted by fitness.
    /// TODO: Switch from <see cref="ICloneable"/> to something different. Here it used to defined
    /// a *deep* copy, which is not immediately noticeable. 
    /// </summary>
    public interface IIndividual : ICloneable, IComparable<IIndividual>
    {
        /// <summary>
        /// Gets or sets the last calculated fitness value. 
        /// </summary>
        double Fitness { get; set; }

        /// <summary>
        /// Gets or sets the settings for this individual.
        /// </summary>
        IIndividualSettings Settings { get; set; }

        /// <summary>
        /// Mutates this individual.
        /// </summary>
        void Mutate();

        /// <summary>
        /// Performs crossover with a mate and returns a new child.
        /// </summary>
        /// <param name="mate">The individual to mate with.</param>
        /// <returns>A new child individual.</returns>
        IIndividual Crossover(IIndividual mate);

        /// <summary>
        /// Calculates the fitness and updates the Fitness property.
        /// </summary>
        void CalculateFitness();
    }
}
