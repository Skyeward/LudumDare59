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
        TryUpdatePuzzleRotationDistance();
        //try updates satellites
        _planetPuzzleController.RotatePuzzle();
        //rotate satellites
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
        
        float distanceBetweenPreviousMousePositions = Vector3.Distance(_previousMousePositions[1], _previousMousePositions[0]);
        distanceBetweenPreviousMousePositions *= _rotationSpeed * Time.deltaTime;
        
        if (_previousMousePositions[1].x < _previousMousePositions[0].x)
        {
            distanceBetweenPreviousMousePositions *= -1;
        }
        
        _planetPuzzleController.PuzzleDistanceToRotate += distanceBetweenPreviousMousePositions;
    }


}
