// -----------------------------------------------------------------------
// <copyright file="IterationEventArgs.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides statistical information to passed as the arguments in an 
    /// IterationEvent in <see cref="GeneticAlgorithm"/>.
    /// </summary>
    public class IterationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IterationEventArgs"/> class.
        /// </summary>
        /// <param name="minFitness">The minimum fitness seen.</param>
        /// <param name="maxFitness">The maximum fitness seen.</param>
        /// <param name="averageFitness">The average fitness seen.</param>
        /// <param name="minDifference">The minimum genotype difference seen.</param>
        /// <param name="maxDifference">The maximum genotype difference seen.</param>
        /// <param name="averageDifference">The average genotype difference seen.</param>
        public IterationEventArgs(
            double minFitness,
            double maxFitness,
            double averageFitness,
            double minDifference,
            double maxDifference,
            double averageDifference)
        {
            this.MinFitness = minFitness;
            this.MaxFitness = maxFitness;
            this.AverageFitness = averageFitness;
            this.MinDifference = minDifference;
            this.MaxDifference = maxDifference;
            this.AverageDifference = averageDifference;
        }

        /// <summary>
        /// Gets or sets the maximum fitness.
        /// </summary>
        public double MaxFitness { get; set; }

        /// <summary>
        /// Gets or sets the minimum fitness.
        /// </summary>
        public double MinFitness { get; set; }

        /// <summary>
        /// Gets or sets the average fitness.
        /// </summary>
        public double AverageFitness { get; set; }

        /// <summary>
        /// Gets or sets the maximum genotype difference.
        /// </summary>
        public double MaxDifference { get; set; }

        /// <summary>
        /// Gets or sets the minimum genotype difference.
        /// </summary>
        public double MinDifference { get; set; }

        /// <summary>
        /// Gets or sets the average genotype difference.
        /// </summary>
        public double AverageDifference { get; set; }
    }
}
