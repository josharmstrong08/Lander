using ArtificialNeuralNetwork;
using GeneticAlgorithm;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Lander.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;

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

        private Lander.Model.Environment environment;

        private Lander.Model.Lander lander;

        private double landerPositionY = 0;

        private double landerPositionX = 0;

        private Timer timer;

        private RelayCommand exitCommand;

        private RelayCommand playCommand;

        private RelayCommand stopCommand;

        private NeuralNetwork neuralNetwork;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.environment = new Environment();
            this.lander = new Lander.Model.Lander(this.environment, 100, 0, 100);
            this.environment.Gravity = .0322;
            timer = new Timer(10);
            timer.Elapsed += new ElapsedEventHandler(UpdateLanderPosition);
            timer.AutoReset = true;
            this.LanderPositionX = this.lander.PositionX;
            this.LanderPositionY = this.lander.PositionY;
            this.neuralNetwork = new NeuralNetwork();
            // Set up the network for it's seven inputs and two outputs. 
            this.neuralNetwork.InputCount = 7;
            this.neuralNetwork.OutputCount = 2;
            this.neuralNetwork.AddHiddenLayer(8);

            GeneticAlgorithm.GeneticAlgorithm ga = new GeneticAlgorithm.GeneticAlgorithm();
        }

        /// <summary>
        /// Exits the program
        /// </summary>
        private void ExecuteExitCommand()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Starts simulation
        /// </summary>
        private void ExecutePlayCommand()
        {
            this.lander.Reset();
            this.timer.Start();
            this.LanderPositionX = this.lander.PositionX;
            this.LanderPositionY = this.lander.PositionY;
            //List<double> inputs = new List<double>();
            //inputs.Add(1);
            //inputs.Add(2);
            //IList<double> output = neuralNetwork.Run(inputs);
            //foreach (var num in output)
            //{
            //    Debug.WriteLine(num);
            //}
        }

        private void ExecuteStopCommand()
        {
            this.lander.Reset();
            this.timer.Stop();
            this.LanderPositionX = this.lander.PositionX;
            this.LanderPositionY = this.lander.PositionY;
        }

        /// <summary>
        /// Handles the elapsed event of the timer. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
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

            this.lander.Update();
            this.LanderPositionX = this.lander.PositionX;
            this.LanderPositionY = this.lander.PositionY;
            if (this.lander.Status != LanderStatus.Flying)
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

    }
}