using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanderSimulator.Model
{
    public class Environment
    {
        private double gravity;

        private double windSpeed;

        private Random random;

        public Environment()
        {
            this.random = new Random();
            this.Gravity = 2.0;
            //this.WindSpeed = 0.2 * (this.random.NextDouble() - 0.5);
            this.WindSpeed = 0.1;
        }

        /// <summary>
        /// Call this once every time unit to update the wind speed.
        /// </summary>
        public void Update()
        {
            // NYI
        }

        /// <summary>
        /// This function calculates the elevation (y value) of the given x position.
        /// Currently it simply returns 0, for a completely flat landscape. 
        /// </summary>
        /// <param name="xposition">The x position to look at, in length units.</param>
        /// <returns>The current y position, in length units.</returns>
        public double GetElevation(double xposition)
        {
            return 0;
        }

        /// <summary>
        /// Gets or sets the acceleration due to gravity, in length units squared per time unit. 
        /// </summary>
        public double Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }

        /// <summary>
        /// Gets or sets the current wind speed, in length units per time units. 
        /// </summary>
        public double WindSpeed
        {
            get { return windSpeed; }
            set { windSpeed = value; }
        }
    }
}
