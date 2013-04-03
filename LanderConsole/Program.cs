using ArtificialNeuralNetwork;
using GeneticAlgorithm;
using Lander;
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
        private static Lander.Model.Lander lander;

        private static Lander.Model.Environment environment;

        private static NeuralNetwork neuralNetwork;

        static void Main(string[] args)
        {
            bool quit = false;
            string input;

            environment = new Lander.Model.Environment();
            lander = new Lander.Model.Lander(environment, 100, 0, 100);
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
                        Train();
                        break;
                    case "r":
                    case "run":
                        RunSimulation();
                        break;
                    case "h":
                    case "help":
                        PrintHelp();
                        break;
                    case "p":
                    case "print":
                        PrintNetwork();
                        break;
                    default:
                        Console.WriteLine("Unrecognized command. Try 'help'");
                        break;
                }
            }
        }

        private static void PrintNetwork()
        {
            Console.WriteLine(neuralNetwork.ToString());
        }

        private static void PrintHelp()
        {
            Console.WriteLine("t (train): Train lander with ga");
            Console.WriteLine("r (run): Run simulation");
            Console.WriteLine("q (quit): Quit program");
            Console.WriteLine("h (help): Display this help message");
            Console.WriteLine("p (print): Print out the current neural network");
        }

        private static void RunSimulation()
        {
            // First query the neural net to see how it reacts to the current conditions. 
            // Then update the conditions. 
            bool stop = false;

            Console.WriteLine("(X,Y,Vx,Vy,Fuel,Wind,Gravity) => (Burn,Thrust)");

            lander.Reset();

            while (stop == false)
            {
                // height, xPosition, Yvelocity, Xvelocity, wind, acceleration, and fuel.
                List<double> inputs = new List<double>();
                inputs.Add(lander.PositionX);
                inputs.Add(lander.PositionY);
                inputs.Add(lander.VelocityY);
                inputs.Add(lander.VelocityX);
                inputs.Add(environment.WindSpeed);
                inputs.Add(environment.Gravity);
                inputs.Add(lander.Fuel);

                IList<double> output = neuralNetwork.Run(inputs);
                lander.Burn = output[0];
                lander.Thrust = output[1];

                lander.Update();

                if (lander.Status != Lander.Model.LanderStatus.Flying)
                {
                    stop = true;
                }

                Console.CursorLeft = 0;
                switch (lander.Status)
                {
                    case Lander.Model.LanderStatus.Flying:
                        Console.Write("Flying:  ");
                        break;
                    case Lander.Model.LanderStatus.Landed:
                        Console.Write("Landed:  ");
                        break;
                    case Lander.Model.LanderStatus.Crashed:
                        Console.Write("Crashed: ");
                        break;
                }

                Console.Write("({0:f2},{1:f2},{2:f2},{3:f2},{4:f2},{5:f2},{6:f2}) => ({7:f2},{8:f2})",
                    lander.PositionX, lander.PositionY, lander.VelocityX, lander.VelocityY, lander.Fuel,
                    environment.WindSpeed, environment.Gravity, output[0], output[1]);

                environment.Update();

                Thread.Sleep(1000);
            }

            Console.WriteLine();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Train()
        {
            LanderIndividualSettings landerIndividualSettings = new LanderIndividualSettings();
            LanderIndividualFactory landerFactory = new LanderIndividualFactory();
            GeneticAlgorithm.GeneticAlgorithm ga = new GeneticAlgorithm.GeneticAlgorithm(landerFactory);
            LanderIndividual best = null;
            int currentIteration = 0;

            // Set up the ga
            environment.Gravity = 2.0;
            environment.WindSpeed = 0.1;
            landerIndividualSettings.StartingFuel = 100;
            landerIndividualSettings.StartingHeight = 100;
            landerIndividualSettings.StartingHorizontal = 0;
            landerIndividualSettings.LanderEnvironment = environment;
            landerIndividualSettings.CrossoverAlgorithm = LanderIndividualSettings.CrossoverType.OnePoint;
            ga.SelectionType = GeneticAlgorithm.GeneticAlgorithm.SelectionTypes.Tournament;
            ga.TournamentSize = 3;
            ga.CrossoverProbability = 0.99;
            ga.MutationProbability = 1;
            ga.CalculationLimit = 40000;
            ga.ElitistCount = 3;
            ga.PopulationSize = 100;
            landerFactory.IndividualSettings = landerIndividualSettings;

            // This lambda function handles the iteration events from the ga.
            EventHandler<IterationEventArgs> handler = (sender, args) =>
            {
                Console.CursorLeft = 0;
                Console.Write("{0:g}\t{1:f3}\t{2:f3}\t{3:f3}\t{4:f3}\t{5:f3}\t{6:f3}", 
                    currentIteration, 
                    args.MinFitness, args.AverageFitness, args.MaxFitness, 
                    args.MinDifference, args.AverageDifference, args.MaxDifference);
                currentIteration++;
            };

            ga.IterationEvent += handler;

            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", 
                "", "Min Fit", "Avg Fit", "Max Fit",
                "Min Dif", "Avg Dif", "Max Dif");

            // Run the ga
            best = (LanderIndividual)ga.Run(landerIndividualSettings);
            Console.WriteLine();

            neuralNetwork.SetAllWeights(best.CurrentNeuralNetwork.GetAllWeights());

            ga.IterationEvent -= handler;
        }
    }
}
