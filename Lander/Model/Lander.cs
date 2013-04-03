using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lander.Model
{
    public enum LanderStatus
    {
        Flying,
        Landed,
        Crashed
    }

    /// <summary>
    /// Length units could feet.
    /// Time units could be milliseconds
    /// Volume units could be gallons.
    /// 
    /// </summary>
    public class Lander
    {
        private double thrust;

        private double burn;

        private double fuel;

        private double startingFuel;

        private double positionX;

        private double positionY;

        private double startingPositionX;

        private double startingPositionY;

        private Environment environment;

        private LanderStatus status;

        private double minSafeX;

        private double maxSafeX;

        private double maxLandingVelocity;

        private double velocityX;

        private double velocityY;

        private int currentTime;

        /// <summary>
        /// Contrustor for the Lander class. 
        /// </summary>
        /// <param name="environment"></param>
        public Lander(Environment environment, double fuel, double positionx, double positiony)
        {
            // Load up with reasonable values
            this.PositionX = positionx;
            this.PositionY = positiony;
            this.startingPositionX = positionx;
            this.startingPositionY = positiony;
            this.Status = LanderStatus.Flying;
            this.Fuel = fuel;
            this.startingFuel = fuel;
            this.CurrentTime = 0;
            this.VelocityX = 0;
            this.VelocityY = 0;
            this.MaxLandingVelocity = -4.0;
            this.MinSafeX = -0.2;
            this.MaxSafeX = 0.2;
            this.Enviroment = environment;
            this.Thrust = 0;
            this.Burn = 0;
        }

        /// <summary>
        /// Call this function every time unit to update the lander's position based 
        /// on the current enviroment, and burn and thrust settings.
        /// </summary>
        /// <returns>The new lander status. </returns>
        public LanderStatus Update()
        {
            this.CurrentTime++;
            this.VelocityY -= this.Enviroment.Gravity;

            // Adjust velocity for burn
            //this.VelocityY += (this.Fuel - this.Burn >= 0 ? this.Burn : this.Burn - this.Fuel);
            if (this.Burn > this.Fuel)
            {
                this.VelocityY += this.Fuel;
            }
            else
            {
                this.VelocityY += this.Burn;
            }
            this.Fuel = Math.Max(this.Fuel - this.Burn, 0);

            // Adjust velocity for thrust
            //this.VelocityX += (this.Fuel - this.Thrust >= 0 ? this.Thrust : this.Thrust - this.Fuel);
            if (Math.Abs(this.Thrust) > this.Fuel)
            {
                this.VelocityX += (Math.Abs(this.Thrust) - this.Fuel) * (this.Thrust / Math.Abs(this.Thrust));
            }
            else
            {
                this.VelocityX += this.Thrust;
            }
            this.Fuel = Math.Max(this.Fuel - Math.Abs(this.Thrust), 0);

            // New position based on velocity
            this.PositionX += this.VelocityX + this.Enviroment.WindSpeed;
            this.PositionY += this.VelocityY;

            // Check if crashed
            if (this.PositionY > this.Enviroment.GetElevation(this.PositionX))
            {
                // We are still in the air, so flying.
                this.Status = LanderStatus.Flying;
            }
            else if (this.VelocityY < this.MaxLandingVelocity ||
              this.PositionX < this.MinSafeX ||
              this.PositionX > this.MaxSafeX)
            {
                // Now we are are on the ground and have broken one of the constraints
                this.status = LanderStatus.Crashed;
            }
            else
            {
                // All contraints passed, lander has landed. 
                this.status = LanderStatus.Landed;
                this.VelocityX = 0;
                this.VelocityY = 0;
                this.PositionY = this.Enviroment.GetElevation(this.PositionX);
            }

            return this.Status;
        }
        
        /// <summary>
        /// Calculates the fitness of the lander. If the lander is in the air then
        /// the fitness is defined as -1. If it has landed, then the fitness is 
        /// calculated based on the vertical velocity and distance from the landing
        /// zone. Smaller fitnesses are better, and a fitness of 0 means the lander
        /// has landed successfully. 
        /// </summary>
        /// <returns>The fitness of the lander</returns>
        public double CalculateFitness()
        {
            // Add to the fitness the positive difference between velocity and max velocity
            double fitness = Math.Max(0, this.MaxLandingVelocity - this.velocityY);

            // Add to the fitness the positive difference between x and max/min safe x
            if (this.PositionX < this.MinSafeX)
            {
                fitness += this.MinSafeX - this.PositionX;
            }
            else if (this.PositionX > this.MaxSafeX)
            {
                fitness += this.PositionX - this.MaxSafeX;
            }

            // If the lander has landed, add some extra signals

            if (this.Status == LanderStatus.Landed)
            {
                /*
                fitness -= Math.Max(
                                (this.VelocityY - this.MaxLandingVelocity) +
                                (this.Fuel) +
                                ((this.Fuel + this.startingPositionY) - this.CurrentTime),
                                0);
                 */
                //fitness -= this.Fuel;
            }

            return fitness;
        }

        /// <summary>
        /// Reset all internal values back to their initial state. (Velocity,
        /// Fuel, CurrentTime). 
        /// </summary>
        public void Reset()
        {
            this.CurrentTime = 0;
            this.Fuel = this.startingFuel;
            this.VelocityX = 0;
            this.VelocityY = 0;
            this.PositionX = this.startingPositionX;
            this.PositionY = this.startingPositionY;
            this.Status = LanderStatus.Flying;
            this.Thrust = 0;
            this.Burn = 0;
        }

        /// <summary>
        /// This is the current enviroment. 
        /// </summary>
        public Environment Enviroment
        {
            get { return this.environment; }
            set { this.environment = value; }
        }

        /// <summary>
        /// Gets or sets the burn setting (vertical adjustment) in volumne units per time unit
        /// </summary>
        public double Burn
        {
            get { return burn; }
            set
            {
                // Make sure burn is a positive number
                if (burn >= 0)
                {
                    burn = value;
                }
                else if (double.IsNaN(value) == true)
                {
                    throw new Exception("Invalid burn value");
                }
                else
                {
                    burn = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets current thrust setting of the lander (horizontal adjustments) in 
        /// volume units per time units. 
        /// </summary>
        public double Thrust
        {
            get { return thrust; }
            set { thrust = value; }
        }

        /// <summary>
        /// The current lander status.
        /// </summary>
        public LanderStatus Status 
        {
            get { return this.status; }
            private set { this.status = value; }
        }

        /// <summary>
        /// Gets the lander's current horizontal position in length units. 
        /// </summary>
        public double PositionX
        {
            get { return this.positionX; }
            private set { this.positionX = value; }
        }

        /// <summary>
        /// Gets the lander's current vertical position in length units. 
        /// </summary>
        public double PositionY
        {
            get { return this.positionY; }
            private set { this.positionY = value; }
        }

        public double MinSafeX
        {
            get { return minSafeX; }
            set { minSafeX = value; }
        }

        public double MaxSafeX
        {
            get { return maxSafeX; }
            set { maxSafeX = value; }
        }

        /// <summary>
        /// Gets or sets the maximum safe landing velocity for the lander, in length units per time units. 
        /// </summary>
        public double MaxLandingVelocity
        {
            get { return this.maxLandingVelocity; }
            set { this.maxLandingVelocity = value; }
        }

        /// <summary>
        /// Gets the amount of fuel the lander has in volume units. 
        /// </summary>
        public double Fuel
        {
            get { return this.fuel; }
            private set { this.fuel = value; }
        }

        /// <summary>
        /// Gets the lander's current horizontal velocity in length units per time units.
        /// </summary>
        public double VelocityX
        {
            get { return velocityX; }
            private set { velocityX = value; }
        }

        /// <summary>
        /// Gets the lander's current vertical velocity in length units per time units.h
        /// </summary>
        public double VelocityY
        {
            get { return velocityY; }
            private set { velocityY = value; }
        }

        public int CurrentTime
        {
            get { return this.currentTime; }
            private set { this.currentTime = value; }
        }
    }
}
