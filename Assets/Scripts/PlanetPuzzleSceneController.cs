using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class PlanetPuzzleSceneController : MonoBehaviour
{
    [SerializeField] private List<PlanetPuzzleController> _planetPuzzleControllers;
    [SerializeField] private LayerMask _planetLayerMask;
    [SerializeField] private LayerMask _puzzleButtonLayerMask;
    [SerializeField] private AnimationCurve _cameraSlideCurve;
    [SerializeField] private CanvasGroup _overallCG;
    [SerializeField] private CanvasGroup _logoCG;
    [SerializeField] private CanvasGroup _pressAnyToExitCG;
    [SerializeField] private CanvasGroup _fadeInCG;
    [SerializeField] private AudioSource _dogToboggan;
    public TextMeshProUGUI OverallTMP;
    public AudioManager MyAudioManager;
    private PlanetPuzzleController _currentPlanetPuzzleController;
    
    private bool _isRotatingSatelliteOrb = false;
    private bool _isRotatingPuzzle = false;
    private List<Vector3> _previousMousePositions = new List<Vector3>();
    private float _rotationSpeed = 10f;
    
    private List<Vector3> _cameraPositionsMenuStages = new List<Vector3>()
    {
        new Vector3(2.45f, -0.88f, -5.26f),
        new Vector3(2.45f, -1.9f, -15.19f),
        new Vector3(5.2f, -2.44f, -22.57f),
        new Vector3(4f, -3.5f, -30f),
    };

    public GameThreadStage CurrentGameThreadStage;
    private GameProgress _gameProgress;
    private int previousMenuCameraIndex = 0;
    
    
    private void Start()
    {
        Camera.main.transform.position = _cameraPositionsMenuStages[0];
        CurrentGameThreadStage = GameThreadStage.InteractionBlocked;
        _gameProgress = new GameProgress();
        
        MyAudioManager.EnterMainMenu();
        
        StartCoroutine(FadeInMenu());
    }
    
    
    private IEnumerator FadeInMenu()
    {
        if (_fadeInCG != null)
        {
            float t = 0;
            float totalTime = 1;
            
            while (t < 1)
            {
                t += Time.deltaTime / totalTime;
                
                _fadeInCG.alpha = 1 - t;
                
                yield return null;
            }
            
            _fadeInCG.gameObject.SetActive(false);
        }
        
        CurrentGameThreadStage = GameThreadStage.WaitingForPlanetSelection;
    }
    
    
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log("Completinging puzzle for testing purposes");
        //     foreach (PlanetPuzzleController controller in _planetPuzzleControllers)
        //     {
        //         controller.MyPuzzleData.CompletionPercentage = 100;
        //     }
        // }

        if (CurrentGameThreadStage == GameThreadStage.WaitingForPlanetSelection)
        {
            CheckPlanetRaycasts();
        }

        if (CurrentGameThreadStage == GameThreadStage.SolvingPuzzle || CurrentGameThreadStage == GameThreadStage.AnimatingSolution)
        {
            PuzzleButton button = CheckButtonRaycasts();
            
            if (button != null)
            {
                _previousMousePositions.Clear();
                
                if (Input.GetMouseButtonDown(0) && CurrentGameThreadStage == GameThreadStage.SolvingPuzzle)
                {
                    if (button.MyButtonType == ButtonType.LeavePuzzle)
                    {
                        MyAudioManager.EnterMainMenu();
                        MyAudioManager.ExitPuzzle(_planetPuzzleControllers.Select(puzzCon => puzzCon.MyPuzzleData).ToList());
                        StartCoroutine(LeavePuzzle(button.MyController));
                        StartCoroutine(FadeInOverall());
                    }
                    else if (button.MyButtonType == ButtonType.Satellite)
                    {
                        if (AreSatellitesClearOfObstacles(_currentPlanetPuzzleController))
                        {
                            int puzzleCompletion = _currentPlanetPuzzleController.CalculateCurrentPuzzleCompletion();
                            Debug.Log($"Current puzzle completion: {puzzleCompletion}%");
                            _currentPlanetPuzzleController.ShowSolution(puzzleCompletion);
                            _gameProgress.PlanetPuzzles = _planetPuzzleControllers.Select(controller => controller.MyPuzzleData).ToList();
                        }
                        else
                        {
                            MyAudioManager.PlaySatelliteObstruction();
                        }
                    }
                }
            }
            else
            {
                SetRotationInteractionFlags();
            }
            
            SaveMousePosition();
            
            TryUpdateSatelliteOrbRotationDistance();
            TryUpdatePuzzleRotationDistance();
            
            if (_currentPlanetPuzzleController != null) //null for one frame when leaving puzzle
            {
                _currentPlanetPuzzleController.RotateSatelliteOrb();
                _currentPlanetPuzzleController.RotatePuzzle();
            }


        }
    }
    
    
    private bool AreSatellitesClearOfObstacles(PlanetPuzzleController puzzleController)
    {
        foreach (SatelliteController sc in puzzleController.SatelliteParentTransform.GetComponentsInChildren<SatelliteController>())
        {
            if (sc.GetObstacleCollisionCount() > 0)
            {
                return false;
            }
        }
        
        return true;
    }
    

    public void UpdateOverallCompletionPercentage()
    {
        _gameProgress.UpdateOverallCompletionPercentage();
        Debug.Log($"Overall completion: {_gameProgress.OverallCompletionPercentage}%");
        
        OverallTMP.SetText($"Overall: {_gameProgress.OverallCompletionPercentage}%   <i><size=28>(GOAL: 90%)");
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
            if (!_isRotatingPuzzle && CurrentGameThreadStage != GameThreadStage.AnimatingSolution)
            {
                _currentPlanetPuzzleController.HideSolution();
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _planetLayerMask))
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
            MyAudioManager.ExitMainMenu();
            MyAudioManager.EnterPuzzle(hoveredPlanetPuzzleController.MyPuzzleData);
            
            StartCoroutine(FadeOutOverall());
        }
    }
    
    
    private PuzzleButton CheckButtonRaycasts()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //PlanetPuzzleController hoveredPlanetPuzzleController = null;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _puzzleButtonLayerMask))
        {
            //hoveredPlanetPuzzleController = hit.transform.gameObject.GetComponentInParent<PlanetPuzzleController>();
            
            return hit.transform.gameObject.GetComponent<PuzzleButton>();
        }
        
        return null;
    }
    
    
    private IEnumerator SelectPlanetPuzzle(PlanetPuzzleController selectedPlanetPuzzleController)
    {
        CurrentGameThreadStage = GameThreadStage.InteractionBlocked;
        _currentPlanetPuzzleController = selectedPlanetPuzzleController;
        
        foreach (PlanetPuzzleController planetPuzzleController in _planetPuzzleControllers)
        {
            planetPuzzleController.TransitionToPuzzleMode(planetPuzzleController == selectedPlanetPuzzleController);
        }
        
        yield return StartCoroutine(SlideCamera(selectedPlanetPuzzleController.GetCameraPosition()));
        
        CurrentGameThreadStage = GameThreadStage.SolvingPuzzle;
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
    
    
    private IEnumerator LeavePuzzle(PlanetPuzzleController currentPuzzleController)
    {
        CurrentGameThreadStage = GameThreadStage.InteractionBlocked;
        _currentPlanetPuzzleController = null;
        
        foreach (PlanetPuzzleController planetPuzzleController in _planetPuzzleControllers)
        {
            planetPuzzleController.TransitionToMenuMode(planetPuzzleController == currentPuzzleController);
        }

        if(previousMenuCameraIndex != GetMenuCameraIndex())
        {
            yield return StartCoroutine(SlideCamera(_cameraPositionsMenuStages[previousMenuCameraIndex]));
            previousMenuCameraIndex = GetMenuCameraIndex();
        }
        
        // slide from old to new position here?? 
        yield return StartCoroutine(SlideCamera(_cameraPositionsMenuStages[GetMenuCameraIndex()]));
        
        bool allComplete = true;
        
        foreach (PlanetPuzzleController con in _planetPuzzleControllers)
        {
            if (con.MyPuzzleData.CompletionPercentage < con.MyPuzzleData.WinThresholdPercentage)
            {
                allComplete = false;
                
                break;
            }
        }
        
        if (allComplete && _gameProgress.OverallCompletionPercentage >= 90)
        {
            StartCoroutine(ShowWin());
        }
        else
        {
            CurrentGameThreadStage = GameThreadStage.WaitingForPlanetSelection;
        }
    }


    private int GetNumberOfSolvedPuzzles()
    {
        int solvedCount = 0;

        foreach (PlanetPuzzleController controller in _planetPuzzleControllers)
        {
            if (controller.MyPuzzleData.CompletionPercentage >= controller.MyPuzzleData.GetWinThresholdPercentage())
            {
                Debug.Log($"Solved puzzle: {controller.MyPuzzleData.PlanetName} ({controller.MyPuzzleData.CompletionPercentage}% of {controller.MyPuzzleData.GetWinThresholdPercentage()}% threshold)");
                solvedCount++;
            }
        }

        Debug.Log($"Solved count = {solvedCount}");
        return solvedCount;
    }

    private int GetMenuCameraIndex()
    {
        int solvedPuzzles = GetNumberOfSolvedPuzzles();

        if (solvedPuzzles == 0)
        {
            return Math.Max(0, previousMenuCameraIndex);
        }
        else if (solvedPuzzles < 3)
        {
            return Math.Max(1, previousMenuCameraIndex);
        }
        else if (solvedPuzzles < 7)
        {
            return Math.Max(2, previousMenuCameraIndex);
        }
        else
        {
            return Math.Max(3, previousMenuCameraIndex);
        }
    }
    
    
    private IEnumerator FadeOutOverall()
    {
        float t = 0;
        float totalTime = 1;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            
            _overallCG.alpha = 1 - t;
            
            yield return null;
        }
    }
    
    
    private IEnumerator FadeInOverall()
    {
        float t = 0;
        float totalTime = 1;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            
            _overallCG.alpha = t;
            
            yield return null;
        }
    }


    private IEnumerator ShowWin()
    {
        CurrentGameThreadStage = GameThreadStage.InteractionBlocked;
        
        yield return new WaitForSeconds(1f);
        
        StartCoroutine(FadeOutOverall());
        
        yield return StartCoroutine(FadeInLogo());
        
        yield return new WaitForSeconds(2f);
        
        _dogToboggan.Play();
        
        yield return new WaitForSeconds(6f);
        
        yield return StartCoroutine(FadeInPressAnyToExit());
        
        while (!Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
        {
            yield return null;
        }
        
        Debug.Log("Quitting...");
        
        Application.Quit();
    }
    
    
    private IEnumerator FadeInLogo()
    {
        float t = 0;
        float totalTime = 1;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            
            _logoCG.alpha = t;
            
            yield return null;
        }
    }
    
    
    private IEnumerator FadeInPressAnyToExit()
    {
        float t = 0;
        float totalTime = 1;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            
            _pressAnyToExitCG.alpha = t;
            
            yield return null;
        }
    }
}

public enum GameThreadStage
{
    InteractionBlocked,
    WaitingForPlanetSelection,
    SolvingPuzzle, 
    AnimatingSolution,
}