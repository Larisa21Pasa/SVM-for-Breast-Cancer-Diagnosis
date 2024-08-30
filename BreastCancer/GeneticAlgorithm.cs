/**************************************************************************
 *                                                                        *
 *  Copyright:   (c) 2023-2024, Echipa                                    *
 *  Description: GeneticAlgorithm Class                                   *
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
using System.Linq;

namespace BreastCancer
{
    /// <summary>
    /// Clasa GeneticAlgorithm implementeaza algoritmul genetic pentru problema de optimizare a clasificarii cancerului mamar 
    /// </summary>
    public class GeneticAlgorithm
    {
        private BreastCancer _breastCancer;
        private TrainingDataset _trainingDataset;

        private static Random _rand = new Random();

        /// <summary>
        /// Constructorul clasei GeneticAlgorithm inițializează algoritmul cu o instanță specifică a problemei BreastCancer.
        /// Această asociere permite algoritmului să acceseze funcții și date specifice problemei de optimizare a clasificării cancerului mamar,
        /// cum ar fi evaluarea fitness-ului, generarea populației inițiale și ajustarea parametrilor specifici.
        /// </summary>
        /// <param name="breastCancer">Instanța problemei BreastCancer, reprezentând contextul specific și datele pentru algoritm.</param>
        public GeneticAlgorithm(BreastCancer breastCancer, TrainingDataset trainingDataset)
        {
            _breastCancer = breastCancer;
            _trainingDataset = trainingDataset;
        }


        /// <summary>
        /// Metoda Solve implementează logica principală a algoritmului genetic pentru optimizarea SVM în contextul clasificării cancerului mamar.
        /// Inițializează populația de cromozomi, calculează fitness-ul fiecărui cromozom și ajustează populația conform necesităților problemei.
        /// În fiecare generație, se efectuează selecție, încrucișare și mutație pentru a genera o nouă populație. Procesul se repetă până la atingerea numărului maxim de generații.
        /// În final, returnează populația curentă, oferind soluții potențiale la problema de optimizare
        /// </summary>
        /// <returns>Array-ul de cromozomi, reprezentând populația finală a algoritmului genetic.</returns>
        public Chromosome[] Solve()
        {
            // Inițializează populația de cromozomi pentru algoritmul genetic
            Chromosome[] population = _breastCancer.PopulateChromosome();

            // Calculează fitness-ul fiecărui cromozom din populație
            for (int i = 0; i < population.Length; ++i)
            {
                _breastCancer.ComputeFitness(population[i], _trainingDataset);
            }

            // Ajustează populația conform anumitor criterii.
            for (int i=0; i < population.Length; i++)
            {
                Adjustment(population[i]);
            }

            // Execută algoritmul genetic pentru un număr definit de generații
            for (int generatie = 0; generatie < Constants.MaxGenerations; ++generatie)
            {
                // Inițializează noua populație
                Chromosome[] newPopulation = new Chromosome[population.Length];

                // Pastrează cel mai bun cromozom prin elitism
                newPopulation[0] = GetBest(population);

                // Generează restul noii populații
                for (int i = 1; i < population.Length; ++i)
                {
                    // Selectează doi părinți
                    Chromosome mother = Selection_Tournament(population);
                    Chromosome father = Selection_Tournament(population);

                    // Aplică încrucișarea
                    Chromosome child = Crossover_Arithmetic(mother, father, Constants.CrossoverRate);

                    // Aplică mutația
                    Mutation_Reset(child, Constants.MutationRate);

                    // Ajustează noul copil
                    Adjustment(child);

                    // Calculează fitness-ul noului cromozom
                    _breastCancer.ComputeFitness(child, _trainingDataset);

                    // Adaugă copilul în noua populație
                    newPopulation[i] = child;
                }

                // Înlocuiește populația veche cu cea nouă
                population = newPopulation;

                // Afisează informații despre starea curentă a algoritmului
                Console.WriteLine("Generatia: " + (generatie + 1));
                Console.WriteLine("Fitnes: " + population[0].Fitness);
                Console.WriteLine("Alfa: " + population[0].Genes[0]);
            }

            // Returnează populația finală.
            return population;
        }

        /// <summary>
        /// Metoda Selection_Tournament implementează selecția turnirului în algoritmul genetic.
        /// Selectează doi indivizi aleatoriu din populație și compară valoarea lor de fitness.
        /// Întoarce individul cu valoarea de fitness mai bună, asigurându-se că indivizii mai adaptați
        /// au o șansă mai mare de a fi selectați pentru încrucișare.
        /// </summary>
        /// <param name="population">Array-ul de cromozomi din care se selectează.</param>
        /// <returns>Cel mai adaptat cromozom dintre cei doi selectați aleatoriu.</returns>
        public Chromosome Selection_Tournament(Chromosome[] population)
        {
            int numberChromosome = population.Length;
            int chromosome1 = _rand.Next(numberChromosome);
            int chromosome2 = _rand.Next(numberChromosome);

            while (chromosome1 == chromosome2)
            {
                chromosome2 = _rand.Next(population.Length);
            }

            return population[chromosome1].Fitness >= population[chromosome2].Fitness ? population[chromosome1] : population[chromosome2];
        }


        /// <summary>
        /// Metoda GetBest returnează cel mai bun cromozom din populația dată, bazat pe valoarea de fitness.
        /// Această funcție identifică cromozomul cu cea mai mare valoare de fitness din populație și
        /// returnează o copie nouă a acestuia. Aceasta este esențială pentru menținerea celor mai bune soluții
        /// în procesul de evoluție al algoritmului genetic.
        /// </summary>
        /// <param name="population">Populația de cromozomi din care se caută cel mai bun.</param>
        /// <returns>O nouă instanță a cromozomului cu cea mai bună valoare de fitness din populație.</returns>
        public Chromosome GetBest(Chromosome[] population)
        {
            var maxim = population.Max(x => x.Fitness); // Identifică valoarea maximă de fitness din populație.
            var best = population.Where(x => x.Fitness == maxim).First(); // Selectează cromozomul cu fitness-ul maxim.
            return new Chromosome(best); // Întoarce o copie a cromozomului selectat.
        }


        /// <summary>
        /// Metoda Crossover_Arithmetic implementează încrucișarea aritmetică între doi cromozomi.
        /// Această tehnică combină genele a doi cromozomi părinți (mama și tata) pentru a genera un nou cromozom (copil),
        /// folosind o rată de încrucișare și un coeficient aleatoriu pentru amestecul genelor.
        /// Încrucișarea contribuie la diversificarea genetică a populației în algoritmul genetic.
        /// </summary>
        /// <param name="mother">Cromozomul mama.</param>
        /// <param name="father">Cromozomul tata.</param>
        /// <param name="rate">Rata de încrucișare, probabilitatea ca încrucișarea să aibă loc.</param>
        /// <returns>Un nou cromozom rezultat din încrucișarea părinților.</returns>
        public Chromosome Crossover_Arithmetic(Chromosome mother, Chromosome father, double rate)
        {
            // Determină aleator dacă încrucișarea va avea loc, bazat pe rata de încrucișare.
            double crossoverChance = _rand.NextDouble();

            if (crossoverChance <= rate) // Verifică dacă încrucișarea trebuie să se întâmple.
            {
                // Coeficient aleatoriu pentru combinarea genelor părinților.
                double a = _rand.NextDouble();

                // Creează un nou cromozom copil, inițial o copie a cromozomului mamă.
                Chromosome child = new Chromosome(mother);

                // Combinați genele părinților în copil folosind coeficientul aleatoriu.
                for (int i = 0; i < mother.Genes.Length; ++i)
                    child.Genes[i] = a * mother.Genes[i] + (1 - a) * father.Genes[i];

                return child;
            }

            // Dacă nu se aplică încrucișarea, returnează unul dintre părinți.
            return _rand.Next(2) == 0 ? new Chromosome(mother) : new Chromosome(father);
        }

        /// <summary>
        /// Implementează mutația prin resetare pentru un cromozom dat. Acest proces modifică genele cromozomului
        /// bazat pe o rată de mutație, resetând valorile genelor la valori aleatoare în intervalul lor valid.
        /// Mutația contribuie la introducerea variațiilor genetice în populație, esențială pentru explorarea spațiului soluțiilor.
        /// </summary>
        /// <param name="child">Cromozomul care va suferi mutația.</param>
        /// <param name="rate">Rata de mutație, probabilitatea ca o genă să fie mutată.</param>
        public void Mutation_Reset(Chromosome child, double rate)
        {
            // Parcurge fiecare genă a cromozomului pentru a decide dacă va suferi mutația.
            for (int i = 0; i < child.Genes.Length; ++i)
            {
                double mutationChance = _rand.NextDouble(); // Generează o probabilitate pentru mutație.
                if (mutationChance <= rate) // Verifică dacă gena curentă va suferi mutația.
                {
                    // Resetează valoarea genei la o valoare aleatoare în intervalul său valid.
                    child.Genes[i] = child.MinValues[i] + _rand.NextDouble() * (child.MaxValues[i] - child.MinValues[i]);
                }
            }
        }

        /// <summary>
        /// Ajustează populația de cromozomi pentru a asigura echilibrul între sumele multiplicatorilor clasificatorilor pentru clasele pozitive și negative.
        /// Acest proces corectează valorile genelor cromozomului pentru a respecta constrângerile problemei, fiind crucial pentru menținerea validității soluțiilor în contextul clasificării.
        /// </summary>
        /// <param name="alpha">Cromozomul care trebuie ajustat.</param>
        public void Adjustment(Chromosome alpha)
        {
            const int MaxIterations = 1000;
            int iterationCount = 0;

            double difference;
            do
            {
                // Calculează sumele pentru clasele pozitive și negative
                double positiveSum = CalculateSum(alpha, 1);
                double negativeSum = CalculateSum(alpha, -1);
                 difference = positiveSum - negativeSum;

                // Selectează un indice de genă pentru ajustare
                int geneIndex = SelectGeneIndex(alpha, positiveSum, negativeSum);

                // Actualizează valoarea genei selectate
                UpdateGeneValue(alpha, geneIndex, Math.Abs(difference));

                // Recalculează sumele după ajustare
                positiveSum = CalculateSum(alpha, 1);
                negativeSum = CalculateSum(alpha, -1);
                difference = positiveSum - negativeSum;

                iterationCount++;

            } while (Math.Abs(difference) > 1e-6 && iterationCount < MaxIterations);
        }

        /// <summary>
        /// Calculează suma valorilor genelor cromozomului care corespund unei anumite clase.
        /// </summary>
        /// <param name="alpha">Cromozomul pentru care se face calculul.</param>
        /// <param name="classLabel">Eticheta clasei pentru care se face suma (1 pentru pozitiv, -1 pentru negativ).</param>
        /// <returns>Suma valorilor genelor pentru clasa specificată.</returns>
        private double CalculateSum(Chromosome alpha, int classLabel)
        {
            return alpha.Genes
                .Where((gene, index) => _trainingDataset.Clasa[index] == classLabel)
                .Sum();
        }

        /// <summary>
        /// Selectează un indice de genă pentru ajustare, bazat pe sumele claselor pozitive și negative.
        /// Alegerea indicelui este influențată de care clasă are suma mai mare, asigurând echilibrul între clase.
        /// </summary>
        /// <param name="alpha">Cromozomul care este analizat.</param>
        /// <param name="positiveSum">Suma claselor pozitive.</param>
        /// <param name="negativeSum">Suma claselor negative.</param>
        /// <returns>Indicele genei selectate pentru ajustare.</returns>
        private int SelectGeneIndex(Chromosome alpha, double positiveSum, double negativeSum)
        {
            var indices = Enumerable.Range(0, _trainingDataset.Clasa.Length)
                .Where(i => _trainingDataset.Clasa[i] == (Math.Abs(positiveSum) > Math.Abs(negativeSum) ? 1 : -1))
                .ToList();

            return indices[_rand.Next(0, indices.Count)];
        }

        /// <summary>
        /// Actualizează valoarea unei gene specifice în cromozom, în scopul echilibrării sumelor claselor.
        /// Dacă valoarea genei este mai mare decât diferența necesară pentru echilibru, o reduce cu acea diferență;
        /// altfel, setează gena la zero.
        /// </summary>
        /// <param name="alpha">Cromozomul care conține gena ce trebuie actualizată.</param>
        /// <param name="geneIndex">Indicele genei care va fi actualizată.</param>
        /// <param name="difference">Diferența necesară pentru a echilibra sumele claselor.</param>
        private void UpdateGeneValue(Chromosome alpha, int geneIndex, double difference)
        {
            if (alpha.Genes[geneIndex] > difference)
            {
                alpha.Genes[geneIndex] -= difference;
            }
            else
            {
                alpha.Genes[geneIndex] = 0;
            }
        }
    }
}
