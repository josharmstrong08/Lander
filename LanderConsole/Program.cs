using ArtificialNeuralNetwork;
using GeneticAlgorithm;
using Lander;
using Lander.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace LanderConsole
{
    class Program
    {
        private static NeuralNetwork neuralNetwork;

        static void Main(string[] args)
        {
            bool quit = false;
            string input;

            // NOTE: network topology defined here
            neuralNetwork = new NeuralNetwork();
            neuralNetwork.InputCount = 7;
            neuralNetwork.OutputCount = 2;
            neuralNetwork.AddHiddenLayer(5);

            PrintHelp();

            while (quit == false)
            {
                Console.Write("> ");
                input = Console.ReadLine();
                switch (input)
                {
                    case "q":
                    case "quit":
                        quit = true;
                        break;
                    case "t":
                    case "train":
                        Train(true);
                        break;
                    case "tl":
                    case "trainlong":
                        TrainLong();
                        break;
                    case "r":
                    case "run":
                        RunSimulation();
                        break;
                    case "ra":
                    case "runall":
                        RunSimulationVarying(true);
                        break;
                    case "h":
                    case "help":
                        PrintHelp();
                        break;
                    case "p":
                    case "print":
                        PrintNetwork();
                        break;
                    case "rs":
                    case "reset":
                        ResetNetwork();
                        break;
                    case "rg":
                    case "rungeneralize":
                        RunSimulationGeneralize();
                        break;
                    default:
                        Console.WriteLine("Unrecognized command. Try 'help'");
                        break;
                }
            }
        }

        private static void TrainLong()
        {
            double minFitness = double.MaxValue;
            double maxFitness = double.MinValue;
            double totalFitness = 0;
            double minLandedPercentage = double.MaxValue;
            double maxLandedPercentage = double.MinValue;
            NeuralNetwork bestNetwork;
            double totalLandedPercentage = 0;
            
            for (int i = 0; i < 10; i++)
            {
                double fitness = Train(false);
                double percentage = RunSimulationVarying(false);

                if (fitness < minFitness) minFitness = fitness;
                if (fitness > maxFitness) maxFitness = fitness;

            }
        }


        private static void PrintNetwork()
        {
            Console.WriteLine(neuralNetwork.ToString());
        }

        private static void PrintHelp()
        {
            Console.WriteLine("t (train): Train lander with ga");
            Console.WriteLine("tl (trainlong): Train lander with ga, time intensive");
            Console.WriteLine("rs (reset): Reset lander training");
            Console.WriteLine("r (run): Run simulation with random fixed parameters");
            Console.WriteLine("ra (runall): Run simulation with varying parameters");
            Console.WriteLine("rg (rungeneralize): Run the simulation with varying extra parameters");
            Console.WriteLine("q (quit): Quit program");
            Console.WriteLine("h (help): Display this help message");
            Console.WriteLine("p (print): Print out the current neural network");
        }

        private static void RunSimulationGeneralize()
        {
            int timesCrashed = 0;
            int timesLanded = 0;
            int runCount = 0;

            for (double varGravity = 0.1; varGravity < 1.0; varGravity += 0.1)
            {
                if (RunOneSimulation(false, 0, 100, 100, 0.1, varGravity) == LanderStatus.Crashed)
                {
                    timesCrashed++;
                }
                else
                {
                    timesLanded++;
                }

                runCount++;
            }

            for (double varGravity = 3.0; varGravity < 4.0; varGravity += 0.1)
            {
                if (RunOneSimulation(false, 0, 100, 100, 0.1, varGravity) == LanderStatus.Crashed)
                {
                    timesCrashed++;
                }
                else
                {
                    timesLanded++;
                }

                runCount++;
            }

            Console.WriteLine("Crashed: {0:p}", timesCrashed / (double)runCount);
            Console.WriteLine("Landed:  {0:p}", timesLanded / (double)runCount);
        }

        private static double RunSimulationVarying(bool showOutput)
        {
            int timesCrashed = 0;
            int timesLanded = 0;
            int runCount = 0;

            //for (double varWind = -1.0; varWind <= 1.0; varWind += 0.1)
            //{
                for (double varGravity = 1.0; varGravity < 3.0; varGravity += 0.1)
                {
                    if (RunOneSimulation(false, 0, 100, 100, 0.1, varGravity) == LanderStatus.Crashed)
                    {
                        timesCrashed++;
                    }
                    else
                    {
                        timesLanded++;
                    }

                    runCount++;
                }
            //}

            if (showOutput == true)
            {
                Console.WriteLine("Crashed: {0:p}", timesCrashed / (double)runCount);
                Console.WriteLine("Landed:  {0:p}", timesLanded / (double)runCount);
            }

            return timesLanded / (double)runCount;
        }

        private static void RunSimulation()
        {
            Random rand = new Random();

            //RunOneSimulation(true, 0, 100, 100, rand.NextDouble() * 2.0 - 1.0, rand.NextDouble() * 2.0 + 1.0);
            RunOneSimulation(true, 0, 100, 100, 0.1, rand.NextDouble() * 2.0 + 1.0);
            //RunOneSimulation(true, 0, 100, 100, 0.1, 2.0);
        }

        /// <summary>
        /// Runs the simulation with given starting variables.
        /// </summary>
        /// <returns></returns>
        private static LanderStatus RunOneSimulation(bool showOutput, double positionx, double positiony,
            double fuel, double windspeed, double gravity)
        {
            // First query the neural net to see how it reacts to the current conditions. 
            // Then update the conditions. 
            bool stop = false;

            if (showOutput)
            {
                Console.WriteLine("(X,Y,Vx,Vy,Fuel,Wind,Gravity) => (Burn,Thrust)");
            }

            Lander.Model.Environment env = new Lander.Model.Environment();
            env.Gravity = gravity;
            env.WindSpeed = windspeed;
            Lander.Model.Lander lander = new Lander.Model.Lander(env, fuel, positionx, positiony);

            while (stop == false)
            {
                // height, xPosition, Yvelocity, Xvelocity, wind, acceleration, and fuel.
                List<double> inputs = new List<double>();
                inputs.Add(lander.PositionX);
                inputs.Add(lander.PositionY);
                inputs.Add(lander.VelocityX);
                inputs.Add(lander.VelocityY);
                inputs.Add(env.WindSpeed);
                inputs.Add(env.Gravity);
                inputs.Add(lander.Fuel);

                IList<double> output = neuralNetwork.Run(inputs);

                lander.Burn = output[0];
                lander.Thrust = output[1];

                lander.Update();

                if (lander.Status != Lander.Model.LanderStatus.Flying)
                {
                    stop = true;
                }

                if (showOutput)
                { 
                    Console.CursorLeft = 0;
                    switch (lander.Status)
                    {
                        case Lander.Model.LanderStatus.Flying:
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("Flying:  ");
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        case Lander.Model.LanderStatus.Landed:
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("Landed:  ");
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case Lander.Model.LanderStatus.Crashed:
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.Write("Crashed: ");
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                    }

                    Console.Write("({0,6:f2},{1,6:f2},{2,6:f2},{3,6:f2},{4,6:f2},{5,6:f2},{6,6:f2}) => ({7,6:f2},{8,6:f2})",
                        lander.PositionX, lander.PositionY, lander.VelocityX, lander.VelocityY, lander.Fuel,
                        env.WindSpeed, env.Gravity, output[0], output[1]);
                    Thread.Sleep(250);
                }

                env.Update();
            }

            if (showOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Fitness: {0:f3}", lander.CalculateFitness());
            }

            return lander.Status;
        }

        private static void ResetNetwork()
        {
            List<double> weights = neuralNetwork.GetAllWeights();
            for (int i = 0; i < weights.Count; i++)
            {
                weights[0] = 0;
            }
            neuralNetwork.SetAllWeights(weights);
        }

        private static double Train(bool showOutput)
        {
            LanderIndividualSettings landerIndividualSettings = new LanderIndividualSettings();
            LanderIndividualFactory landerFactory = new LanderIndividualFactory();
            GeneticAlgorithm.GeneticAlgorithm ga = new GeneticAlgorithm.GeneticAlgorithm(landerFactory);
            LanderIndividual best = null;
            int currentIteration = 0;
            double bestfitness = 0;

            // Set up the ga
            Lander.Model.Environment environment = new Lander.Model.Environment();
            environment.Gravity = 2.0;
            environment.WindSpeed = 0.1;
            landerIndividualSettings.StartingFuel = 100;
            landerIndividualSettings.StartingHeight = 100;
            landerIndividualSettings.StartingHorizontal = 0;
            landerIndividualSettings.LanderEnvironment = environment;
            landerIndividualSettings.CrossoverAlgorithm = LanderIndividualSettings.CrossoverType.Uniform;
            ga.SelectionType = GeneticAlgorithm.GeneticAlgorithm.SelectionTypes.Tournament;
            ga.TournamentSize = 5;
            ga.CrossoverProbability = 0.98;
            ga.MutationProbability = 1;
            ga.CalculationLimit = 60000;
            ga.ElitistCount = 10;
            ga.PopulationSize = 500;
            landerFactory.IndividualSettings = landerIndividualSettings;

            // This lambda function handles the iteration events from the ga.
            EventHandler<IterationEventArgs> handler = (sender, args) =>
            {
                if (showOutput)
                {
                    Console.CursorLeft = 0;
                    Console.Write("{0,4:g}{1,12:f3}{2,12:f3}{3,12:f3}{4,12:f3}{5,12:f3}{6,12:f3}",
                        currentIteration,
                        args.MinFitness, args.AverageFitness, args.MaxFitness,
                        args.MinDifference, args.AverageDifference, args.MaxDifference);
                }

                currentIteration++;
                bestfitness = args.MinFitness;
            };

            ga.IterationEvent += handler;

            if (showOutput)
            {
                Console.WriteLine("{0,4}{1,12}{2,12}{3,12}{4,12}{5,12}{6,12}",
                "", "Min Fit", "Avg Fit", "Max Fit",
                "Min Dif", "Avg Dif", "Max Dif");
            }

            // Run the ga
            best = (LanderIndividual)ga.Run(landerIndividualSettings);
            Console.WriteLine();

            neuralNetwork.SetAllWeights(best.CurrentNeuralNetwork.GetAllWeights());

            ga.IterationEvent -= handler;

            return bestfitness;
        }
    }
}
