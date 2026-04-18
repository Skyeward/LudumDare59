using System.Collections.Generic;
using UnityEngine;


public class PlanetPuzzleData
{
    public string PlanetPrefabName;
    public List<SphereCoordinate> RadioTowerCoordinates;
    public List<SphereCoordinate> SatelliteCoordinates;
}


// public class TemplatePlanetPuzzleData : PlanetPuzzleData
// {
//     public TestPlanetPuzzleData()
//     {
//         PlanetPrefabName = "";
//         List<SphereCoordinate> RadioTowerCoordinates = new List<SphereCoordinate>()
//         {
            
//         };
//         List<SphereCoordinate> SatelliteCoordinates = new List<SphereCoordinate>()
//         {
            
//         };
//     }
// }


public class TestPlanetPuzzleData : PlanetPuzzleData
{
    public TestPlanetPuzzleData()
    {
        PlanetPrefabName = "PlanetTest";
        List<SphereCoordinate> RadioTowerCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(170, 10)
        };
        List<SphereCoordinate> SatelliteCoordinates = new List<SphereCoordinate>()
        {
            new SphereCoordinate(10, 10)
        };
    }
}