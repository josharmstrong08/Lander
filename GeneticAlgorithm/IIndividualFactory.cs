using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    interface IIndividualFactory
    {
        IIndividual CreateIndividual();
    }
}
