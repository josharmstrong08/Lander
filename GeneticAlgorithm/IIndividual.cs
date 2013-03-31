using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    interface IIndividual : ICloneable, IComparable<IIndividual>
    {
        void Mutate();

        IIndividual Crossover(IIndividual mate);

        double Fitness { get; set; }

        void CalculateFitness();


    }
}
