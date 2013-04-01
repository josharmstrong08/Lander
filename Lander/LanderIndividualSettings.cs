// -----------------------------------------------------------------------
// <copyright file="LanderIndividualSettings.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Lander
{
    using GeneticAlgorithm;
    using Model;

    /// <summary>
    /// Defines genetic algorithm specific settings for neural network specific individuals.
    /// </summary>
    public class LanderIndividualSettings : IIndividualSettings
    {
        /// <summary>
        /// Defines the types of crossovers possible for neural network individuals.
        /// </summary>
        public enum CrossoverType
        {
            /// <summary>
            /// Cycle crossover
            /// </summary>
            Cycle,

            /// <summary>
            /// PMX crossover
            /// </summary>
            PMX
        }

        /// <summary>
        /// Gets or sets the crossover algorithm to use.
        /// </summary>
        public CrossoverType CrossoverAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the environment to test the lander
        /// </summary>
        public Environment LanderEnvironment { get; set; }
    }
}
