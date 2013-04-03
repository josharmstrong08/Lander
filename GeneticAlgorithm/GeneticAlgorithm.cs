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
        /// The factory that produces individuals. 
        /// </summary>
        private IIndividualFactory individualFactory;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgorithm"/> class.
        /// </summary>
        /// <param name="individualFactory">The individual settings to use during this run.</param>
        public GeneticAlgorithm(IIndividualFactory individualFactory)
        {
            this.randomGenerator = new Random();
            this.individualFactory = individualFactory;
        }

        /// <summary>
        /// This event fires every iteration of the genetic algorithm to provide real time 
        /// statistical information.
        /// </summary>
        public event EventHandler<IterationEventArgs> IterationEvent;

        /// <summary>
        /// Available selection algorithms.
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
        public string CrossoverType { get; set; }

        /// <summary>
        /// Gets or sets the selection algorithm to use during selection.
        /// </summary>
        public SelectionTypes SelectionType { get; set; }

        /// <summary>
        /// Runs the genetic algorithm with the current settings. Blocks until the search is done. 
        /// </summary>
        /// <param name="settings">The individual settings to use during this run.</param>
        /// <returns>The best individual.</returns>
        public IIndividual Run(IIndividualSettings settings)
        {
            List<IIndividual> currentPopulation = new List<IIndividual>();
            List<IIndividual> newPopulation = new List<IIndividual>();
            List<TimeSpan> times = new List<TimeSpan>();
            Stopwatch timerFitness = new Stopwatch();
            Stopwatch timerTotal = new Stopwatch();

            // Give the individual factory the settings to use.
            // This might have to be a copy so the settings can be changed mid-run.
            this.individualFactory.IndividualSettings = settings;

            // Start timing
            timerTotal.Start();

            // Create the initial population.
            for (int i = 0; i < this.PopulationSize; i++)
            {
                currentPopulation.Add(this.individualFactory.CreateIndividual());
                currentPopulation[i].CalculateFitness();
            }
            
            // Start doing the search. 
            int calculations = 0;
            while (calculations < this.CalculationLimit)
            {
                // Sort the population by fitness so that we can do elitism.
                currentPopulation.Sort();

                // Calculate the genotype differences
                double differenceTotal = 0;
                double minDifference = double.MaxValue;
                double maxDifference = double.MinValue;
                for (int i = 0; i < currentPopulation.Count / 100; i++ )
                {
                    foreach (var individual2 in currentPopulation)
                    {
                        double difference = currentPopulation[i].CompareGenotype(individual2);
                        differenceTotal += difference;
                        if (difference < minDifference) minDifference = difference;
                        if (difference > maxDifference) maxDifference = difference;
                    }
                }
                double averageDifference = differenceTotal / (currentPopulation.Count * currentPopulation.Count);

                // Raise an iteration event with current statistical info.
                this.RaiseIterationEvent(new IterationEventArgs(
                    currentPopulation[0].Fitness,
                    currentPopulation[currentPopulation.Count - 1].Fitness,
                    (from item in currentPopulation select item.Fitness).Average(),
                    minDifference,
                    maxDifference,
                    averageDifference));

                // Elitism: Copy a certain number of the very best individuals to the new population.
                for (int i = 0; i < this.ElitistCount; i++)
                {
                    newPopulation.Add(currentPopulation[i]);
                }

                // For the rest of the population, use crossover and mutation to create new children.
                for (int i = this.ElitistCount; i < this.PopulationSize; i++)
                {
                    // Selection: Select two parents based on the specifed selection algorithm
                    List<IIndividual> parents = null;
                    switch (this.SelectionType)
                    {
                        case SelectionTypes.Tournament:
                            parents = this.TournamentSelection(currentPopulation);
                            break;
                        case SelectionTypes.RouletteWheel:
                            parents = this.RouletteWheelSelection(currentPopulation);
                            break;
                    }
                    
                    // Crossover: For a certain probability create a new child by crossing 
                    // over two individuals. Otherwise just copy the first parent chosen. 
                    IIndividual child;
                    if (this.randomGenerator.NextDouble() < this.CrossoverProbability)
                    {
                        child = parents[0].Crossover(parents[1]);
                    }
                    else
                    {
                        child = parents[0];
                    }

                    // Mutation: For a certain probability call the individual's mutate function.
                    if (this.randomGenerator.NextDouble() < this.MutationProbability)
                    {
                        child.Mutate();
                    }

                    // Now calculate the new child's fitness. 
                    timerFitness.Restart();
                    child.CalculateFitness();
                    timerFitness.Stop();

                    times.Add(timerFitness.Elapsed);
                    calculations++;

                    // Insert the child into the new population.
                    newPopulation.Add(child);
                }

                // Copy the new population into the current population.
                // Note: List copies the reference of each item in this operation.
                currentPopulation = new List<IIndividual>(newPopulation);

                // Now clear the population.
                newPopulation.Clear();
            }

            // We've passed the maxium fitness calculation limit. Sort the resulting population.
            currentPopulation.Sort();

            timerTotal.Stop();
            Debug.WriteLine("Best individual: " + currentPopulation[0] + " at " + currentPopulation[0].Fitness + " fitness");
            Debug.WriteLine("Total time: " + timerTotal.Elapsed);
            Debug.WriteLine("Totale time spend evaluating fitnesses: " + (times.Sum((time) => time.TotalMilliseconds) / 1000));
            Debug.WriteLine("Average time evaluating fitness: " + (times.Average((time) => time.TotalMilliseconds) / 1000));
            Debug.WriteLine("Fitness evaluation count: " + calculations);

            // Return the best individual
            return currentPopulation[0];
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
        private List<IIndividual> TournamentSelection(List<IIndividual> population)
        {
            List<IIndividual> tournament = new List<IIndividual>();
            for (int i = 0; i < this.TournamentSize; i++)
            {
                int index = this.randomGenerator.Next(0, population.Count);

                // If the index specifies one of the elitist individuals, then the individual
                // must be cloned otherwise 
                if (index < this.ElitistCount)
                {
                    tournament.Add((IIndividual)population[index].Clone());
                }
                else
                {
                    tournament.Add(population[index]);
                }
            }

            // Remove the all but the two best individuals.
            tournament.Sort();
            for (int i = 0; i < this.TournamentSize - 2; i++)
            {
                tournament.RemoveAt(tournament.Count - 1);
            }

            // Return the list with the two best individuals. 
            return tournament;
        }

        /// <summary>
        /// Performs roulette wheel selection on the given population and returns two parents in a list.
        /// </summary>
        /// <param name="population">The population to select from.</param>
        /// <returns>A list of two parent individuals.</returns>
        private List<IIndividual> RouletteWheelSelection(List<IIndividual> population)
        {
            List<IIndividual> children = new List<IIndividual>();
            double fitnesssum = 0;
            foreach (var guy in population)
            {
                fitnesssum += guy.Fitness;
            }

            double rand = this.randomGenerator.NextDouble() * fitnesssum;
            IIndividual guy1 = null;
            double rand2 = this.randomGenerator.NextDouble() * fitnesssum;
            IIndividual guy2 = null;
            for (int i = 0; i < population.Count; i++)
            {
                fitnesssum += population[i].Fitness;
                if (guy1 == null && rand < fitnesssum)
                {
                    if (i < this.ElitistCount)
                    {
                        guy1 = (IIndividual)population[i].Clone();
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
                        guy2 = (IIndividual)population[i].Clone();
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
