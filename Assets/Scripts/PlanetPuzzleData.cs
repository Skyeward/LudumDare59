using System.Collections.Generic;


public class PlanetPuzzleData
{
    public string PlanetPrefabName;
    public string PlanetName;
    public string PlanetDesignation;
    public float PlanetRadius;
    public float CameraDistance;
    public List<SphereCoordinate> RadioTowerCoordinates;
    public List<SphereCoordinate> SatelliteCoordinates;
    public int CompletionPercentage;
    public float BestSolutionDistance;
    public float WorstSolutionDistance;
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
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(30, 10),
            new SphereCoordinate(10, 30),
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(10, 10),
            new SphereCoordinate(20, 20)
        };
    }
}


public class TestPlanetPuzzleData2 : PlanetPuzzleData
{
    public TestPlanetPuzzleData2()
    {
        PlanetPrefabName = "Pearlescence2";
        PlanetName = "The Test";
        PlanetDesignation = "A1PD-N";
        PlanetRadius = 1.1f;
        CameraDistance = 4.7f;
        BestSolutionDistance = 1.14f;
        WorstSolutionDistance = 5.45f;
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(30, 10),
            new SphereCoordinate(10, 30),
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(10, 10),
            new SphereCoordinate(20, 20)
        };
    }
}