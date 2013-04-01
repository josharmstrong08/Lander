// -----------------------------------------------------------------------
// <copyright file="IIndividualFactory.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GeneticAlgorithm
{
    /// <summary>
    /// Defines the interface for a factory to produce individuals with specified settings. Classes
    /// implementing this interface can produce individual for use in the genetic algorithm.
    /// </summary>
    public interface IIndividualFactory
    {
        /// <summary>
        /// Gets or sets the settings to use in all individuals created using this factory.
        /// </summary>
        IIndividualSettings IndividualSettings { get; set; }

        /// <summary>
        /// Creates a new individual.
        /// </summary>
        /// <returns>A new individual.</returns>
        IIndividual CreateIndividual();
    }
}
