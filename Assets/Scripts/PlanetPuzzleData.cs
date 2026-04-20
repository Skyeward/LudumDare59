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


public class Planet1Purple : PlanetPuzzleData
{
    public Planet1Purple()
    {
        PlanetPrefabName = "PurpleMoon";
        PlanetName = "The Pale Bluish Green Dot";
        PlanetDesignation = "A1GD-M";
        PlanetRadius = 0.98f;
        CameraDistance = 4.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 0.55f;
        WorstSolutionDistance = 2.5f;
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
        MeshColour = new Color32(0x96, 0x7C, 0xC9, 0xFF); // #967CC9
        SetWinThresholdPercentage(95);
    }
}


public class Planet2Seaglass : PlanetPuzzleData
{
    public Planet2Seaglass()
    {
        PlanetPrefabName = "SeaglassGiant";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 1.17f;
        CameraDistance = 5.65f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 1.35f;
        WorstSolutionDistance = 5.45f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(160, -30),
            new SphereCoordinate(160, 30),
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, 0),
            new SphereCoordinate(140, 0)
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>();
        DoubleSatelliteCoordinates = new List<SphereCoordinate>();
        MeshColour = new Color32(0x84, 0xBF, 0xCE, 0xFF); // #84BFCE
        SetWinThresholdPercentage(90);
    }
}


public class Planet3Orange : PlanetPuzzleData
{
    public Planet3Orange()
    {
        PlanetPrefabName = "FireBall";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 0.5f;
        CameraDistance = 3.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 1.98f;
        WorstSolutionDistance = 3.69f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(30, -10),
            new SphereCoordinate(50, -30),
            new SphereCoordinate(40, 180),
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, 0),
            new SphereCoordinate(0, 0),
            new SphereCoordinate(150, -30),

        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>()
        {

        };
        DoubleSatelliteCoordinates = new List<SphereCoordinate>()
        {
        };
        MeshColour = new Color32(0xFF, 0xF9, 0xAC, 0xFF); // #FFF9AC
        SetWinThresholdPercentage(90);
    }
}


public class Planet4Red : PlanetPuzzleData
{
    public Planet4Red()
    {
        PlanetPrefabName = "Terracotta";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 0.5f;
        CameraDistance = 3.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 1.51f;
        WorstSolutionDistance = 2.91f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, -90),
            new SphereCoordinate(100, -145),
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
        MeshColour = new Color32(0xFF, 0xAC, 0xAC, 0xFF); // #FFACAC
        SetWinThresholdPercentage(90);
    }
}


public class Planet5RedBlue : PlanetPuzzleData
{
    public Planet5RedBlue()
    {
        PlanetPrefabName = "FireBall";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 1.5f;
        CameraDistance = 5.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 1.9f;
        WorstSolutionDistance = 8.04f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(-160, 20),
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, 30),
            new SphereCoordinate(160, 15),
            new SphereCoordinate(20, 25),
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(20, -10),
        };
        DoubleSatelliteCoordinates = new List<SphereCoordinate>()
        {
        };
        MeshColour = new Color32(0xFF, 0xAC, 0xAC, 0xFF); // #FFACAC
        SetWinThresholdPercentage(90);
    }
}


public class Planet6Turquoise : PlanetPuzzleData
{
    public Planet6Turquoise()
    {
        PlanetPrefabName = "FireBall";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 0.7f;
        CameraDistance = 4.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 3.17f;
        WorstSolutionDistance = 5.99f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(-90, 30),
            new SphereCoordinate(-20, 90),
            new SphereCoordinate(150, 30),
            new SphereCoordinate(100, -40),
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, -50),
            new SphereCoordinate(20, -10),
        };
        DoubleSatelliteCoordinates = new List<SphereCoordinate>()
        {
        };
        MeshColour = new Color32(0xAC, 0xFF, 0xDF, 0xFF); // #ACFFDF
        SetWinThresholdPercentage(90);
    }
}


public class Planet7Blue : PlanetPuzzleData
{
    public Planet7Blue()
        {
            PlanetPrefabName = "FireBall";
            PlanetName = "The Test";
            PlanetDesignation = "A1PD-N";
            PlanetRadius = 0.6f;
            CameraDistance = 3.5f;
            IsRandomizingStartingSatelliteRotation = false;
            BestSolutionDistance = 4.33f;
            WorstSolutionDistance = 8.97f;
            RadioTowerCoordinates = new List<SphereCoordinate>()
            {
                new SphereCoordinate(0, 30),
                new SphereCoordinate(0, 15),
                new SphereCoordinate(180, 10),
            };
            SatelliteCoordinates = new List<SphereCoordinate>()
            {
                new SphereCoordinate(20, 100),
                new SphereCoordinate(50, 90),
                new SphereCoordinate(35, 70),
            };
            DoubleRadioTowerCoordinates = new List<SphereCoordinate>()
            {
                new SphereCoordinate(190, 25),
            };
            DoubleSatelliteCoordinates = new List<SphereCoordinate>()
            {
                new SphereCoordinate(30, -70),
            };
            MeshColour = new Color32(0x92, 0xC2, 0xF9, 0xFF); // #92C2F9
            SetWinThresholdPercentage(90);
        }
}


public class Planet8Plasma : PlanetPuzzleData
{
    public Planet8Plasma()
    {
        PlanetPrefabName = "FireBall";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 0.2f;
        CameraDistance = 2.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 2.8f;
        WorstSolutionDistance = 8.46f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(100, 60),
            new SphereCoordinate(120, -60),
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, 30),
            new SphereCoordinate(160, 15),
            new SphereCoordinate(50, 30),
            new SphereCoordinate(110, 15),
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(20, -10),
        };
        DoubleSatelliteCoordinates = new List<SphereCoordinate>()
        {
        };
        MeshColour = new Color32(0xF4, 0xAC, 0xFF, 0xFF); // #F4ACFF
        SetWinThresholdPercentage(90);
    }
}


public class Planet9YellowRinged : PlanetPuzzleData
{
    public Planet9YellowRinged()
    {
        PlanetPrefabName = "FireBall";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 0.45f;
        CameraDistance = 4.5f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 1.83f;
        WorstSolutionDistance = 3.59f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(120, 30),
            new SphereCoordinate(80, 60),
            new SphereCoordinate(0, 40),
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, 30),
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>()
        {
        };
        DoubleSatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(20, -10),
        };
        MeshColour = new Color32(0xFE, 0xEF, 0x98, 0xFF); // #FEEF98
        SetWinThresholdPercentage(90);
    }
}


public class Planet10GreenRinged : PlanetPuzzleData
{
    public Planet10GreenRinged()
    {
        PlanetPrefabName = "FireBall";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 0.3f;
        CameraDistance = 3f;
        IsRandomizingStartingSatelliteRotation = false;
        BestSolutionDistance = 2.26f;
        WorstSolutionDistance = 3.5f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(20, 10),
            new SphereCoordinate(-30, 40),
            new SphereCoordinate(40, 20),
            new SphereCoordinate(10, 190),

        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(180, 30),
            new SphereCoordinate(160, 15),
            new SphereCoordinate(140, 60),
            new SphereCoordinate(-15, 20),
        };
        DoubleRadioTowerCoordinates = new List<SphereCoordinate>()
        {
        };
        DoubleSatelliteCoordinates = new List<SphereCoordinate>()
        {
        };
        MeshColour = new Color32(0xC2, 0xFF, 0xAC, 0xFF); // #C2FFAC
        SetWinThresholdPercentage(85);
    }
}