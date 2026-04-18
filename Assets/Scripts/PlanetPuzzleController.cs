using UnityEngine;
using System.Collections.Generic;


public class PlanetPuzzleController : MonoBehaviour
{
    public Transform PuzzleParentTransform;
    public Transform PlanetParentTransform;
    public Transform SatelliteParentTransform;
    public List<GameObject> RadioTowers;
    public List<GameObject> Satellites;
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
            RadioTowers.Add(radioTower);
        }
        
        foreach (SphereCoordinate satelliteCoord in puzzleData.SatelliteCoordinates)
        {
            // MAKE THE SATELLITES!
            //Satellites.Add(satellite);
        }
    }
    
    
    public void RotatePuzzle()
    {
        float distanceToRotate = PuzzleDistanceToRotate * _rotationSmoothing * Time.deltaTime;
        PuzzleDistanceToRotate = PuzzleDistanceToRotate - distanceToRotate;
        PuzzleParentTransform.Rotate(0, distanceToRotate, 0);
    }


    public int CalculateCurrentPuzzleCompletionPercentage(PlanetPuzzleData puzzleData)
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

        float maxDistance = puzzleData.PlanetRadius * 2f * n; // rough upper bound
        float score = 1f - (totalDistance / maxDistance);
        score = Mathf.Clamp01(score);

        return Mathf.RoundToInt(score * 100f);
    }
}
