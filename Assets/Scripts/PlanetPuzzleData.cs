using System.Collections.Generic;


public class PlanetPuzzleData
{
    public string PlanetPrefabName;
    public float PlanetRadius;
    public List<SphereCoordinate> RadioTowerCoordinates;
    public List<SphereCoordinate> SatelliteCoordinates;
    public int CompletionPercentage;
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
        PlanetPrefabName = "Pearlescence2";
        PlanetRadius = 1.1f;
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