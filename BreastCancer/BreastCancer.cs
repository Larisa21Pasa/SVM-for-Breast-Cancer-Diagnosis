/**************************************************************************
 *                                                                        *
 *  Copyright:   (c) 2023-2024, Echipa                                    *
 *  Description: BreastCancer Class                                       *
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BreastCancer
{
    /// <summary>
    /// Clasa care reprezinta problema de optimizare a clasificarii cancerului de san
    /// <summary>
    public class BreastCancer : IOptimizationProblem
    {

        /// <summary>
        /// Inițializează populația de cromozomi pentru algoritmul genetic. Datele sunt încărcate dintr-un fișier, 
        /// iar instanțele care conțin valori lipsă sunt eliminate pentru a asigura calitatea datelor. Selecționează 
        /// 70% din setul de date pentru antrenare și creează cromozomi cu aceste date
        /// </summary>
        /// <returns>Array de cromozomi care formează populația inițială</returns>
        public Chromosome[] PopulateChromosome()
        {
            //aici am numarul de indivizi -> pot sa am 50, 100 de indivizi in popylatie, fiecare cu cate 20 de gene 
            Chromosome[] population = new Chromosome[Constants.NumberOfIndivids];
            int lengthPopulation = population.Length;

            // Iterarea prin setul de date pentru a crea cromozomii.
            for (int i = 0; i < lengthPopulation ; ++i)
            {
                    // Crearea cromozomului și adăugarea acestuia în populație.
                    population[i] = CreateChromosome();
            }

            return population;
        }


        /// <summary>
        /// Metodă care face load din fisierul de date si creeaza un obiect de tip TrainingDataset
        /// </summary>
        /// <returns>Returnează un obiect de tip TrainingDataset</returns>
        public TrainingDataset  PopulateTrainingDataset()
        {
            // Citirea datelor din fișier și divizarea acestora în rânduri și coloane
            string[] lines = File.ReadAllLines("breast-cancer-wisconsin.data");
            var dataSet = lines.Select(line => line.Split(',')).ToArray();
            var clase = new int[Constants.NumberOfInstances];
            double[][] datasetInstances = new double[Constants.NumberOfInstances][];

            for (int i = 0; i < Constants.NumberOfInstances; ++i)
            {
                datasetInstances[i] = new double[Constants.NumberAtributesInstances];
            }

            // Iterarea prin setul de date pentru a crea cromozomii.
            for (int i = 0; i < Constants.NumberOfInstances; ++i)
            {
                // Eliminarea instanțelor cu valori lipsă (normalizare)
                if (!dataSet[i].Any(row => row.Contains("?")))
                {
                    // Conversia atributelor în valori numerice.
                    for (int j = 1; j < dataSet[i].Length - 1; ++j)
                    {
                        datasetInstances[i][j - 1] = double.Parse(dataSet[i][j]);
                    }

                    // Determinarea clasei cromozomului bazată pe ultimul atribut al datelor.
                    clase[i] = (dataSet[i][10] == "2") ? -1 : 1;
                }
            }

            return new TrainingDataset(clase, datasetInstances);
        }

        /// <summary>
        /// Metodă care face load din fisierul de date si creeaza un obiect de tip TrainingDataset
        /// </summary>
        /// <returns>Returnează un obiect de tip TrainingDataset</returns>
        public TestDataset PopulateTestDataset()
        {
            // Citirea datelor din fișier și divizarea acestora în rânduri și coloane
            string[] lines = File.ReadAllLines("breast-cancer-wisconsin_test.data");
            var dataSet = lines.Select(line => line.Split(',')).ToArray();
            var clase = new int[Constants.NumberOfInstances];
            double[][] datasetInstances = new double[Constants.NumberOfInstances][];

            for (int i = 0; i < Constants.NumberOfInstances; ++i)
            {
                datasetInstances[i] = new double[Constants.NumberAtributesInstances];
            }

            // Iterarea prin setul de date pentru a crea cromozomii.
            for (int i = 0; i < Constants.NumberOfInstances; ++i)
            {
                // Eliminarea instanțelor cu valori lipsă (normalizare)
                if (!dataSet[i].Any(row => row.Contains("?")))
                {
                    // Conversia atributelor în valori numerice.
                    for (int j = 1; j < dataSet[i].Length - 1; ++j)
                    {
                        datasetInstances[i][j - 1] = double.Parse(dataSet[i][j]);
                    }

                    // Determinarea clasei cromozomului bazată pe ultimul atribut al datelor.
                    clase[i] = (dataSet[i][10] == "2") ? -1 : 1;
                }
            }

            return new TestDataset(clase, datasetInstances);
        }

        /// <summary>
        /// Calculează fitness-ul unui cromozom pe baza propriilor gene si a setului de date din clasa TrainingDataset, conform metodologiei descrise în documentul de suport.
        /// </summary>
        /// <param name="chromosome">Cromozomul pentru care se calculează fitnessul.</param>
        /// <param name="trainingDataset">Setul de date utilizat pentru antrenare.</param>
        public void ComputeFitness(Chromosome chromosome, TrainingDataset trainingDataset)
        {
            double [] chromosomeGenes = chromosome.Genes;
            double sum1 = 0.0;

            // Calculează primul termen din formula fitness-ului
            // Acesta reprezintă suma valorilor genelor din cromozom
            for (int i = 0; i < Constants.NumberOfGenes; ++i)
            {
                sum1 += chromosomeGenes[i];
            }

            // Calculează al doilea termen din formula fitness-ului, care este o sumă de produse.
            double sum21 = 0.0;
            for (int j = 0; j < Constants.NumberOfGenes; ++j)
            {
                double sum22 = 0.0;

                // Parcurge populația și calculează produsul scalar între genele cromozomilor.
                for (int i = 0; i < Constants.NumberOfGenes; ++i)
                {
                    // Adaugă produsul scalar, înmulțit cu clasele și valoarea returnată de funcția kernel.
                    sum22 += chromosomeGenes[i] * chromosomeGenes[j]
                                    * trainingDataset.Clasa[i] * trainingDataset.Clasa[j]
                                    * Kernel(trainingDataset.Instances[i], trainingDataset.Instances[j]);
                   
                }

                sum21 += sum22;
            }

            // Calculează valoarea fitness-ului conform formulei date.
            chromosome.Fitness = -sum1 + 0.5 * sum21;
        }


        /// <summary>
        /// Creează un cromozom nou, apelând constructorul din clasa Chromosome.
        /// Această funcție este folosită pentru a inițializa cromozomii în populație.
        /// </summary>
        /// <returns>Noul cromozom creat.</returns>
        public Chromosome CreateChromosome()
        {
            double[] minValues = Enumerable.Range(0, Constants.NumberOfGenes).Select(_ => Constants.MinimOfAlpha).ToArray();
            double[] maxValues = Enumerable.Range(0, Constants.NumberOfGenes).Select(_ => Constants.MaximOfAlpha).ToArray();

            // Apelează constructorul Chromosome cu cei doi vectori
            return new Chromosome(Constants.NumberOfGenes, minValues, maxValues);
        }

        /// <summary>
        /// Implementează funcția de kernel RBF (Radial Basis Function) cunoscută și sub numele de Gaussian,
        /// utilizată pentru a măsura similaritatea în spațiul caracteristicilor în SVM.
        /// </summary>
        /// <param name="x">Primul vector de instanțe.</param>
        /// <param name="y">Al doilea vector de instanțe.</param>
        /// <returns>Valoarea calculată a kernelului RBF.</returns>
        public double Kernel(double[] x, double[] y)
        {
            // Calculează suma pătratelor diferențelor între elementele vectorilor,
            // ceea ce reprezintă pătratul distanței euclidiene.
            double sumOfSquares = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sumOfSquares += Math.Pow(x[i] - y[i], 2);
            }

            // Aplică transformarea RBF pe distanța calculată, ajustată de parametrul gamma.
            // Rezultatul este o măsură a similarității: cu cât doi vectori sunt mai apropiați,
            // cu atât valoarea kernelului este mai mare.
            return Math.Exp(-Constants.Gamma * Math.Sqrt(sumOfSquares));
        }


        /// <summary>
        /// Calculează valoarea biasului pentru întreaga populație de cromozomi.
        /// Biasul este o măsură care contribuie la ajustarea deciziilor luate de modelul SVM.
        /// </summary>
        /// <param name="population">Populația de cromozomi.</param>
        /// <returns>Valoarea calculată a biasului.</returns>
        public double Bias(Chromosome chromosome, TrainingDataset trainingDataset)
        {
            double l = chromosome.Genes.Length; // Lungimea genelor
            double sumaFinala = 0.0; // Inițializează suma finală pentru calculul biasului

            // Calculează suma necesară pentru determinarea biasului.
            for (int i = 0; i < l; ++i)
            {
                double sumInterior = 0;

                // Calculează suma produselor dintre clase, gene și valoarea kernelului.
                for (int j = 0; j < l; ++j)
                {
                    if (chromosome.Genes[0] != 0)
                    {
                        sumInterior += trainingDataset.Clasa[j] * chromosome.Genes[j] * Kernel(trainingDataset.Instances[i], trainingDataset.Instances[j]);

                    }
                }
                // Adaugă diferența dintre clasa cromozomului și suma calculată anterior.
                sumaFinala += trainingDataset.Clasa[i] - sumInterior;
            }

            // Returnează media valorilor calculate, reprezentând biasul.
            return (double)(1.0 / l) * sumaFinala;
        }


        /// <summary>
        /// Identifică și returnează vectorii suport din soluția algoritmului genetic.
        /// Vectorii suport sunt cromozomii critici care influențează determinarea hiperplanului în modelul SVM.
        /// Hiperplanul este o structură geometrică care separă diferitele clase de date în spațiul de caracteristici.
        /// </summary>
        /// <param name="chromosome"> Cromozomul pentru care se determină vectorii suport.</param>
        /// <returns>O listă de cromozomi care sunt vectori suport.</returns>
        public List<int> SVM(Chromosome chromosome)
        {
            List<int> supportVectors = new List<int>();

            // Iterează prin soluție pentru a identifica vectorii suport
            for (int i = 0; i < Constants.NumberOfGenes; i++)
            {
                // Un vector suport este identificat prin valoarea non-zero a genei (multiplicatorul Lagrange, α)
                if (chromosome.Genes[i] != 0.0)
                {
                    // Adaugă cromozomul la lista de vectori suport
                    supportVectors.Add(i);
                }
            }

            // Returnează lista de vectori suport
            return supportVectors;
        }

        /// <summary>
        /// Calculează hiperplanul (W) și marginea pentru clasificatorul SVM. Hiperplanul este un vector care ajută la separarea 
        /// datelor în clasificarea SVM, iar marginea reprezintă distanța dintre cele mai apropiate puncte ale claselor separate și hiperplan.
        /// </summary>
        /// <param name="chromosome">Cromozomul care conține genele (coeficienții) pentru calculul hiperplanului.</param>
        /// <param name="trainingDataset">Setul de date de antrenare care conține instanțele și clasele lor.</param>
        /// <returns>Vectorul W care reprezintă hiperplanul.</returns>
        public double[] WandM (Chromosome chromosome, TrainingDataset trainingDataset)
        {
            // Inițializarea vectorului W.
            double[] W = new double[Constants.NumberOfGenes];

            // Calculul valorilor lui W bazat pe genele cromozomului și datele de antrenament.
            Debug.WriteLine("\nValorile lui W sunt: ");
            for (int i = 0; i < Constants.NumberOfGenes; i++)
            {
                for (int j = 0; j < Constants.NumberOfGenes; j++)
                {
                    W[i] += chromosome.Genes[i] * trainingDataset.Clasa[i] * trainingDataset.Instances[i][j];
                }
                Debug.WriteLine(W[i]);
            }

            // Calculul marginii, care este inversul normei euclidiene a lui W.
            double margine = 0.0;
            double sumaEuclidiana = 0.0;
            for (int i = 0; i < Constants.NumberOfGenes; i++)
            {
                sumaEuclidiana += Math.Pow(W[i], 2);
            }

            sumaEuclidiana = Math.Sqrt(sumaEuclidiana);
            margine = 1 / sumaEuclidiana;

            Debug.WriteLine("\nMarginea este: " + margine);

            return W;
        }

        /// <summary>
        /// Verifică respectarea condiției ca suma produselor dintre genele cromozomului (α) și clasele instanțelor să fie zero.
        /// Această condiție este importantă pentru corectitudinea modelului SVM.
        /// </summary>
        /// <param name="chromosome">Cromozomul care conține genele (α) pentru verificare.</param>
        /// <param name="trainingDataset">Setul de date de antrenare care conține clasele instanțelor.</param>
        public void CalculareSumAlfaClasa(Chromosome chromosome, TrainingDataset trainingDataset)
        {
            double sum = 0.0;

            // Calculul sumei produselor dintre gene și clase.
            for (int i = 0; i < Constants.NumberOfGenes; i++)
            {
                sum += chromosome.Genes[i] * trainingDataset.Clasa[i];
            }

            // Afisarea sumei pentru verificare.
            Debug.WriteLine("\nSuma de alfa * clasa este: " + sum);
        }
    }
}
