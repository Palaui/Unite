
using UnityEngine;

namespace Unite
{
    public class Matrix
    {
        // Variables
        #region Variables

        private int rows;
        private int columns;

        private float[,] values;
        private float[,] lu;

        #endregion

        // Properties
        #region Properties

        public int Rows { get { return rows; } }
        public int Columns { get { return columns; } }
        public float[,] Values { get { return values; } }

        #endregion

        // Override
        #region Override

        public Matrix(int inRows, int inColumns)
        {
            rows = inRows;
            columns = inColumns;

            values = new float[rows, columns];

            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    values[i, j] = 0;
        }

        #endregion

        // Public
        #region Public

        public void FillRow(float[] inRow, int index)
        {
            if (index >= rows)
            {
                Debug.LogError("Matrix Fill Row: Unable to fill row, index passed is bigger than the number of rows, Aborting");
                return;
            }

            if (inRow.Length != columns)
            {
                Debug.LogError("Matrix Fill Row: passed row length is different than the number of columns, Aborting");
                return;
            }

            for (int j = 0; j < columns; j++)
                values[index, j] = inRow[j];
        }

        public void FillColumn(float[] inColumn, int index)
        {
            if (index >= columns)
            {
                Debug.LogError("Matrix Fill Column: Unable to fill column, index passed is bigger than the number of columns, Aborting");
                return;
            }

            if (inColumn.Length != rows)
            {
                Debug.LogError("Matrix Fill Column: passed column length is different than the number of rows, Aborting");
                return;
            }

            for (int i = 0; i < columns; i++)
                values[i, index] = inColumn[i];
        }

        public float Determinant()
        {
            if (!IsSquare())
            {
                Debug.LogError("Matrix Determinant: Unable to process determinant of a non square matrix, Aborting");
                return 0;
            }

            if (lu == null)
                LU();

            float det = 1;
            for (int i = 0; i < columns; i++)
                det *= lu[i, i];

            return det;
        }

        public void LU()
        {
            if (!IsSquare())
            {
                Debug.LogError("Matrix LU: Unable to decompose matrix in LU, the matrix is not square");
                return;
            }

            lu = new float[rows, columns];
            float sum = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = i; j < rows; j++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                        sum += lu[i, k] * lu[k, j];
                    lu[i, j] = values[i, j] - sum;
                }
                for (int j = i + 1; j < rows; j++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                        sum += lu[j, k] * lu[k, i];
                    lu[j, i] = (1 / lu[i, i]) * (values[j, i] - sum);
                }
            }
        }

        public bool IsSquare()
        {
            return (rows == columns);
        }

        public float[] Solve(float[] results)
        {
            if (results.Length != rows)
            {
                Debug.LogError("Matrix Solve: passed results length is different from the matrix order, Aborting");
                return null;
            }

            if (lu == null)
                LU();

            if (!IsSquare())
                return null;

            // Result for Ly = results
            float[] y = new float[rows];
            float sum;
            for (int i = 0; i < rows; i++)
            {
                sum = 0;
                for (int k = 0; k < i; k++)
                    sum += lu[i, k] * y[k];
                y[i] = results[i] - sum;
            }

            // Results for Ux = y
            float[] x = new float[rows];
            for (int i = rows - 1; i >= 0; i--)
            {
                sum = 0;
                for (int k = i + 1; k < rows; k++)
                    sum += lu[i, k] * x[k];
                x[i] = (1 / lu[i, i]) * (y[i] - sum);
            }
            
            return x;
        }

        #endregion
    }
}
