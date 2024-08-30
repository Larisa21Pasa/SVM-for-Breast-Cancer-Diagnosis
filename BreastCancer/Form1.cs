/**************************************************************************
 *                                                                        *
 *  Copyright:   (c) 2023-2024, Echipa                                    *
 *  Description: Form1  Class                                             *
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
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BreastCancer
{
    // Clasa Form1 este fereastra principală a interfeței cu utilizatorul în aplicație.
    public partial class Form1 : Form
    {
        // Constructorul pentru Form1 inițializează componentele formularului.
        public Form1()
        {
            InitializeComponent();
        }

        // Handler-ul de evenimente pentru click-ul pe buton care inițiază procesul de optimizare.
        private void button1_Click(object sender, EventArgs e)
        {
            // Inițializarea problemei de optimizare pentru clasificarea cancerului mamar.
            BreastCancer breastCancer = new BreastCancer();
            TrainingDataset trainingDataset = breastCancer.PopulateTrainingDataset();
            TestDataset testDataset = breastCancer.PopulateTestDataset();

            // Inițializarea algoritmului genetic cu problema de optimizare a cancerului mamar.
            GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(breastCancer, trainingDataset);

            // Apelarea metodei solve pentru a obține soluția de la algoritmul genetic.
            Chromosome[] solution = geneticAlgorithm.Solve();

            // Extragerea celui mai bun cromozom (soluție) din algoritmul genetic.
            Chromosome best = geneticAlgorithm.GetBest(solution);

            // Calcularea vectorilor de suport din soluție.
            List<int> supportVectors = breastCancer.SVM(best);

            // Calculul bias-ului din soluție.
            double bias = breastCancer.Bias(best, trainingDataset);

            double [] W = breastCancer.WandM(best, trainingDataset);
            breastCancer.CalculareSumAlfaClasa(best, trainingDataset);

            // Afișarea rezultatelor în caseta de text îmbogățită de pe formular.
            richTextBox1.Text = "";
            richTextBox1.Text += "Cel mai bun fitness: " + best.Fitness + "\n" +
                "Valoare Bias: " + bias + "\n" +
                "Numărul vectorilor de suport: " + supportVectors.Count + "\n";

            // Calculating precision, recall, and specificity
            double precision = CalculatePrecision(best, supportVectors, testDataset, W, bias);
            double recall = CalculateRecall(best, supportVectors, testDataset, W, bias);
            double specificity = CalculateSpecificity(best, supportVectors, testDataset, W, bias);

            richTextBox1.AppendText("\n\nMetrics:\n");
            richTextBox1.AppendText("Precision: " + precision + "\n");
            richTextBox1.AppendText("Recall: " + recall + "\n");
            richTextBox1.AppendText("Specificity: " + specificity + "\n");
            richTextBox1.AppendText("Vectorii de suport sunt: " + "\n");


            // Afisarea matricei de confuzie
            int[,] confusionMatrix = CalculateConfusionMatrix(best, supportVectors, testDataset, W, bias);
            richTextBox1.AppendText("\n\nConfusion Matrix:\n");
            richTextBox1.AppendText($"True Positive: {confusionMatrix[0, 0]}\n");
            richTextBox1.AppendText($"False Positive: {confusionMatrix[0, 1]}\n");
            richTextBox1.AppendText($"False Negative: {confusionMatrix[1, 0]}\n");
            richTextBox1.AppendText($"True Negative: {confusionMatrix[1, 1]}\n");

            // Iterarea prin lista de vectori de suport și afișarea proprietăților lor.
            for (int i = 0; i < supportVectors.Count; i++)
            {
                richTextBox1.Text += "\n" + i + ".\n";
                richTextBox1.Text += "Valoare multiplicator Lagrange: " + best.Genes[supportVectors[i]] + " \n";
                richTextBox1.Text += "Valoare clasă: " + trainingDataset.Clasa[supportVectors[i]] + "\n";
                richTextBox1.Text += "\tClump_thickness: " + trainingDataset.Instances[supportVectors[i]][0] + "\n";
                richTextBox1.Text += "\tUniformity_of_cell_size: " + trainingDataset.Instances[supportVectors[i]][1] + "\n";
                richTextBox1.Text += "\tUniformity_of_cell_shape: " + trainingDataset.Instances[supportVectors[i]][2] + "\n";
                richTextBox1.Text += "\tMarginal_adhesion: " + trainingDataset.Instances[supportVectors[i]][3] + "\n";
                richTextBox1.Text += "\tSingle_epithelial_cell_size: " + trainingDataset.Instances[supportVectors[i]][4] + "\n";
                richTextBox1.Text += "\tBare_nuclei: " + trainingDataset.Instances[supportVectors[i]][5] + "\n";
                richTextBox1.Text += "\tBland_chromatin: " + trainingDataset.Instances[supportVectors[i]][6] + "\n";
                richTextBox1.Text += "\tNormal_nucleoli: " + trainingDataset.Instances[supportVectors[i]][7] + "\n";
                richTextBox1.Text += "\tMitoses: " + trainingDataset.Instances[supportVectors[i]][8] + "\n";

            }

            // Afișarea graficului cu vectorii de suport.
            this.chart.Series.Clear();
            this.chart.ChartAreas.Clear();
            this.chart.ChartAreas.Add(new ChartArea());

            this.chart.ChartAreas[0].AxisX.Title = "Uniformity_of_cell_size";
            this.chart.ChartAreas[0].AxisY.Title = "Uniformity_of_cell_shape";

            // Adăugarea punctelor de date pentru vectorii de suport.
            Series seriesSupportVectorsPlus = new Series();
            seriesSupportVectorsPlus.Name = "Clasa 1";
            Series seriesSupportVectorsMinus = new Series();
            seriesSupportVectorsMinus.Name = "Clasa -1";

            // Setarea culorilor pentru punctele de date.
            for (int i = 0; i < Constants.NumberOfInstances; i++)
            {
                if (trainingDataset.Clasa[i] == 1) // Clasa 1
                {
                    // Setarea culorii pentru punctele de date.
                    seriesSupportVectorsPlus.ChartType = SeriesChartType.Point;
                    seriesSupportVectorsPlus.Color = System.Drawing.Color.Red;
                }
                else
                {
                    // Setarea culorii pentru punctele de date.
                    seriesSupportVectorsMinus.ChartType = SeriesChartType.Point;
                    seriesSupportVectorsMinus.Color = System.Drawing.Color.Blue;
                }
            }

            // Adăugarea punctelor de date pentru vectorii de suport.
            for (int i = 0; i < supportVectors.Count; i++)
            {
                if (trainingDataset.Clasa[supportVectors[i]] == 1)
                    seriesSupportVectorsPlus.Points.AddXY(i + 1, trainingDataset.Instances[supportVectors[i]][2]);
                else
                    seriesSupportVectorsMinus.Points.AddXY(i + 1, trainingDataset.Instances[supportVectors[i]][2]);
            }

            this.chart.Series.Add(seriesSupportVectorsPlus);
            this.chart.Series.Add(seriesSupportVectorsMinus);
        }


        /// <summary>
        /// Calculează și returnează matricea de confuzie pentru setul de date de testare. Matricea de confuzie este o tabelă specifică utilizată 
        /// pentru a înțelege performanța unui model de clasificare. Elementele matricei reprezintă numărul de predicții corecte și incorecte
        /// împărțite în patru categorii: adevărat pozitive, fals pozitive, fals negative și adevărat negative.
        /// </summary>
        /// <param name="best">Cel mai bun cromozom obținut din algoritmul genetic, folosit pentru a face predicții.</param>
        /// <param name="supportVectors">Lista de indici ai vectorilor de suport, folosiți pentru predicția claselor.</param>
        /// <param name="testDataset">Setul de date de testare utilizat pentru evaluarea modelului.</param>
        /// <param name="W">Vectorul de ponderi obținut din modelul SVM.</param>
        /// <param name="bias">Valoarea biasului obținută din modelul SVM.</param>
        /// <returns>Matricea de confuzie sub forma unui array bidimensional 2x2, unde fiecare element reprezintă numărul de instanțe pentru fiecare categorie a matricei de confuzie.</returns>
        private int[,] CalculateConfusionMatrix(Chromosome best, List<int> supportVectors, TestDataset testDataset, double[] W, double bias)
        {

            int[,] confusionMatrix = new int[2, 2];
            for (int i = 0; i < supportVectors.Count; i++)
            {

                int actualIndex = supportVectors[i];
                int predictedClass = (W[actualIndex] + bias > 0) ? 1 : -1;
                int actualClass = testDataset.Clasa[actualIndex];         
     
                if (predictedClass == 1 && actualClass == 1)
                {
                    confusionMatrix[0, 0]++; // True Positive
                }
                else if (predictedClass == 1 && actualClass == -1)
                {
                    confusionMatrix[0, 1]++; // False Positive
                }
                else if (predictedClass == -1 && actualClass == 1)
                {
                    confusionMatrix[1, 0]++; // False Negative
                }
                else if (predictedClass == -1 && actualClass == -1)
                {
                    confusionMatrix[1, 1]++; // True Negative
                }
            }

            return confusionMatrix;

        }

        /// <summary>
        /// Calculează precizia modelului de clasificare. Precizia este măsura care indică proporția predicțiilor corecte pozitive 
        /// (adevărat pozitive) din totalul predicțiilor pozitive (adevărat pozitive și fals pozitive). Această metrică este importantă 
        /// pentru evaluarea performanței modelului, în special când costurile erorilor pozitive false sunt semnificative.
        /// </summary>
        /// <param name="best">Cromozomul cel mai bun obținut din algoritmul genetic, utilizat pentru a face predicții.</param>
        /// <param name="supportVectors">Lista de indici ai vectorilor de suport folosiți pentru a face predicții.</param>
        /// <param name="testDataset">Setul de date de testare utilizat pentru evaluarea modelului.</param>
        /// <param name="W">Vectorul de ponderi obținut din modelul SVM.</param>
        /// <param name="bias">Valoarea biasului obținută din modelul SVM.</param>
        /// <returns>Precizia modelului ca un număr în virgulă mobilă, calculată ca raportul dintre numărul de adevărat pozitive și suma adevărat pozitivelor și fals pozitivelor.</returns>
        private double CalculatePrecision(Chromosome best, List<int> supportVectors, TestDataset testDataset, double[] W, double bias)
        {
            int truePositives = 0; // Contorizează numărul de adevărat pozitive.
            int falsePositives = 0; // Contorizează numărul de fals pozitive.

            // Iterează prin vectorii de suport și clasifică fiecare instanță.
            for (int i = 0; i < supportVectors.Count; i++)
            {
                int actualIndex = supportVectors[i];
                int predictedClass = (W[actualIndex] + bias > 0) ? 1 : -1;
                int actualClass = testDataset.Clasa[actualIndex];

                // Calculează numărul de adevărat pozitive și fals pozitive.
                if (predictedClass == 1 && actualClass == 1)
                {
                    truePositives++;
                }
                else if (predictedClass == 1 && actualClass == -1)
                {
                    falsePositives++;
                }
            }

            // Verifică dacă suma adevărat pozitivelor și fals pozitivelor este zero pentru a evita împărțirea la zero.
            if (truePositives + falsePositives == 0)
            {           
                return 0.0;
            }

            // Calculează și returnează precizia.
            return (double)truePositives / (truePositives + falsePositives);
        }

        /// <summary>
        /// Calculează rata de rechemare (recall) a modelului de clasificare. Rata de rechemare este o metrică ce indică proporția 
        /// predicțiilor corecte pozitive (adevărat pozitive) din totalul cazurilor pozitive reale (adevărat pozitive și fals negative).
        /// Această măsură este importantă pentru evaluarea capacității modelului de a identifica toate cazurile relevante, în special în scenarii unde omiterea cazurilor pozitive este critică.
        /// </summary>
        /// <param name="best">Cromozomul cel mai bun obținut din algoritmul genetic, utilizat pentru a face predicții.</param>
        /// <param name="supportVectors">Lista de indici ai vectorilor de suport folosiți pentru a face predicții.</param>
        /// <param name="testDataset">Setul de date de testare utilizat pentru evaluarea modelului.</param>
        /// <param name="W">Vectorul de ponderi obținut din modelul SVM.</param>
        /// <param name="bias">Valoarea biasului obținută din modelul SVM.</param>
        /// <returns>Rata de rechemare a modelului, calculată ca raportul dintre numărul de adevărat pozitive și suma adevărat pozitivelor și fals negativelor.</returns>
        private double CalculateRecall(Chromosome best, List<int> supportVectors, TestDataset testDataset, double[] W, double bias)
        {
            int truePositives = 0; // Contorizează numărul de adevărat pozitive.
            int falseNegatives = 0; // Contorizează numărul de fals negative.

            // Iterează prin vectorii de suport și clasifică fiecare instanță.
            for (int i = 0; i < supportVectors.Count; i++)  
            {
  
                int actualIndex = supportVectors[i];
                int predictedClass = (W[actualIndex] + bias > 0) ? 1 : -1;
                int actualClass = testDataset.Clasa[actualIndex];

                // Calculează numărul de adevărat pozitive și fals negative.
                if (predictedClass == 1 && actualClass == 1)
                {
                    truePositives++;
                }
                else if (predictedClass == -1 && actualClass == 1)
                {
                    falseNegatives++;
                }
            }

            // Verifică dacă suma adevărat pozitivelor și fals negativelor este zero pentru a evita împărțirea la zero.
            if (truePositives + falseNegatives == 0)
            {
           
                return 0.0;
            }

            // Calculează și returnează rezultatul.
            return (double)truePositives / (truePositives + falseNegatives);
        }

        /// <summary>
        /// Calculează specificitatea modelului de clasificare. Specificitatea este o metrică care indică proporția predicțiilor corecte negative 
        /// (adevărat negative) din totalul cazurilor negative reale (adevărat negative și fals pozitive). Este importantă pentru evaluarea 
        /// capacitatii modelului de a identifica corect instanțele care nu aparțin clasei pozitive, fiind esențială în scenarii unde este 
        /// critică identificarea corectă a cazurilor negative.
        /// </summary>
        /// <param name="best">Cromozomul cel mai bun obținut din algoritmul genetic, utilizat pentru a face predicții.</param>
        /// <param name="supportVectors">Lista de indici ai vectorilor de suport folosiți pentru a face predicții.</param>
        /// <param name="testDataset">Setul de date de testare utilizat pentru evaluarea modelului.</param>
        /// <param name="W">Vectorul de ponderi obținut din modelul SVM.</param>
        /// <param name="bias">Valoarea biasului obținută din modelul SVM.</param>
        /// <returns>Specificitatea modelului, calculată ca raportul dintre numărul de adevărat negative și suma adevărat negativelor și fals pozitivelor.</returns>
        private double CalculateSpecificity(Chromosome best, List<int> supportVectors, TestDataset testDataset, double[] W, double bias)
        {
            int trueNegatives = 0; // Contorizează numărul de adevărat negative.
            int falsePositives = 0; // Contorizează numărul de fals pozitive.

            // Iterează prin vectorii de suport și clasifică fiecare instanță.
            for (int i = 0; i < supportVectors.Count; i++)
            {
         
               int actualIndex = supportVectors[i];
                int predictedClass = (W[actualIndex] + bias > 0) ? 1 : -1;
                int actualClass = testDataset.Clasa[actualIndex];

                // Calculează numărul de adevărat negative și fals pozitive.
                if (predictedClass == -1 && actualClass == -1)
                {
                    trueNegatives++;
                }
                else if (predictedClass == 1 && actualClass == -1)
                {
                    falsePositives++;
                }
            }

            // Verifică dacă suma adevărat negativelor și fals pozitivelor este zero pentru a evita împărțirea la zero.
            if (trueNegatives + falsePositives == 0)
            { 
                return 0.0;
            }

            // Calculează și returnează specificitatea.
            return (double)trueNegatives / (trueNegatives + falsePositives);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
