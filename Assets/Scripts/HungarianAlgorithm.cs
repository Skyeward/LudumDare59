using System;

public static class HungarianAlgorithm
{
    public static int[] Solve(float[,] costMatrix)
    {
        int n = costMatrix.GetLength(0);
        int m = costMatrix.GetLength(1);

        if (n != m)
        {
            throw new ArgumentException("Cost matrix must be square.");
        }

        int size = n;

        float[,] cost = new float[size, size];
        Array.Copy(costMatrix, cost, costMatrix.Length);

        float[] u = new float[size + 1];
        float[] v = new float[size + 1];
        int[] p = new int[size + 1];
        int[] way = new int[size + 1];

        for (int i = 1; i <= size; i++)
        {
            p[0] = i;
            float[] minv = new float[size + 1];
            bool[] used = new bool[size + 1];

            for (int j = 1; j <= size; j++)
            {
                minv[j] = float.MaxValue;
                used[j] = false;
            }

            int j0 = 0;

            do
            {
                used[j0] = true;
                int i0 = p[j0];
                float delta = float.MaxValue;
                int j1 = 0;

                for (int j = 1; j <= size; j++)
                {
                    if (used[j]) continue;

                    float cur = cost[i0 - 1, j - 1] - u[i0] - v[j];

                    if (cur < minv[j])
                    {
                        minv[j] = cur;
                        way[j] = j0;
                    }

                    if (minv[j] < delta)
                    {
                        delta = minv[j];
                        j1 = j;
                    }
                }

                for (int j = 0; j <= size; j++)
                {
                    if (used[j])
                    {
                        u[p[j]] += delta;
                        v[j] -= delta;
                    }
                    else
                    {
                        minv[j] -= delta;
                    }
                }

                j0 = j1;

            } while (p[j0] != 0);

            do
            {
                int j1 = way[j0];
                p[j0] = p[j1];
                j0 = j1;
            }
            while (j0 != 0);
        }

        int[] assignment = new int[size];

        for (int j = 1; j <= size; j++)
        {
            if (p[j] != 0)
            {
                assignment[p[j] - 1] = j - 1;
            }
        }

        return assignment;
    }
}