// -----------------------------------------------------------------------
// <copyright file="LanderIndividualFactory.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Lander
{
    using GeneticAlgorithm;

    /// <summary>
    /// 
    /// </summary>
    public class LanderIndividualFactory : IIndividualFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IIndividual CreateIndividual()
        {
            return new LanderIndividual() { Settings = this.IndividualSettings };
        }

        /// <summary>
        /// Gets or sets the settings to use on the new lander individuals.
        /// </summary>
        public IIndividualSettings IndividualSettings { get; set; }
    }
}
