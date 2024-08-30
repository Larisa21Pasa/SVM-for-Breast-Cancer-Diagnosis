/**************************************************************************
 *                                                                        *
 *  Copyright:   (c) 2023-2024, Echipa                                    *
 *  Description: IOptimizationProblem Interface                           *
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BreastCancer
{
    /// <summary>
    /// Definește o interfață pentru problemele de optimizare, oferind un plan pentru clasele care vor implementa algoritmi de optimizare specifici.
    /// </summary>
    public interface IOptimizationProblem
    {
        /// <summary>
        /// Calculează fitness-ul unui cromozom dat, folosind setul de date de antrenament specificat. 
        /// </summary>
        /// <param name="chromosome">Cromozomul a cărui fitness trebuie calculat.</param>
        /// <param name="trainingDataset">Setul de date de antrenament folosit pentru calculul fitness-ului.</param>
        void ComputeFitness(Chromosome chromosome, TrainingDataset trainingDataset);

        /// <summary>
        /// Creează un nou cromozom cu o structură predefinită adecvată pentru problema specifică de optimizare.
        /// </summary>
        /// <returns>Un cromozom nou creat.</returns>
        Chromosome CreateChromosome();

        /// <summary>
        /// Inițializează o populație de cromozomi pentru algoritmul genetic. Această metodă este responsabilă pentru crearea unei populații inițiale de cromozomi, pregătiți pentru procesul de optimizare.
        /// </summary>
        /// <returns>Un array de cromozomi reprezentând populația inițială.</returns>
        Chromosome[] PopulateChromosome();

        /// <summary>
        /// Populează și returnează un set de date de antrenament pentru utilizarea în procesul de optimizare. 
        /// </summary>
        /// <returns>Un set de date de antrenament.</returns>
        TrainingDataset PopulateTrainingDataset();

        /// <summary>
        /// Calculează și verifică suma produselor dintre genele cromozomului și clasele din setul de date de antrenament. 
        /// </summary>
        /// <param name="chromosome">Cromozomul pentru care se face calculul.</param>
        /// <param name="trainingDataset">Setul de date de antrenament folosit pentru calcul.</param>
        void CalculareSumAlfaClasa(Chromosome chromosome, TrainingDataset trainingDataset);

        /// <summary>
        /// Calculează și returnează valorile necesare pentru determinarea hiperplanului și marginii în contextul unui clasificator SVM. 
        /// </summary>
        /// <param name="chromosome">Cromozomul folosit pentru calculul hiperplanului și marginii.</param>
        /// <param name="trainingDataset">Setul de date de antrenament folosit în calcul.</param>
        /// <returns>Un array de valori reprezentând componentele hiperplanului.</returns>
        double[] WandM(Chromosome chromosome, TrainingDataset trainingDataset);
    }
}