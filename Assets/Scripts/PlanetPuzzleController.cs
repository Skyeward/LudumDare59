using UnityEngine;


public class PlanetPuzzleController : MonoBehaviour
{
    public Transform PuzzleParentTransform;
    public Transform PlanetParentTransform;
    public Transform SatelliteParentTransform;
    [SerializeField] private GameObject SatelliteOrbMeshPrefab;
    public float SatelliteOrbXDistanceToRotate = 0f;
    public float SatelliteOrbYDistanceToRotate = 0f;
    public float PuzzleDistanceToRotate = 0f;
    private float _rotationSmoothing = 5f;
    private float _satelliteOrbMeshRadiusMultiplier = 1.4f;
    
    
    public void SetUpPuzzle(PlanetPuzzleData puzzleData)
    {
        GameObject planetInstance = Instantiate(Resources.Load($"Planets/{puzzleData.PlanetPrefabName}")) as GameObject;
        planetInstance.transform.parent = PlanetParentTransform;
        
        GameObject satelliteOrbMeshInstance = Instantiate(SatelliteOrbMeshPrefab);
        satelliteOrbMeshInstance.transform.parent = SatelliteParentTransform;
        float satelliteOrbMeshRadius = puzzleData.PlanetRadius * _satelliteOrbMeshRadiusMultiplier * planetInstance.transform.localScale.x;
        satelliteOrbMeshInstance.transform.localScale = new Vector3(satelliteOrbMeshRadius, satelliteOrbMeshRadius, satelliteOrbMeshRadius);

        foreach (SphereCoordinate radioTowerCoord in puzzleData.RadioTowerCoordinates)
        {
            GameObject radioTower = Instantiate(Resources.Load("RadioTower")) as GameObject;
            radioTower.transform.parent = planetInstance.transform;
            
            radioTower.transform.position = SphereCoordinate.GetCartesianPositionFromSphereCoordinate(radioTowerCoord, puzzleData.PlanetRadius);
            
            radioTower.transform.LookAt(planetInstance.transform);
            Quaternion currentRotation = radioTower.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(270, 0, 0);
            radioTower.transform.rotation = newRotation;
        }
        
        foreach (SphereCoordinate satelliteCoord in puzzleData.SatelliteCoordinates)
        {
            GameObject satellite = Instantiate(Resources.Load("Satellite")) as GameObject;
            satellite.transform.parent = SatelliteParentTransform;
            
            satellite.transform.position = SphereCoordinate.GetCartesianPositionFromSphereCoordinate(satelliteCoord, puzzleData.PlanetRadius * _satelliteOrbMeshRadiusMultiplier);
            
            satellite.transform.LookAt(planetInstance.transform);
            Quaternion currentRotation = satellite.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(270, 0, 0);
            satellite.transform.rotation = newRotation;
        }
    }
    
    
    public void RotateSatelliteOrb()
    {
        float xDistanceToRotate = SatelliteOrbXDistanceToRotate * _rotationSmoothing * Time.deltaTime;
        SatelliteOrbXDistanceToRotate -= xDistanceToRotate;
        
        float yDistanceToRotate = SatelliteOrbYDistanceToRotate * _rotationSmoothing * Time.deltaTime;
        SatelliteOrbYDistanceToRotate -= yDistanceToRotate;
        
        SatelliteParentTransform.RotateAround(SatelliteParentTransform.position, Vector3.up, xDistanceToRotate);
        SatelliteParentTransform.RotateAround(SatelliteParentTransform.position, Vector3.right, yDistanceToRotate);
    }
    
    
    public void RotatePuzzle()
    {
        float distanceToRotate = PuzzleDistanceToRotate * _rotationSmoothing * Time.deltaTime;
        PuzzleDistanceToRotate -= distanceToRotate;
        PuzzleParentTransform.Rotate(0, distanceToRotate, 0);
    }
}
