// -----------------------------------------------------------------------
// <copyright file="LanderIndividualSettings.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace LanderSimulator
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
            /// One-point crossover
            /// </summary>
            OnePoint,

            /// <summary>
            /// Two-point crossover
            /// </summary>
            TwoPoint,

            /// <summary>
            /// Uniform crossover
            /// </summary>
            Uniform
        }

        /// <summary>
        /// Gets or sets the crossover algorithm to use.
        /// </summary>
        public CrossoverType CrossoverAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the environment to test the lander
        /// </summary>
        public Environment LanderEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the amount of starting fuel
        /// </summary>
        public double StartingFuel { get; set; }

        /// <summary>
        /// Gets or sets the starting Y position.
        /// </summary>
        public double StartingHeight { get; set; }

        /// <summary>
        /// Gets or sets the starting X position.
        /// </summary>
        public double StartingHorizontal { get; set; }
    }
}
