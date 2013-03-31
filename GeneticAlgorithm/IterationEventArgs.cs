// -----------------------------------------------------------------------
// <copyright file="IterationEvent.cs" company="">
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
    /// TODO: Update summary.
    /// </summary>
    public class IterationEventArgs : EventArgs
    {
        public IterationEventArgs(
            double minFitness,
            double maxFitness,
            double averageFitness)
        {
            this.MinFitness = minFitness;
            this.MaxFitness = maxFitness;
            this.AverageFitness = averageFitness;
        }

        public double MaxFitness { get; set; }
        public double MinFitness { get; set; }
        public double AverageFitness { get; set; }
    }
}
