using ArtificialNeuralNetwork;
using GeneticAlgorithm;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Lander.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using Lander;
using System.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace LanderUI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The <see cref="LanderPositionX" /> property's name.
        /// </summary>
        public const string LanderPositionXPropertyName = "LanderPositionX";

        /// <summary>
        /// The <see cref="LanderPositionY" /> property's name.
        /// </summary>
        public const string LanderPositionYPropertyName = "LanderPositionY";

        /// <summary>
        /// The <see cref="LanderStatus" /> property's name.
        /// </summary>
        public const string LanderStatusPropertyName = "LanderStatus";

        /// <summary>
        /// The <see cref="IsTraining" /> property's name.
        /// </summary>
        public const string IsTrainingPropertyName = "IsTraining";

        private bool isTraining = false;

        private Lander.Model.Environment environment;

        private Lander.Model.Lander lander;

        private double landerPositionY = 0;

        private double landerPositionX = 0;

        private Timer timer;

        private RelayCommand exitCommand;

        private RelayCommand playCommand;

        private RelayCommand stopCommand;

        private RelayCommand trainCommand;

        private NeuralNetwork neuralNetwork;

        private string landerStatus;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.environment = new Lander.Model.Environment();
            this.lander = new Lander.Model.Lander(this.environment, 100, 0, 100);
            timer = new Timer(10);
            timer.Elapsed += new ElapsedEventHandler(UpdateLanderPosition);
            timer.AutoReset = true;
            this.LanderPositionX = this.lander.PositionX;
            this.LanderPositionY = this.lander.PositionY;

            this.MinFitnessValues = new ObservableCollection<Tuple<int, double>>();
            this.MaxFitnessValues = new ObservableCollection<Tuple<int, double>>();
            this.AvgFitnessValues = new ObservableCollection<Tuple<int, double>>();

            this.ExecuteTrainCommand();
        }

        /// <summary>
        /// Exits the program
        /// </summary>
        private void ExecuteExitCommand()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Start the simulation
        /// </summary>
        private void ExecutePlayCommand()
        {
            if (this.IsTraining == false)
            {
                this.lander.Reset();
                this.timer.Start();
                this.LanderPositionX = this.lander.PositionX;
                this.LanderPositionY = this.lander.PositionY;
            }
        }

        /// <summary>
        /// Stop the simulation
        /// </summary>
        private void ExecuteStopCommand()
        {
            this.lander.Reset();
            this.timer.Stop();
            this.LanderPositionX = this.lander.PositionX;
            this.LanderPositionY = this.lander.PositionY;
        }

        /// <summary>
        /// Run the genetic algorithm to train the neural network
        /// </summary>
        private void ExecuteTrainCommand()
        {
            this.ExecuteStopCommand();

            // The background worker allows the GA to be run in a different thread
            BackgroundWorker backgroundWorker = new BackgroundWorker();

            // Setup of the genetic algorithm
            LanderIndividualSettings landerIndividualSettings = new LanderIndividualSettings();
            LanderIndividualFactory landerFactory = new LanderIndividualFactory();
            GeneticAlgorithm.GeneticAlgorithm ga = new GeneticAlgorithm.GeneticAlgorithm(landerFactory);
            LanderIndividual best = null;
            int currentIteration = 0;

            // Set up the ga
            this.environment.Gravity = 2.0;
            this.environment.WindSpeed = 0.1;
            landerIndividualSettings.StartingFuel = 100;
            landerIndividualSettings.StartingHeight = 100;
            landerIndividualSettings.StartingHorizontal = 0;
            landerIndividualSettings.LanderEnvironment = this.environment;
            landerIndividualSettings.CrossoverAlgorithm = LanderIndividualSettings.CrossoverType.OnePoint;
            ga.SelectionType = GeneticAlgorithm.GeneticAlgorithm.SelectionTypes.Tournament;
            ga.TournamentSize = 3;
            ga.CrossoverProbability = 1;
            ga.MutationProbability = 1;
            ga.CalculationLimit = 30000;
            ga.ElitistCount = 10;
            ga.PopulationSize = 100;
            landerFactory.IndividualSettings = landerIndividualSettings;

            // This lambda function handles the iteration events from the ga.
            EventHandler<IterationEventArgs> handler = (sender, args) =>
            {
                backgroundWorker.ReportProgress(0, args);
            };

            // Set the work lambda function. 
            backgroundWorker.DoWork += (sender, e) =>
            {

                
                // Add the iteration event handler to report progress back to the main thread
                ga.IterationEvent += handler;

                // Run the ga
                best = (LanderIndividual)ga.Run(landerIndividualSettings);

                // Save the resulting neural network
                this.neuralNetwork = best.CurrentNeuralNetwork;

                ga.IterationEvent -= handler;

            };

            // Update the graph to show progress
            backgroundWorker.ProgressChanged += (sender, e) =>
            {
                this.MinFitnessValues.Add(new Tuple<int, double>(currentIteration, ((IterationEventArgs)e.UserState).MinFitness));
                this.MaxFitnessValues.Add(new Tuple<int, double>(currentIteration, ((IterationEventArgs)e.UserState).MaxFitness));
                this.AvgFitnessValues.Add(new Tuple<int, double>(currentIteration, ((IterationEventArgs)e.UserState).AverageFitness));
                currentIteration++;

                if (this.MinFitnessValues.Count > 600)
                {
                    this.MinFitnessValues.Clear();
                    this.MaxFitnessValues.Clear();
                    this.AvgFitnessValues.Clear();
                }
            };

            backgroundWorker.RunWorkerCompleted += (sender, e) =>
            {
                this.IsTraining = false;
            };
            
            // Clear out previous chart data and run the background worker
            this.MinFitnessValues.Clear();
            this.MaxFitnessValues.Clear();
            this.AvgFitnessValues.Clear();
            backgroundWorker.WorkerReportsProgress = true;
            this.IsTraining = true;
            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Handles the elapsed event of the timer. 
        /// </summary>
        /// <param name="source">The source of elapsed event</param>
        /// <param name="args">The elapsed event arguments</param>
        public void UpdateLanderPosition(object source, ElapsedEventArgs args)
        {
            // First query the neural net to see how it reacts to the current conditions. 
            // Then update the conditions. 

            // height, xPosition, Yvelocity, Xvelocity, wind, acceleration, and fuel.
            List<double> inputs = new List<double>();
            inputs.Add(this.lander.PositionY);
            inputs.Add(this.lander.PositionX);
            inputs.Add(this.lander.VelocityY);
            inputs.Add(this.lander.VelocityX);
            inputs.Add(this.environment.WindSpeed);
            inputs.Add(this.environment.Gravity);
            inputs.Add(this.lander.Fuel);
            IList<double> output = this.neuralNetwork.Run(inputs);
            this.lander.Burn = output[0];
            this.lander.Thrust = output[1];

            RaisePropertyChanging(LanderStatusPropertyName);
            this.lander.Update();
            this.LanderPositionX = this.lander.PositionX;
            this.LanderPositionY = this.lander.PositionY;
            RaisePropertyChanged(LanderStatusPropertyName);
            if (this.lander.Status != Lander.Model.LanderStatus.Flying)
            {
                this.timer.Stop();
            }

            this.environment.Update();
        }

        /// <summary>
        /// Gets the ExitCommand.
        /// </summary>
        public RelayCommand ExitCommand
        {
            get
            {
                return exitCommand ?? (exitCommand = new RelayCommand(ExecuteExitCommand));
            }
        }

        /// <summary>
        /// Gets the PlayCommand
        /// </summary>
        public RelayCommand PlayCommand
        {
            get
            {
                return this.playCommand ?? (this.playCommand = new RelayCommand(ExecutePlayCommand));
            }
        }

        /// <summary>
        /// Gets the StopCommand
        /// </summary>
        public RelayCommand StopCommand
        {
            get
            {
                return this.stopCommand ?? (this.stopCommand = new RelayCommand(ExecuteStopCommand));
            }
        }

        /// <summary>
        /// Gets the TrainCommand
        /// </summary>
        public RelayCommand TrainCommand
        {
            get
            {
                return this.trainCommand ?? (this.trainCommand = new RelayCommand(ExecuteTrainCommand));
            }
        }

        /// <summary>
        /// Sets and gets the LanderPositionX property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double LanderPositionX
        {
            get
            {
                return this.landerPositionX;
            }

            set
            {
                if (this.landerPositionX == value)
                {
                    return;
                }

                RaisePropertyChanging(MainViewModel.LanderPositionXPropertyName);
                this.landerPositionX = value;
                RaisePropertyChanged(MainViewModel.LanderPositionXPropertyName);
            }
        }

        /// <summary>
        /// Sets and gets the LanderPositionY property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double LanderPositionY
        {
            get
            {
                return landerPositionY;
            }

            set
            {
                if (landerPositionY == value)
                {
                    return;
                }

                RaisePropertyChanging(LanderPositionYPropertyName);
                landerPositionY = value;
                RaisePropertyChanged(LanderPositionYPropertyName);
            }
        }

        /// <summary>
        /// Gets a string version of the lander's status
        /// </summary>
        public string LanderStatus
        {
            get
            {
                switch (this.lander.Status)
                {
                    case Lander.Model.LanderStatus.Flying:
                        return "Flying";
                    case Lander.Model.LanderStatus.Landed:
                        return "Landed";
                    case Lander.Model.LanderStatus.Crashed:
                        return "Crashed";
                    default:
                        return "Unknown Status";
                }
            }
        }


        /// <summary>
        /// Sets and gets the IsTraining property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsTraining
        {
            get
            {
                return isTraining;
            }

            set
            {
                if (isTraining == value)
                {
                    return;
                }

                RaisePropertyChanging(IsTrainingPropertyName);
                isTraining = value;
                RaisePropertyChanged(IsTrainingPropertyName);
            }
        }

        /// <summary>
        /// Gets or sets the collection of minimum fitness values reported for each generation
        /// </summary>
        public ObservableCollection<Tuple<int, double>> MinFitnessValues { get; set; }

        /// <summary>
        /// Gets or sets the collection of maximum fitness values reported for each generation
        /// </summary>
        public ObservableCollection<Tuple<int, double>> MaxFitnessValues { get; set; }

        /// <summary>
        /// Gets or sets the average fitness value reported for each generation.
        /// </summary>
        public ObservableCollection<Tuple<int, double>> AvgFitnessValues { get; set; }
    }
}