using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;


public class PlanetPuzzleController : MonoBehaviour
{
    public PlanetPuzzleData CurrentPuzzleData;
    public Transform PuzzleParentTransform;
    public Transform PlanetParentTransform;
    public Transform SatelliteParentTransform;
    public List<GameObject> RadioTowers;
    public List<GameObject> Satellites;
    public Rigidbody SatelliteParentRb;
    [SerializeField] private GameObject _placeholderPlanet;
    [SerializeField] private GameObject SatelliteOrbMeshPrefab;
    public float SatelliteOrbXDistanceToRotate = 0f;
    public float SatelliteOrbYDistanceToRotate = 0f;
    public float PuzzleXDistanceToRotate = 0f;
    public float PuzzleYDistanceToRotate = 0f;
    private float _rotationSmoothing = 5f;
    private float _satelliteOrbMeshRadiusMultiplier = 1.4f;
    
    
    public void Start()
    {
        if (_placeholderPlanet != null)
        {
            Destroy(_placeholderPlanet);   
        }
    }
    
    
    public void SetUpPuzzle(PlanetPuzzleData puzzleData)
    {
        CurrentPuzzleData = puzzleData;
        GameObject planetInstance = Instantiate(Resources.Load($"Planets/{puzzleData.PlanetPrefabName}")) as GameObject;
        planetInstance.transform.parent = PlanetParentTransform;
        
        GameObject satelliteOrbMeshInstance = Instantiate(SatelliteOrbMeshPrefab);
        satelliteOrbMeshInstance.transform.parent = SatelliteParentTransform;
        float satelliteOrbMeshRadius = puzzleData.PlanetRadius * _satelliteOrbMeshRadiusMultiplier * planetInstance.transform.localScale.x/100f;
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
            RadioTowers.Add(radioTower);
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
            Satellites.Add(satellite);
        }
    }
    
    
    public void RotateSatelliteOrb()
    {
        float xDistanceToRotate = SatelliteOrbXDistanceToRotate * _rotationSmoothing * Time.deltaTime;
        SatelliteOrbXDistanceToRotate -= xDistanceToRotate;
        
        if (Mathf.Abs(SatelliteOrbXDistanceToRotate) < 0.001f)
        {
            SatelliteOrbXDistanceToRotate = 0;
        }
        
        float yDistanceToRotate = SatelliteOrbYDistanceToRotate * _rotationSmoothing * Time.deltaTime;
        SatelliteOrbYDistanceToRotate -= yDistanceToRotate;
        
        if (Mathf.Abs(SatelliteOrbYDistanceToRotate) < 0.001f)
        {
            SatelliteOrbYDistanceToRotate = 0;
        }
        
        SatelliteParentTransform.RotateAround(SatelliteParentTransform.position, Vector3.up, xDistanceToRotate);
        SatelliteParentTransform.RotateAround(SatelliteParentTransform.position, Vector3.right, yDistanceToRotate);
    }
    
    
    public void RotatePuzzle()
    {
        float xDistanceToRotate = PuzzleXDistanceToRotate * _rotationSmoothing * Time.deltaTime;
        PuzzleXDistanceToRotate -= xDistanceToRotate;
        
        if (Mathf.Abs(PuzzleXDistanceToRotate) < 0.001f)
        {
            PuzzleXDistanceToRotate = 0;
        }
        
        float yDistanceToRotate = PuzzleYDistanceToRotate * _rotationSmoothing * Time.deltaTime;
        PuzzleYDistanceToRotate -= yDistanceToRotate;
        
        if (Mathf.Abs(PuzzleYDistanceToRotate) < 0.001f)
        {
            PuzzleYDistanceToRotate = 0;
        }
        
        PuzzleParentTransform.RotateAround(SatelliteParentTransform.position, Vector3.up, xDistanceToRotate);
        PuzzleParentTransform.RotateAround(SatelliteParentTransform.position, Vector3.right, yDistanceToRotate);
    }


    public int CalculateCurrentPuzzleCompletionPercentage()
    {
        List<Vector3> towerPositions = new List<Vector3>();
        List<Vector3> satellitePositions = new List<Vector3>();

        foreach (GameObject tower in RadioTowers)
        {
            towerPositions.Add(tower.transform.position);
        }

        foreach (GameObject satellite in Satellites)
        {
            satellitePositions.Add(satellite.transform.position);
        }

        int n = towerPositions.Count;
        float[,] costMatrix = new float[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                costMatrix[i, j] = Vector3.Distance(towerPositions[i], satellitePositions[j]);
            }
        }
        
        int[] assignment = HungarianAlgorithm.Solve(costMatrix);

        float totalDistance = 0f;

        for (int i = 0; i < n; i++)
        {
            totalDistance += costMatrix[i, assignment[i]];
        }

        float maxDistance = CurrentPuzzleData.PlanetRadius * 2f * n; // rough upper bound
        float score = 1f - (totalDistance / maxDistance);
        score = Mathf.Clamp01(score);

        return Mathf.RoundToInt(score * 100f);
    }
}
