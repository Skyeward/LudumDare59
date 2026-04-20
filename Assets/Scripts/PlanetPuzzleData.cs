using System.Collections.Generic;
using UnityEngine;


public class PlanetPuzzleData
{
    public string PlanetPrefabName;
    public string PlanetName;
    public string PlanetDesignation;
    public float PlanetRadius;
    public float CameraDistance;
    public bool IsRandomizingStartingSatelliteRotation;
    public List<SphereCoordinate> RadioTowerCoordinates;
    public List<SphereCoordinate> SatelliteCoordinates;
    public List<SphereCoordinate> DoubleRadioTowerCoordinates;
    public List<SphereCoordinate> DoubleSatelliteCoordinates;
    public int CompletionPercentage = 0;
    public int WinThresholdPercentage;
    public float BestSolutionDistance;
    public float WorstSolutionDistance;
    public Color MeshColour;


    public void SetWinThresholdPercentage(int percentage)
    {
        WinThresholdPercentage = percentage;
    }

    public int GetWinThresholdPercentage()
    {
        return WinThresholdPercentage;
    }


    public bool Complete()
    {
        return CompletionPercentage >= WinThresholdPercentage;
    }
}


// public class TemplatePlanetPuzzleData : PlanetPuzzleData
// {
//     public TestPlanetPuzzleData()
//     {
//         PlanetPrefabName = "";
//         RadioTowerCoordinates = new List<SphereCoordinate>()
//         {
            
//         };
//         SatelliteCoordinates = new List<SphereCoordinate>()
//         {
            
//         };
//     }
// }


public class TestPlanetPuzzleData : PlanetPuzzleData
{
    public TestPlanetPuzzleData()
    {
        PlanetPrefabName = "PlanetBlueGreen";
        PlanetName = "The Pale Bluish Green Dot";
        PlanetDesignation = "A1GD-M";
        PlanetRadius = 0.98f;
        CameraDistance = 4.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 0.29f;
        WorstSolutionDistance = 2.44f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(90, 70)
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(275, 5)
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>();
        DoubleSatelliteCoordinates = new List<SphereCoordinate>();
        MeshColour = new Color32(0xB7, 0xFF, 0xF7, 0xFF); // #B7FFF7
        SetWinThresholdPercentage(95);
    }
}


public class TestPlanetPuzzleData2 : PlanetPuzzleData
{
    public TestPlanetPuzzleData2()
    {
        PlanetPrefabName = "SeaglassGiant";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 1.1f;
        CameraDistance = 4.7f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 1.14f;
        WorstSolutionDistance = 5.45f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(20, -10),
            new SphereCoordinate(160, 15),
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(-50, -10),
            new SphereCoordinate(-150, 15)
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>();
        DoubleSatelliteCoordinates = new List<SphereCoordinate>();
        MeshColour = new Color32(0xFF, 0xB7, 0xFA, 0xFF); // #FFB7FA
        SetWinThresholdPercentage(90);
    }
}


public class DoubleTowerPuzzleData : PlanetPuzzleData
{
    public DoubleTowerPuzzleData()
    {
        PlanetPrefabName = "Orange";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 0.7f;
        CameraDistance = 4.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 0.79f;
        WorstSolutionDistance = 3.48f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, 30),
            new SphereCoordinate(160, 15),
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(20, -10),
        };
        DoubleSatelliteCoordinates = new List<SphereCoordinate>()
        {
        };
        MeshColour = new Color32(0xFF, 0xF9, 0xAC, 0xFF); // #FFF9AC
        SetWinThresholdPercentage(90);
    }
}


public class DoubleSatellitePuzzleData : PlanetPuzzleData
{
    public DoubleSatellitePuzzleData()
    {
        PlanetPrefabName = "Terracotta";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 1f;
        CameraDistance = 4.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 1.95f;
        WorstSolutionDistance = 4.7f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, 30),
            new SphereCoordinate(100, 15),
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>()
        {
        };
        DoubleSatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(20, -10),
        };
        MeshColour = new Color32(0xFF, 0xCB, 0xAC, 0xFF); // #FFCBAC
        SetWinThresholdPercentage(90);
    }
}