using UnityEngine;


public class PlanetPuzzleController : MonoBehaviour
{
    public Transform PlanetParentTransform;
    public Transform SatelliteParentTransform;
    
    
    public void SetUpPuzzle(PlanetPuzzleData puzzleData)
    {
        GameObject planetInstance = Instantiate(Resources.Load($"Planets/{puzzleData.PlanetPrefabName}")) as GameObject;
        planetInstance.transform.parent = PlanetParentTransform;
        
        float planetRadius = planetInstance.transform.localScale.x;
        
        foreach (SphereCoordinate radioTowerCoord in puzzleData.RadioTowerCoordinates)
        {
            GameObject radioTower = Instantiate(Resources.Load($"RadioTower")) as GameObject;
            radioTower.transform.parent = planetInstance.transform;
            
            //radioTower.transform.position = Vector3.GeoToWorldGlobePosition(); //GeoToWorldGlobePosition(double lat, double lon, float radius);
        }
    }
}
