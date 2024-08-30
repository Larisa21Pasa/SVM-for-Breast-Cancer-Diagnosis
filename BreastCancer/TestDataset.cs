using BreastCancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreastCancer
{
    

public class TestDataset
    {
    // Indică apartenența la una din cele două clase ale problemei (1 sau -1).
    public int[] Clasa { get; set; }

    // Reține valorile instanțelor - atributele caracteristice ale datelor.
    public double[][] Instances { get; set; }

    /// <summary>
    /// Constructorul inițializează un set de date de antrenament cu instanțe și clasă date.
    /// </summary>
    /// <param name="clasa">Clasa din setul de date (1 sau -1)</param>
    /// <param name="instances">Vectorul de instanțe</param>
    public TestDataset(int[] clasa, double[][] instances)
    {
        Clasa = new int[Constants.NumberOfInstances];
        Instances = new double[Constants.NumberOfInstances][];

        // Copiază vectorul de clase
        for (int i = 0; i < Constants.NumberOfInstances; ++i)
        {
            Clasa[i] = clasa[i];
        }

        // Copiază matricea de instanțe furnizata.
        for (int i = 0; i < Constants.NumberOfInstances; ++i)
        {
            Instances[i] = new double[Constants.NumberOfInstances];

            for (int j = 0; j < Constants.NumberAtributesInstances; ++j)
            {
                Instances[i][j] = instances[i][j];
            }
        }
    }
}
}
