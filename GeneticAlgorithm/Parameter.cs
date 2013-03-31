// -----------------------------------------------------------------------
// <copyright file="Parameter.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Parameter
    {
        public Parameter(string name, object value, Type type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }

        public string Name { get; set; }

        public object Value { get; set; }

        public Type Type { get; set; }
    }
}
