using System.Collections.Generic;


public class PlanetPuzzleData
{
    public string PlanetPrefabName;
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
        PlanetPrefabName = "PlanetTest";
        RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(0, 0),
            new SphereCoordinate(0, 2),
            //new SphereCoordinate(0, 45)
        };
        SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(10, 10),
            new SphereCoordinate(20, 20)
        };
    }
}