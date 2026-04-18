using System.Collections.Generic;
using UnityEngine;


public class PlanetPuzzleSceneController : MonoBehaviour
{
    [SerializeField] private PlanetPuzzleController _planetPuzzleController;
    
    private bool _isRotatingSatelliteOrb = false;
    private bool _isRotatingPuzzle = false;
    private List<Vector3> _previousMousePositions = new List<Vector3>();
    
    
    private void Start()
    {
        _planetPuzzleController.SetUpPuzzle(new TestPlanetPuzzleData());
    }
    
    
    private void Update()
    {
        SaveMousePosition();
        SetRotationInteractionFlags();
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
}
