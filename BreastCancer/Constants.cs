/**************************************************************************
 *                                                                        *
 *  Copyright:   (c) 2023-2024, Echipa                                    *
 *  Description: Constants  Class                                         *
 *               (Proiect Inteligenta artificiala)                        *
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/

namespace BreastCancer
{
    /// <summary>
    /// Clasa pentru constante folosite in algoritmul genetic si in SVM 
    /// </summary>
    public static class Constants
    {
        // Numărul de gene dintr-un cromozom
        public static readonly int NumberOfGenes = 20;

        // Numărul de instanțe sau caracteristici ale datelor
        public static readonly int NumberOfInstances = 20;

   
        // Numărul de atribute dintr-o instantanță
        public static readonly int NumberAtributesInstances = 9;

        // Numărul de chromozomi dintr-o populație
        public static readonly int NumberOfIndivids = 50;

        // Numărul de valori lipsă din setul de date; necesită manipulare specială în preprocesare.
        public static readonly int MissingValues = 16;

        // Numărul maxim de iterații (generații) pentru algoritmul genetic până la terminare.
        public static readonly int MaxGenerations = 100;

        // Rata de încrucișare în algoritmul genetic; 90% din populație va trece prin procesul de încrucișare.
        public static readonly double CrossoverRate = 0.9;

        // Rata de mutație în algoritmul genetic; 20% din genele populației vor suferi mutații.
        public static readonly double MutationRate = 0.2;

        // Parametrul de regularizare în SVM; ajută la controlul marjei de eroare și la evitarea supra-ajustării.
        public static readonly double C = 0.1;

        // Parametrul gamma pentru kernel-ul SVM; afectează complexitatea modelului.
        public static readonly double Gamma = 0.001;

        // Valoarea minimă permisă pentru multiplicatorii Lagrange în optimizarea SVM.
        public static readonly double MinimOfAlpha = 0.0;

        // Valoarea maximă permisă pentru multiplicatorii Lagrange în optimizarea SVM; este egală cu parametrul de regularizare C.
        public static readonly double MaximOfAlpha = C;
    }
}
