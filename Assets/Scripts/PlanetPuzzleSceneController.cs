using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;


public class PlanetPuzzleSceneController : MonoBehaviour
{
    [SerializeField] private PlanetPuzzleController _planetPuzzleController;
    
    private bool _isRotatingSatelliteOrb = false;
    private bool _isRotatingPuzzle = false;
    private List<Vector3> _previousMousePositions = new List<Vector3>();
    private float _rotationSpeed = 10f;
    
    
    private void Start()
    {
        _planetPuzzleController.SetUpPuzzle(new TestPlanetPuzzleData());
    }
    
    
    private void Update()
    {

        if(_planetPuzzleController.ShowingSolution())
        {
            return;
        }

        SaveMousePosition();
        SetRotationInteractionFlags();
        
        TryUpdateSatelliteOrbRotationDistance();
        TryUpdatePuzzleRotationDistance();
        
        _planetPuzzleController.RotateSatelliteOrb();
        _planetPuzzleController.RotatePuzzle();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Current puzzle completion: {_planetPuzzleController.CalculateCurrentPuzzleCompletion()}%");
            _planetPuzzleController.ShowSolution();
        }
    }
    
    
    
    private void SaveMousePosition()
    {
        if (
            Input.mousePosition.x < 0
            || Input.mousePosition.y < 0
            || Input.mousePosition.x > Screen.width
            || Input.mousePosition.y > Screen.height
        )
        {
            _previousMousePositions.Clear();
            
            return;
        }
        
        _previousMousePositions.Add(Input.mousePosition);
        
        while (_previousMousePositions.Count > 2)
        {
            _previousMousePositions.RemoveAt(0);
        }
    }
    
    
    private void SetRotationInteractionFlags()
    {
        if (!Input.GetMouseButton(0))
        {
            //Debug.Log("Stopped rotating satellite orb");
            _isRotatingSatelliteOrb = false;
        }
        
        if (!Input.GetMouseButton(1))
        {
            //Debug.Log("Stopped rotating puzzle");
            _isRotatingPuzzle = false;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Started rotating satellite orb");
            if (!_isRotatingPuzzle)
            {
                _planetPuzzleController.HideSolution();
                _isRotatingSatelliteOrb = true;
                
            }
        }
        
        else if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log("Started rotating puzzle");
            if (!_isRotatingSatelliteOrb)
            {
                _isRotatingPuzzle = true;
            }
        }
    }
    
    
    private void TryUpdatePuzzleRotationDistance()
    {
        if (!_isRotatingPuzzle || _previousMousePositions.Count < 2)
        {
            return;
        }
        
        float xDistanceBetweenPreviousMousePositions = _previousMousePositions[0].x - _previousMousePositions[1].x;
        xDistanceBetweenPreviousMousePositions *= _rotationSpeed * 0.01f; //* Time.deltaTime;
        
        float yDistanceBetweenPreviousMousePositions = _previousMousePositions[1].y - _previousMousePositions[0].y;
        yDistanceBetweenPreviousMousePositions *= _rotationSpeed * 0.01f; //* Time.deltaTime;
        
        _planetPuzzleController.PuzzleXDistanceToRotate += xDistanceBetweenPreviousMousePositions;
        _planetPuzzleController.PuzzleYDistanceToRotate += yDistanceBetweenPreviousMousePositions;
    }
    
    
    private void TryUpdateSatelliteOrbRotationDistance()
    {
        if (!_isRotatingSatelliteOrb || _previousMousePositions.Count < 2)
        {
            return;
        }
        
        float xDistanceBetweenPreviousMousePositions = _previousMousePositions[0].x - _previousMousePositions[1].x;
        xDistanceBetweenPreviousMousePositions *= _rotationSpeed * 0.01f; //* Time.deltaTime;
        
        float yDistanceBetweenPreviousMousePositions = _previousMousePositions[1].y - _previousMousePositions[0].y;
        yDistanceBetweenPreviousMousePositions *= _rotationSpeed * 0.01f; //* Time.deltaTime;
        
        _planetPuzzleController.SatelliteOrbXDistanceToRotate += xDistanceBetweenPreviousMousePositions;
        _planetPuzzleController.SatelliteOrbYDistanceToRotate += yDistanceBetweenPreviousMousePositions;
    }
}
