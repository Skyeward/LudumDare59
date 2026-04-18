using System.Collections.Generic;
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
        SaveMousePosition();
        SetRotationInteractionFlags();
        
        TryUpdateSatelliteOrbRotationDistance();
        TryUpdatePuzzleRotationDistance();
        
        _planetPuzzleController.RotateSatelliteOrb();
        _planetPuzzleController.RotatePuzzle();
    }
    
    
    private void SaveMousePosition()
    {
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
            _isRotatingSatelliteOrb = false;
        }
        
        if (!Input.GetMouseButton(1))
        {
            _isRotatingPuzzle = false;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isRotatingPuzzle)
            {
                _isRotatingSatelliteOrb = true;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
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
        
        float distanceBetweenPreviousMousePositions = _previousMousePositions[1].x - _previousMousePositions[0].x;
        distanceBetweenPreviousMousePositions *= _rotationSpeed * Time.deltaTime;
        
        _planetPuzzleController.PuzzleDistanceToRotate += distanceBetweenPreviousMousePositions;
    }
    
    
    private void TryUpdateSatelliteOrbRotationDistance()
    {
        if (!_isRotatingSatelliteOrb || _previousMousePositions.Count < 2)
        {
            return;
        }
        
        float xDistanceBetweenPreviousMousePositions = _previousMousePositions[0].x - _previousMousePositions[1].x;
        xDistanceBetweenPreviousMousePositions *= _rotationSpeed * Time.deltaTime;
        
        float yDistanceBetweenPreviousMousePositions = _previousMousePositions[1].y - _previousMousePositions[0].y;
        yDistanceBetweenPreviousMousePositions *= _rotationSpeed * Time.deltaTime;
        
        _planetPuzzleController.SatelliteOrbXDistanceToRotate += xDistanceBetweenPreviousMousePositions;
        _planetPuzzleController.SatelliteOrbYDistanceToRotate += yDistanceBetweenPreviousMousePositions;
    }
}
