using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PuzzleAnalyzer
{
    [MenuItem("Tools/Analyze Test Puzzle")]
    public static void Analyze()
    {
        var data = new TestPlanetPuzzleData();

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


    static void ExpandPuzzleData(
        PlanetPuzzleData data,
        out Vector3[] towers,
        out Vector3[] satellites)
    {
        List<Vector3> towerList = new List<Vector3>();
        List<Vector3> satelliteList = new List<Vector3>();

        float towerRadius = data.PlanetRadius;
        float satRadius = data.PlanetRadius * 1.5f;

        // --- Single towers ---
        foreach (var coord in data.RadioTowerCoordinates)
        {
            towerList.Add(ToCartesian(coord, towerRadius));
        }

        // --- Double towers (add twice) ---
        foreach (var coord in data.DoubleRadioTowerCoordinates)
        {
            Vector3 pos = ToCartesian(coord, towerRadius);

            towerList.Add(pos);
            towerList.Add(pos);
        }

        // --- Single satellites ---
        foreach (var coord in data.SatelliteCoordinates)
        {
            satelliteList.Add(ToCartesian(coord, satRadius));
        }

        // --- (Future) Double satellites ---
        if (data.DoubleSatelliteCoordinates != null)
        {
            foreach (var coord in data.DoubleSatelliteCoordinates)
            {
                Vector3 pos = ToCartesian(coord, satRadius);

                satelliteList.Add(pos);
                satelliteList.Add(pos);
            }
        }

        // --- Safety check ---
        if (towerList.Count != satelliteList.Count)
        {
            throw new System.Exception(
                $"Mismatch: towers={towerList.Count}, satellites={satelliteList.Count}");
        }

        towers = towerList.ToArray();
        satellites = satelliteList.ToArray();
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
        ExpandPuzzleData(data, out Vector3[] towers, out Vector3[] satellites);

        int n = towers.Length;

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