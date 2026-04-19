using UnityEngine;
using UnityEditor;

public class PuzzleAnalyzer
{
    [MenuItem("Tools/Analyze Test Puzzle")]
    public static void Analyze()
    {
        var data = new TestPlanetPuzzleData2();

        ComputeBounds(data, 100000, out float best, out float worst);

        Debug.Log($"Best: {best}");
        Debug.Log($"Worst: {worst}");
    }

    public static Vector3 ToCartesian(SphereCoordinate coord, float radius)
    {
        float lat = coord.Latitude * Mathf.Deg2Rad;
        float lon = coord.Longitude * Mathf.Deg2Rad;

        float x = radius * Mathf.Cos(lat) * Mathf.Cos(lon);
        float y = radius * Mathf.Sin(lat);
        float z = radius * Mathf.Cos(lat) * Mathf.Sin(lon);

        return new Vector3(x, y, z);
    }


    static float[,] BuildCostMatrix(Vector3[] towers, Vector3[] satellites)
    {
        int n = towers.Length;
        float[,] cost = new float[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                cost[i, j] = Vector3.Distance(towers[i], satellites[j]);
            }
        }

        return cost;
    }


    static float Evaluate(Vector3[] towers, Vector3[] satellites)
    {
        var cost = BuildCostMatrix(towers, satellites);
        var assignment = HungarianAlgorithm.Solve(cost);

        float total = 0f;

        for (int i = 0; i < assignment.Length; i++)
        {
            total += cost[i, assignment[i]];
        }

        return total;
    }


    public static void ComputeBounds(
        PlanetPuzzleData data,
        int samples,
        out float best,
        out float worst)
    {
        int n = data.RadioTowerCoordinates.Count;

        Vector3[] towers = new Vector3[n];
        Vector3[] satellites = new Vector3[n];

        for (int i = 0; i < n; i++)
        {
            towers[i] = ToCartesian(data.RadioTowerCoordinates[i], data.PlanetRadius);
            satellites[i] = ToCartesian(data.SatelliteCoordinates[i], data.PlanetRadius * 1.5f); // outer sphere
        }

        best = float.MaxValue;
        worst = float.MinValue;

        for (int i = 0; i < samples; i++)
        {
            Quaternion rot = Random.rotation;

            Vector3[] rotatedSatellites = new Vector3[n];

            for (int j = 0; j < n; j++)
            {
                rotatedSatellites[j] = rot * satellites[j];
            }

            float score = Evaluate(towers, rotatedSatellites);

            if (score < best) best = score;
            if (score > worst) worst = score;
        }
    }



}