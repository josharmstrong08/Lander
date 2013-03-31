// -----------------------------------------------------------------------
// <copyright file="GeneticAlgorithm.cs" company="Josh Armstrong">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GeneticAlgorithm
    {
        /// <summary>
        /// The random number generator for use in this class
        /// </summary>
        private Random randomGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgorithm"/> class. 
        /// </summary>
        public GeneticAlgorithm()
        {
            this.randomGenerator = new Random();
        }

        /// <summary>
        /// This event fires every iteration of the genetic algorithm to provide real time 
        /// statistical information.
        /// </summary>
        public event EventHandler<IterationEventArgs> IterationEvent;

        /// <summary>
        /// Available selection algorithms
        /// </summary>
        public enum SelectionTypes
        {
            /// <summary>
            /// Tournament selection
            /// </summary>
            Tournament,

            /// <summary>
            /// Roulette wheel selection
            /// </summary>
            RouletteWheel
        }

        /// <summary>
        /// Gets or sets the number of individuals in the population.
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        /// Gets or sets the the maximum number of times that the fitness will be calculated.
        /// </summary>
        public int CalculationLimit { get; set; }

        /// <summary>
        /// Gets or sets the number of highest ranking individuals that will be saved into the 
        /// next generation. 
        /// </summary>
        public int ElitistCount { get; set; }

        /// <summary>
        /// Gets or sets the probability that crossover is used to generate a child (otherwise
        /// the first parent is used).
        /// </summary>
        public double CrossoverProbability { get; set; }

        /// <summary>
        /// Gets or sets the probability that a new child will be mutated.
        /// </summary>
        public double MutationProbability { get; set; }

        /// <summary>
        /// Gets or sets the size of the tournament in tournament selection.
        /// </summary>
        public int TournamentSize { get; set; }

        /// <summary>
        /// Gets or sets the crossover algorithm to use during crossover.
        /// </summary>
        public Individual.CrossoverType CrossoverType { get; set; }

        /// <summary>
        /// Gets or sets the selection algorithm to use during selection.
        /// </summary>
        public SelectionTypes SelectionType { get; set; }

        /// <summary>
        /// Runs the genetic algorithm with the current settings. Blocks until the search is done. 
        /// </summary>
        /// <returns>The best individual.</returns>
        public Individual Run()
        {
            List<Individual> currentpopulation = new List<Individual>();
            List<Individual> newpopulation = new List<Individual>();
            List<TimeSpan> times = new List<TimeSpan>();
            Stopwatch timer = new Stopwatch();
            Stopwatch timer2 = new Stopwatch();

            timer2.Start();
            for (int i = 0; i < this.PopulationSize; i++)
            {
                
                //currentpopulation.Add(new Individual(this.randomGenerator, contacttable, this.BadContactPenalty));
                //currentpopulation[i].CalculateFitness(cleartext);
            }
            
            int calculations = 0;
            while (calculations < this.CalculationLimit)
            {
                currentpopulation.Sort();
                this.RaiseIterationEvent(new IterationEventArgs(
                    currentpopulation[0].Fitness,
                    currentpopulation[currentpopulation.Count - 1].Fitness,
                    (from item in currentpopulation select item.Fitness).Average()));

                for (int i = 0; i < this.ElitistCount; i++)
                {
                    newpopulation.Add(currentpopulation[i]);
                }

                for (int i = this.ElitistCount; i < this.PopulationSize; i++)
                {
                    // select
                    List<Individual> parents = null;
                    switch (this.SelectionType)
                    {
                        case SelectionTypes.Tournament:
                            parents = this.TournamentSelection(currentpopulation);
                            break;
                        case SelectionTypes.RouletteWheel:
                            parents = this.RouletteWheelSelection(currentpopulation);
                            break;
                    }
                    
                    // crossover
                    Individual child;
                    if (this.randomGenerator.NextDouble() < this.CrossoverProbability)
                    {
                        child = parents[0].Crossover(parents[1], this.CrossoverType);
                    }
                    else
                    {
                        child = parents[0];
                    }

                    // mutate
                    if (this.randomGenerator.NextDouble() < this.MutationProbability)
                    {
                        child.Mutate();
                    }

                    timer.Reset();
                    timer.Start();
                    //child.CalculateFitness(cleartext);
                    timer.Stop();
                    times.Add(timer.Elapsed);
                    calculations++;

                    // insert
                    newpopulation.Add(child);
                }

                currentpopulation = new List<Individual>(newpopulation);

                newpopulation.Clear();
            }

            currentpopulation.Sort();
            timer2.Stop();
            Debug.WriteLine("Best individual: " + currentpopulation[0].Genotype + " at " + currentpopulation[0].Fitness + " fitness");
            //Debug.WriteLine(currentpopulation[0].Decrypt(cleartext));
            Debug.WriteLine("Total time: " + timer2.Elapsed);
            Debug.WriteLine("Totale time spend evaluating fitnesses: " + (times.Sum((time) => time.TotalMilliseconds) / 1000));
            Debug.WriteLine("Average time evaluating fitness: " + (times.Average((time) => time.TotalMilliseconds) / 1000));
            Debug.WriteLine("Fitness evaluation count: " + calculations);

            return currentpopulation[0];
        }

        /// <summary>
        /// Fire a iteration event with the given arguments. 
        /// </summary>
        /// <param name="args">The iteration event arguments to send.</param>
        protected virtual void RaiseIterationEvent(IterationEventArgs args)
        {
            EventHandler<IterationEventArgs> handler = this.IterationEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Performs tournament selection on the given population of individuals and returns two 
        /// selected parents in a list.
        /// </summary>
        /// <param name="population">The population to select from.</param>
        /// <returns>A list of two selected parents.</returns>
        private List<Individual> TournamentSelection(List<Individual> population)
        {
            List<Individual> tournament = new List<Individual>();
            for (int i = 0; i < this.TournamentSize; i++)
            {
                int index = this.randomGenerator.Next(0, population.Count);
                if (index < this.ElitistCount)
                {
                    tournament.Add((Individual)population[index].Clone());
                }
                else
                {
                    tournament.Add(population[index]);
                }
            }

            tournament.Sort();
            for (int i = 0; i < this.TournamentSize - 2; i++)
            {
                tournament.RemoveAt(tournament.Count - 1);
            }

            return tournament;
        }

        /// <summary>
        /// Performs roulette wheel selection on the given population and returns two parents in a list.
        /// </summary>
        /// <param name="population">The population to select from.</param>
        /// <returns>A list of two parent individuals.</returns>
        private List<Individual> RouletteWheelSelection(List<Individual> population)
        {
            List<Individual> children = new List<Individual>();
            double fitnesssum = 0;
            foreach (var guy in population)
            {
                fitnesssum += guy.Fitness;
            }

            double rand = this.randomGenerator.NextDouble() * fitnesssum;
            Individual guy1 = null;
            double rand2 = this.randomGenerator.NextDouble() * fitnesssum;
            Individual guy2 = null;
            for (int i = 0; i < population.Count; i++)
            {
                fitnesssum += population[i].Fitness;
                if (guy1 == null && rand < fitnesssum)
                {
                    if (i < this.ElitistCount)
                    {
                        guy1 = (Individual)population[i].Clone();
                    }
                    else
                    {
                        guy1 = population[i];
                    }

                    if (guy2 != null)
                    {
                        break;
                    }
                }

                if (guy2 == null && rand < fitnesssum)
                {
                    if (i < this.ElitistCount)
                    {
                        guy2 = (Individual)population[i].Clone();
                    }
                    else
                    {
                        guy2 = population[i];
                    }

                    if (guy1 != null)
                    {
                        break;
                    }
                }
            }

            children.Add(guy1);
            children.Add(guy2);
            return children;
        }
    }
}
