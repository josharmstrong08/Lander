// -----------------------------------------------------------------------
// <copyright file="Individual.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class IndividualOld : IComparable<IndividualOld>, ICloneable
    {

        public enum CrossoverType
        {
            Cycle,
            PMX,
            Edge,
            Order
        }

        private Random generator;

        private List<double> englishContactTable;

        public IndividualOld(Random generator, List<double> englishContactTable, double badContactPenalty)
        { 
            this.generator = generator;
            this.englishContactTable = englishContactTable;
            this.BadContactPenalty = badContactPenalty;

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < 26; i++)
            {
                builder.Append((char)(i+97));
            }

            this.Genotype = builder.ToString();

            for (int i = 0; i < 100; i++)
            {
                this.Mutate();
            }
        }

        public string Genotype { get; set; }

        public double Fitness { get; set; }

        public double BadContactPenalty { get; set; }

        public string Decrypt(string text)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] >= 97 && text[i] <= 97 + 26)
                {
                    int index = 0;
                    while (this.Genotype[index] != text[i])
                    {
                        index++;
                    }
                    sb.Append(alphabet[index]);
                }
            }
            return sb.ToString();
        }

        public void CalculateFitness(string cleartext)
        {
            string plaintext = this.Decrypt(cleartext);

            //List<double> table = Enumerable.Repeat(0.0, 676).ToList();
            double[] table = new double[676];

            char char1, char2;
            for (int i = 0; i < plaintext.Length - 1; i++)
            {
                char1 = plaintext[i];
                char2 = plaintext[i + 1];
                table[(char1 - 97) * 26 + (char2 - 97)]++;
            }

            double sum = (plaintext.Length - 1);
            for (int i = 0; i < table.Length; i++)
            {
                table[i] /= sum;
            }

            sum = 0;
            for (int i = 0; i < 676; i++)
            {
                //Console.WriteLine(i + ": " + englishContactTable[i] + " " + table[i]);
                if (table[i] > 0 && englishContactTable[i] == 0.0)
                {
                    sum += this.BadContactPenalty;
                }
                else
                {
                    sum += (englishContactTable[i] - table[i]) * (englishContactTable[i] - table[i]);
                }
            }

            this.Fitness = sum;
        }

        public override string ToString()
        {
            return this.Genotype;
        }

        public void Mutate()
        {
            // swap two letters
            char[] chars = this.Genotype.ToCharArray();

            int index1 = this.generator.Next(0, 26);
            int index2 = this.generator.Next(0, 26);

            char temp = chars[index1];
            chars[index1] = chars[index2];
            chars[index2] = temp;

            this.Genotype = new string(chars);
        }

        public Individual Crossover(Individual parent, CrossoverType crossoverType)
        {
            switch (crossoverType)
            {
                case CrossoverType.Cycle:
                    return this.CycleCrossover(parent);
                case CrossoverType.PMX:
                    return this.PMXCrossover(parent);
                case CrossoverType.Edge:
                    return this.EdgeCrossover(parent);
                case CrossoverType.Order:
                    return this.OrderCrossover(parent);
                default:
                    throw new ArgumentException("Invalid crossover type");
            }
        }

        private Individual PMXCrossover(Individual parent)
        {
            int size = this.Genotype.Length;
            int k = 3;
            int[] a = new int[size];
            for (int i = 0; i < size; i++) a[i] = this.Genotype[i] - 97;
            int[] b = new int[size];
            for (int i = 0; i < size; i++) b[i] = parent.Genotype[i] - 97;
            int[] outgeno = new int[size];

            bool[] pick = new bool[size];
            int[] loca = new int[size];
            int[] locb = new int[size];
            double prob = (double)k / size;

            for (int i = 0; i < size; i++)
            {
                if (this.generator.NextDouble() < prob)
                {
                    pick[i] = true;
                    outgeno[i] = b[i];
                }
                else
                {
                    pick[i] = false;
                    outgeno[i] = -1;
                }
                loca[a[i]] = locb[b[i]] = i;
            }

            for (int i = 0; i < size; i++)
            {
                if (pick[i] && !pick[locb[a[i]]])
                {
                    int loc;
                    loc = i;
                    do
                    {
                        loc = loca[b[loc]];
                    }
                    while (outgeno[loc] != -1);

                    outgeno[loc] = a[i];
                }
            }

            for (int i = 0; i < size; i++)
            {
                if (outgeno[i] == -1)
                {
                    outgeno[i] = a[i];
                }
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                builder.Append((char)(outgeno[i] + 97));
            }
            Individual child = new Individual(this.generator, this.englishContactTable, this.BadContactPenalty);
            child.Genotype = builder.ToString();

            //Array.Sort(outgeno);
            //for (int i = 0; i < size; i++)
            //{
            //    Debug.Write(outgeno[i]);
            //}
            //Debug.WriteLine("");
            return child;


            /*
            double k = 2;
            bool[] pick = new bool[this.Genotype.Length];
            int[] locationa = new int[this.Genotype.Length];
            int[] locationb = new int[this.Genotype.Length];
            double prob = k / this.Genotype.Length;
            StringBuilder newgenotype = new StringBuilder();

            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if (this.generator.NextDouble() < prob)
                {
                    pick[i] = true;
                    newgenotype.Append(parent.Genotype[i]);
                }
                else
                {
                    pick[i] = true;
                    newgenotype.Append('$');
                }
                locationa[this.Genotype[i]-97] = locationb[parent.Genotype[i]-97] = i;
            }

            Debug.Write("PICK");
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                Debug.Write(pick[i] ? "*" : " ");
            }
            Debug.WriteLine("");
            */
            /*
             * void pmx(int *a, int *b, int *out, int k, int size) 
{
    bool pick[size];   // which elements to copy from B
    int loca[size];    // loca[x] is where is x in A
    int locb[size];    // locb[x] is where is x in B
    const double prob = (double)k/size;

    for (int i=0; i<size; i++) {
        if (choose(prob)) {
            pick[i] = true;        // select what to copy from B
            out[i] = b[i];         // copy from B
        }
        else {
            pick[i] = false;       
            out[i] = -1;           // if not copied from B then mark -1
        }
        loca[a[i]] = locb[b[i]] = i;   // set up reverse lookup for A and B
    }

    // print for to use as example
    printf("%4s", "PICK");
    for (int i=0; i<size; i++) printf("%2s ", (pick[i] ? "*" : " "));
    printf("\n");
             * */
            /*
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if (pick[i] == true && pick[locationb[this.Genotype[i]-97]] == false)
                {
                    int location = i;
                    do
                    {
                        location = locationa[parent.Genotype[location]-97];
                    }
                    while (newgenotype[location] != '$');
                    newgenotype[location] = (char)(locationa[i] + 97);
                }
            }

            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if (newgenotype[i] == '$')
                {
                    newgenotype[i] = this.Genotype[i];
                }
            }
            */
            /*

    // copy the lost elements into the duplicate elements
    for (int i=0; i<size; i++) {
        if (pick[i] && !pick[locb[a[i]]]) {
            // a[i] got replaced and is lost
            int loc;

            // find duplicate
            loc = i;
            do {
                loc = loca[b[loc]];
            }
            while (out[loc] != -1);

            out[loc] = a[i];  // copy lost element over duplicate
        }
    }

    // copy from A half where not already copied
    for (int i=0; i<size; i++) {
        if (out[i] == -1) out[i] = a[i];
    }
}        
             */
            /*
            Individual child = new Individual(this.generator, this.englishContactTable);
            child.Genotype = newgenotype.ToString();
            return child;
             * */
        }
        private Individual EdgeCrossover(Individual parent)
        {
            return parent;
        }
        private Individual OrderCrossover(Individual parent)
        {
            return parent;
        }

        private Individual CycleCrossover(Individual parent)
        {
            Individual child = new Individual(this.generator, this.englishContactTable, this.BadContactPenalty);

            int currentindex = 0;
            int startingindex = 0;
            int[] geneslookedat = new int[this.Genotype.Length];
            bool switchchildren = false;
            StringBuilder newgenotype = new StringBuilder("abcdefghijklmnopqrstuvwxyz");

            do
            {
                currentindex = startingindex;
                do
                {
                    if (switchchildren == false)
                    {
                        newgenotype[currentindex] = this.Genotype[currentindex];
                    }
                    else
                    {
                        newgenotype[currentindex] = parent.Genotype[currentindex];
                    }
                    geneslookedat[currentindex] = 1;

                    int secondparentgene = parent.Genotype[currentindex];
                    int newindex = -1;
                    for (int i = 0; i < this.Genotype.Length; i++)
                    {
                        if (this.Genotype[i] == secondparentgene)
                        {
                            newindex = i;
                            break;
                        }
                    }
                    if (newindex == -1)
                    {
                        Console.WriteLine("Error crossing over two genes");
                    }

                    currentindex = newindex;
                } while (currentindex != startingindex);
                switchchildren = !switchchildren;
                while (startingindex < this.Genotype.Length && geneslookedat[startingindex] == 1)
                {
                    startingindex++;
                }
            } while (startingindex < this.Genotype.Length);

            child.Genotype = newgenotype.ToString();

            return child;
        }

        public int CompareTo(Individual other)
        {
            if (other == null) return 1;

            return this.Fitness.CompareTo(other.Fitness);
        }

        public object Clone()
        {
            Individual clone = new Individual(this.generator, this.englishContactTable, this.BadContactPenalty);
            clone.Genotype = this.Genotype;
            clone.Fitness = this.Fitness;
            return clone;
        }
    }
}
