using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlanetPuzzleController : MonoBehaviour
{
    public Transform PuzzleParentTransform;
    public Transform PlanetParentTransform;
    public Transform SatelliteParentTransform;
    public List<GameObject> RadioTowers;
    public List<GameObject> Satellites;
    public Rigidbody SatelliteParentRb;
    [SerializeField] private GameObject _placeholderPlanet;
    [SerializeField] private GameObject SatelliteOrbMeshPrefab;
    [SerializeField] private string _planetDataTypeName; 
    private PlanetPuzzleData _myPuzzleData;
    public TextMeshPro SignalCompletionTMP;
    public SpriteRenderer PlanetSelectionSpriteRenderer;
    public float SatelliteOrbXDistanceToRotate = 0f;
    public float SatelliteOrbYDistanceToRotate = 0f;
    public float PuzzleXDistanceToRotate = 0f;
    public float PuzzleYDistanceToRotate = 0f;
    private float _rotationSmoothing = 5f;
    private float _satelliteOrbMeshRadiusMultiplier = 1.4f;
    
    
    public void Start()
    {
        // if (_placeholderPlanet != null)
        // {
        //     Destroy(_placeholderPlanet);   
        // }
        
        _myPuzzleData = Activator.CreateInstance(Type.GetType(_planetDataTypeName)) as PlanetPuzzleData;
        SignalCompletionTMP.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(0.5f + _myPuzzleData.PlanetRadius));
        PlanetSelectionSpriteRenderer.transform.localScale = new Vector3(0.75f * _myPuzzleData.PlanetRadius, 0.75f * _myPuzzleData.PlanetRadius, 0.75f * _myPuzzleData.PlanetRadius);
    }
    
    
    public void SetUpPuzzle()
    {
        GameObject planetInstance = Instantiate(Resources.Load($"Planets/{_myPuzzleData.PlanetPrefabName}")) as GameObject;
        planetInstance.transform.parent = PlanetParentTransform;
        
        GameObject satelliteOrbMeshInstance = Instantiate(SatelliteOrbMeshPrefab);
        satelliteOrbMeshInstance.transform.parent = SatelliteParentTransform;
        float satelliteOrbMeshRadius = _myPuzzleData.PlanetRadius * _satelliteOrbMeshRadiusMultiplier * planetInstance.transform.localScale.x / 100f;
        satelliteOrbMeshInstance.transform.localScale = new Vector3(satelliteOrbMeshRadius, satelliteOrbMeshRadius, satelliteOrbMeshRadius);

        foreach (SphereCoordinate radioTowerCoord in _myPuzzleData.RadioTowerCoordinates)
        {
            GameObject radioTower = Instantiate(Resources.Load("RadioTower")) as GameObject;
            radioTower.transform.parent = planetInstance.transform;
            
            radioTower.transform.position = SphereCoordinate.GetCartesianPositionFromSphereCoordinate(radioTowerCoord, _myPuzzleData.PlanetRadius);
            
            radioTower.transform.LookAt(planetInstance.transform);
            Quaternion currentRotation = radioTower.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(270, 0, 0);
            radioTower.transform.rotation = newRotation;
            RadioTowers.Add(radioTower);
        }
        
        foreach (SphereCoordinate satelliteCoord in _myPuzzleData.SatelliteCoordinates)
        {
            GameObject satellite = Instantiate(Resources.Load("Satellite")) as GameObject;
            satellite.transform.parent = SatelliteParentTransform;
            
            satellite.transform.position = SphereCoordinate.GetCartesianPositionFromSphereCoordinate(satelliteCoord, _myPuzzleData.PlanetRadius * _satelliteOrbMeshRadiusMultiplier);
            
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

        float maxDistance = _myPuzzleData.PlanetRadius * 2f * n; // rough upper bound
        float score = 1f - (totalDistance / maxDistance);
        score = Mathf.Clamp01(score);

        return Mathf.RoundToInt(score * 100f);
    }


    public Vector3 GetCameraPosition()
    {
        return PuzzleParentTransform.transform.position + new Vector3(_myPuzzleData.CameraDistance * 0.25f, 0, -_myPuzzleData.CameraDistance);
    }
    
    
    public void TransitionToPuzzleMode(bool isSelectedPuzzle)
    {
        StartCoroutine(FadeOutMenuPercentageCompletion());
        PlanetSelectionSpriteRenderer.gameObject.SetActive(false);
    }
    
    
    private IEnumerator FadeOutMenuPercentageCompletion()
    {
        float t = 0;
        float totalTime = 1;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            SignalCompletionTMP.color = new Color(1, 1, 1, 1 - t);
            
            yield return null;
        }
    }
}
