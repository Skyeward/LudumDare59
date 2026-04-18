using UnityEngine;


public class PlanetPuzzleController : MonoBehaviour
{
    public Transform PuzzleParentTransform;
    public Transform PlanetParentTransform;
    public Transform SatelliteParentTransform;
    [SerializeField] private GameObject SatelliteOrbMeshPrefab;
    public float PuzzleDistanceToRotate = 0f;
    private float _rotationSmoothing = 5f;
    private float _satelliteOrbMeshRadiusMultiplier = 1.4f;
    
    
    public void SetUpPuzzle(PlanetPuzzleData puzzleData)
    {
        GameObject planetInstance = Instantiate(Resources.Load($"Planets/{puzzleData.PlanetPrefabName}")) as GameObject;
        planetInstance.transform.parent = PlanetParentTransform;
        
        GameObject satelliteOrbMeshInstance = Instantiate(SatelliteOrbMeshPrefab);
        satelliteOrbMeshInstance.transform.parent = PlanetParentTransform;
        float satelliteOrbMeshScale = puzzleData.PlanetRadius * _satelliteOrbMeshRadiusMultiplier * planetInstance.transform.localScale.x;
        satelliteOrbMeshInstance.transform.localScale = new Vector3(satelliteOrbMeshScale, satelliteOrbMeshScale, satelliteOrbMeshScale);

        foreach (SphereCoordinate radioTowerCoord in puzzleData.RadioTowerCoordinates)
        {
            GameObject radioTower = Instantiate(Resources.Load($"RadioTower")) as GameObject;
            radioTower.transform.parent = planetInstance.transform;
            
            radioTower.transform.position = SphereCoordinate.GetCartesianPositionFromSphereCoordinate(radioTowerCoord, puzzleData.PlanetRadius);
            
            radioTower.transform.LookAt(planetInstance.transform);
            Quaternion currentRotation = radioTower.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(270, 0, 0);
            radioTower.transform.rotation = newRotation;
        }
        
        foreach (SphereCoordinate satelliteCoord in puzzleData.SatelliteCoordinates)
        {
            // MAKE THE SATELLITES!
        }
    }
    
    
    public void RotatePuzzle()
    {
        float distanceToRotate = PuzzleDistanceToRotate * _rotationSmoothing * Time.deltaTime;
        PuzzleDistanceToRotate = PuzzleDistanceToRotate - distanceToRotate;
        PuzzleParentTransform.Rotate(0, distanceToRotate, 0);
    }
}
