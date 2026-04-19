using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetPuzzleSceneController : MonoBehaviour
{
    [SerializeField] private List<PlanetPuzzleController> _planetPuzzleControllers;
    [SerializeField] private LayerMask _planetLayerMask;
    [SerializeField] private AnimationCurve _cameraSlideCurve;
    private PlanetPuzzleController _currentPlanetPuzzleController;
    
    private bool _isRotatingSatelliteOrb = false;
    private bool _isRotatingPuzzle = false;
    private List<Vector3> _previousMousePositions = new List<Vector3>();
    private float _rotationSpeed = 10f;
    
    private List<Vector3> _cameraPositionsMenuStages = new List<Vector3>()
    {
        new Vector3(2.01f, -0.71f, -7.74f)
    };
    private GameThreadStage _currentGameThreadStage;
    
    
    private void Start()
    {
        Camera.main.transform.position = _cameraPositionsMenuStages[0];
        _currentGameThreadStage = GameThreadStage.WaitingForPlanetSelection;
    }
    
    
    private void Update()
    {
        if (_currentGameThreadStage == GameThreadStage.WaitingForPlanetSelection)
        {
            CheckPlanetRaycasts();
        }

        if (_currentGameThreadStage == GameThreadStage.SolvingPuzzle)
        {
            SaveMousePosition();
            SetRotationInteractionFlags();
            
            TryUpdateSatelliteOrbRotationDistance();
            TryUpdatePuzzleRotationDistance();
            
            _currentPlanetPuzzleController.RotateSatelliteOrb();
            _currentPlanetPuzzleController.RotatePuzzle();
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"Current puzzle completion: {_currentPlanetPuzzleController.CalculateCurrentPuzzleCompletionPercentage()}%");
            }
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
        
        _currentPlanetPuzzleController.PuzzleXDistanceToRotate += xDistanceBetweenPreviousMousePositions;
        _currentPlanetPuzzleController.PuzzleYDistanceToRotate += yDistanceBetweenPreviousMousePositions;
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
        
        _currentPlanetPuzzleController.SatelliteOrbXDistanceToRotate += xDistanceBetweenPreviousMousePositions;
        _currentPlanetPuzzleController.SatelliteOrbYDistanceToRotate += yDistanceBetweenPreviousMousePositions;
    }
    
    
    private void CheckPlanetRaycasts()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        PlanetPuzzleController hoveredPlanetPuzzleController = null;

        if (Physics.Raycast(ray, out hit, _planetLayerMask))
        {
            hoveredPlanetPuzzleController = hit.transform.gameObject.GetComponentInParent<PlanetPuzzleController>();
        }
        
        foreach (PlanetPuzzleController planetPuzzleController in _planetPuzzleControllers)
        {
            planetPuzzleController.PlanetSelectionSpriteRenderer.gameObject.SetActive(planetPuzzleController == hoveredPlanetPuzzleController);
        }
        
        if (Input.GetMouseButtonDown(0) && hoveredPlanetPuzzleController != null)
        {
            StartCoroutine(SelectPlanetPuzzle(hoveredPlanetPuzzleController));
        }
    }
    
    
    private IEnumerator SelectPlanetPuzzle(PlanetPuzzleController selectedPlanetPuzzleController)
    {
        _currentGameThreadStage = GameThreadStage.InteractionBlocked;
        _currentPlanetPuzzleController = selectedPlanetPuzzleController;
        
        foreach (PlanetPuzzleController planetPuzzleController in _planetPuzzleControllers)
        {
            planetPuzzleController.TransitionToPuzzleMode(planetPuzzleController == selectedPlanetPuzzleController);
        }
        
        yield return StartCoroutine(SlideCamera(selectedPlanetPuzzleController.GetCameraPosition()));
        
        _currentGameThreadStage = GameThreadStage.SolvingPuzzle;
    }
    
    
    private IEnumerator SlideCamera(Vector3 target)
    {
        float t = 0;
        float totalTime = 1;
        Vector3 startPoint = Camera.main.transform.position;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            float curvePoint = _cameraSlideCurve.Evaluate(t);
            
            Camera.main.transform.position = Vector3.Lerp(startPoint, target, curvePoint);
            
            yield return null;
        }
    }
}


public enum GameThreadStage
{
    InteractionBlocked,
    WaitingForPlanetSelection,
    SolvingPuzzle
}