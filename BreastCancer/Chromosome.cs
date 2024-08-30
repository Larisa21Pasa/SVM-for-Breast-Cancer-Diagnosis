/**************************************************************************
 *                                                                        *
 *  Copyright:   (c) 2023-2024, Echipa                                    *
 *  Description: Chromosome Class                                         *
 *               (Proiect Inteligenta artificiala)                        *
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/

using System;

namespace BreastCancer
{
    /// <summary>
    /// Clasa Chromosome reprezintă un individ din populația algoritmului genetic.
    /// </summary>
    public class Chromosome
    {
        // Genes reprezintă vectorul de multiplicatori Lagrange, α_i, pentru fiecare instanță.
        public double[] Genes { get; set; }

        // Fitness evaluează cât de bine cromozomul rezolvă problema - mai mare este mai bine.
        public double Fitness { get; set; }
 
        // Numărul de gene pe care le conține cromozomul.
        public int NumberOfGenes { get; set; }

        // MinValues și MaxValues definesc limitele între care valorile genelor pot varia.
        public double[] MinValues { get; set; }
        public double[] MaxValues { get; set; }

        // O instanță statică Random pentru a genera valori aleatoare.
        private static readonly Random random = new Random();


        /// <summary>
        /// Constructorul inițializează un cromozom cu valori aleatoare în intervalul definit.
        /// </summary>
        /// <param name="numberOfGenes">Numarul de gene</param>
        /// <param name="minValues">Valoarea minima pe care o poate lua gena</param>
        /// <param name="maxValues">Valoarea maxima pe care o poate lua gena</param>
        public Chromosome(int numberOfGenes, double[] minValues, double[] maxValues)
        {
            NumberOfGenes = numberOfGenes;
            Genes = new double[NumberOfGenes];
            MinValues = new double[numberOfGenes];
            MaxValues = new double[numberOfGenes];

            // Generează valori aleatoare pentru fiecare genă între valorile minime și maxime.
            for (int i = 0; i < NumberOfGenes; ++i)
            {
                MinValues[i] = minValues[i];
                MaxValues[i] = maxValues[i];

                // O gena este o valoare aleatoare pentru alfa intre minim si maxim
                Genes[i] = minValues[i] + random.NextDouble() * (maxValues[i] - minValues[i]);
            }
        }

        /// <summary>
        /// Constructorul de copiere creează o copie exactă a unui cromozom existent.
        /// </summary>
        /// <param name="chromosome">Cromozomul care urmeaza a fi copiat</param>
        public Chromosome(Chromosome chromosome)
        {
            NumberOfGenes = chromosome.NumberOfGenes;

            Genes = new double[NumberOfGenes];
            MinValues = new double[chromosome.NumberOfGenes];
            MaxValues = new double[chromosome.NumberOfGenes];
            Fitness = chromosome.Fitness;

            // Copiază valorile din cromozomul dat ca parametru.
            for (int i = 0; i < NumberOfGenes; ++i)
            {
                MinValues[i] = chromosome.MinValues[i];
                MaxValues[i] = chromosome.MaxValues[i];

                Genes[i] = chromosome.Genes[i];
            }
        }

    }
}
