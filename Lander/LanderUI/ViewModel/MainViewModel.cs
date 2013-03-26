using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LanderSimulator;
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

        private LanderSimulator.Environment enviromenment;

        private Lander lander;

        private double landerPositionY = 0;

        private double landerPositionX = 0;

        private Timer timer;

        private RelayCommand exitCommand;

        private RelayCommand playCommand;

        private RelayCommand stopCommand;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.enviromenment = new Environment();
            this.lander = new Lander(this.enviromenment, 100, 0, 100);
            this.enviromenment.Gravity = .0322;
            timer = new Timer(10);
            timer.Elapsed += new ElapsedEventHandler(UpdateLanderPosition);
            timer.AutoReset = true;
            timer.Start();
            this.LanderPositionX = this.lander.PositionX;
            this.LanderPositionY = this.lander.PositionY;
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
            this.enviromenment.Update();
            this.lander.Update();
            this.LanderPositionX = this.lander.PositionX;
            this.LanderPositionY = this.lander.PositionY;
            if (this.lander.Status != LanderStatus.Flying)
            {
                this.timer.Stop();
            }
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