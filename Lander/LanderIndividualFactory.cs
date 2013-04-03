// -----------------------------------------------------------------------
// <copyright file="LanderIndividualFactory.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Lander
{
    using GeneticAlgorithm;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class LanderIndividualFactory : IIndividualFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private Random randomGenerator;

        /// <summary>
        /// 
        /// </summary>
        public LanderIndividualFactory()
        {
            this.randomGenerator = new Random();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IIndividual CreateIndividual()
        {
            return new LanderIndividual(this.randomGenerator) { Settings = this.IndividualSettings };
        }

        /// <summary>
        /// Gets or sets the settings to use on the new lander individuals.
        /// </summary>
        public IIndividualSettings IndividualSettings { get; set; }
    }
}
