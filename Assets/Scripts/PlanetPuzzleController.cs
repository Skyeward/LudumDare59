using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlanetPuzzleController : MonoBehaviour
{
    public PlanetPuzzleSceneController MySceneController;
    public Transform PuzzleParentTransform;
    public Transform PlanetParentTransform;
    public Transform SatelliteParentTransform;
    public List<GameObject> RadioTowers;
    public List<GameObject> Satellites;
    [SerializeField] private GameObject SatelliteOrbMeshPrefab;
    [SerializeField] private RectTransform _puzzleCanvasRT;
    [SerializeField] private CanvasGroup _puzzleCanvasCG;
    [SerializeField] private List<TextMeshPro> _fadeablePuzzleTMPs;
    [SerializeField] private GameObject _buttonsParentGO;
    [SerializeField] private SpriteRenderer _satelliteButtonSR;
    [SerializeField] private SpriteRenderer _backButtonSR;
    [SerializeField] private string _planetDataTypeName;
    [SerializeField] private TextMeshPro _planetDesignationTMP;
    [SerializeField] private TextMeshPro _planetNameTMP;
    [SerializeField] private TextMeshPro _puzzleSignalPercentageTMP;
    [SerializeField] private Color _puzzleCompleteColor;
    [SerializeField] private Color _puzzleIncompleteColor;
    public PlanetPuzzleData MyPuzzleData;
    public TextMeshPro SignalCompletionTMP;
    public SpriteRenderer PlanetSelectionSpriteRenderer;
    [SerializeField] private GameObject _connectionPrefab;
    public float SatelliteOrbXDistanceToRotate = 0f;
    public float SatelliteOrbYDistanceToRotate = 0f;
    public float PuzzleXDistanceToRotate = 0f;
    public float PuzzleYDistanceToRotate = 0f;
    private float _rotationSmoothing = 5f;
    private float _satelliteOrbMeshRadiusMultiplier = 1.4f;
    private List<TowerSatellitePair> CurrentAssignment;

    private List<ActiveConnection> _activeConnections = new();
    [SerializeField] private int _segments = 20;
    [SerializeField] private float _arcHeightMultiplier = 0f;
    [SerializeField] private float _fadeSpeed = 1f;
    [SerializeField] private float scrollSpeed = 2f;

    private bool _blockingOffsettingRotation = false;
    private bool _markForReset = false;
    public bool IsShowingSolution = false;
    
    
    public void Start()
    {
        // if (_placeholderPlanet != null)
        // {
        //     Destroy(_placeholderPlanet);   
        // }
        
        MySceneController = FindAnyObjectByType<PlanetPuzzleSceneController>();
        MyPuzzleData = Activator.CreateInstance(Type.GetType(_planetDataTypeName)) as PlanetPuzzleData;
        SignalCompletionTMP.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(0.5f + MyPuzzleData.PlanetRadius));
        PlanetSelectionSpriteRenderer.transform.localScale = new Vector3(0.75f * MyPuzzleData.PlanetRadius, 0.75f * MyPuzzleData.PlanetRadius, 0.75f * MyPuzzleData.PlanetRadius);
        
        float scaleSize = MyPuzzleData.CameraDistance / 5.8f;
        Vector3 scale = new Vector3(scaleSize, scaleSize, scaleSize);
        
        _buttonsParentGO.transform.localPosition = new Vector3(MyPuzzleData.CameraDistance / 1.77f, -MyPuzzleData.CameraDistance / 6f);
        _buttonsParentGO.transform.localScale = scale;
        _puzzleCanvasRT.anchoredPosition = new Vector2(MyPuzzleData.CameraDistance / 1.77f, 0);
        _puzzleCanvasRT.localScale = scale;
        
        _planetNameTMP.SetText(MyPuzzleData.PlanetName);
        _planetDesignationTMP.SetText($"DESIGNATION {MyPuzzleData.PlanetDesignation}");
        
        _puzzleSignalPercentageTMP.color = new Color(_puzzleIncompleteColor.r, _puzzleIncompleteColor.g, _puzzleIncompleteColor.b, 0);
        SignalCompletionTMP.color = _puzzleIncompleteColor;
        
        SetUpPuzzle();
    }


    public bool AnimatingSolution()
    {
        return MySceneController.CurrentGameThreadStage == GameThreadStage.AnimatingSolution;
    }

    void LateUpdate()
    {
        if (_markForReset)
        {
            RemoveCurrentSolutionSafe();
            _markForReset = false;
        }
        UpdateConnections();
    }


    public void HideSolution()
    {
        if (IsShowingSolution && MySceneController.CurrentGameThreadStage == GameThreadStage.SolvingPuzzle)
        {
            ExitSolutionView();
        }
    }


    public void RemoveCurrentSolutionSafe()
    {
        for (int i = 0; i < _activeConnections.Count; i++)
        {
            if (_activeConnections[i].Line != null)
                Destroy(_activeConnections[i].Line.gameObject);
        }

        _activeConnections.Clear();

        foreach (SatelliteController sc in SatelliteParentTransform.GetComponentsInChildren<SatelliteController>())
        {
            sc.ShowGuideLine();
            sc.StopPulsingSignal();
        }

        foreach(RadioTowerController rtc in PlanetParentTransform.GetComponentsInChildren<RadioTowerController>())
        {
            rtc.StopPulsingSignal();
        }
    }
    
    
    public void SetUpPuzzle()
    {
        // GameObject planetInstance = Instantiate(Resources.Load($"Planets/{_myPuzzleData.PlanetPrefabName}")) as GameObject;
        // planetInstance.transform.parent = PlanetParentTransform;

        GameObject satelliteOrbMeshInstance = Instantiate(SatelliteOrbMeshPrefab);
        satelliteOrbMeshInstance.transform.parent = SatelliteParentTransform;
        float satelliteOrbMeshRadius = MyPuzzleData.PlanetRadius * _satelliteOrbMeshRadiusMultiplier;
        satelliteOrbMeshInstance.transform.localPosition = Vector3.zero;
        satelliteOrbMeshInstance.transform.localScale = new Vector3(satelliteOrbMeshRadius, satelliteOrbMeshRadius, satelliteOrbMeshRadius);

        foreach (SphereCoordinate radioTowerCoord in MyPuzzleData.RadioTowerCoordinates)
        {
            GameObject radioTower = Instantiate(Resources.Load("RadioTower")) as GameObject;
            radioTower.transform.parent = PlanetParentTransform;
            
            radioTower.transform.localPosition = SphereCoordinate.GetCartesianPositionFromSphereCoordinate(radioTowerCoord, MyPuzzleData.PlanetRadius);
            
            radioTower.transform.LookAt(PlanetParentTransform);
            Quaternion currentRotation = radioTower.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(270, 0, 0);
            radioTower.transform.rotation = newRotation;
            RadioTowers.Add(radioTower);
        }
        
        foreach (SphereCoordinate satelliteCoord in MyPuzzleData.SatelliteCoordinates)
        {
            GameObject satellite = Instantiate(Resources.Load("Satellite")) as GameObject;
            satellite.transform.parent = SatelliteParentTransform;
            
            satellite.transform.localPosition = SphereCoordinate.GetCartesianPositionFromSphereCoordinate(satelliteCoord, MyPuzzleData.PlanetRadius * _satelliteOrbMeshRadiusMultiplier);
            
            satellite.transform.LookAt(PlanetParentTransform);
            Quaternion currentRotation = satellite.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(270, 0, 0);
            satellite.transform.rotation = newRotation;
            Satellites.Add(satellite);
        }
    }
    
    
    public void RotateSatelliteOrb()
    {
        if(!_blockingOffsettingRotation)
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


    private Coroutine _solutionRoutine;

    public void ShowSolution(int completionPercentage)
    {
        if (MySceneController.CurrentGameThreadStage != GameThreadStage.SolvingPuzzle || IsShowingSolution)
        {
            return;
        }

        if (_solutionRoutine != null)
        {
            StopCoroutine(_solutionRoutine);
        }

        MySceneController.CurrentGameThreadStage = GameThreadStage.AnimatingSolution;
        _solutionRoutine = StartCoroutine(DisplayCurrentAssignment(completionPercentage));
    }


    public IEnumerator DisplayCurrentAssignment(int completionPercentage)
    {
        IsShowingSolution = true;
        
        foreach(SatelliteController sc in SatelliteParentTransform.GetComponentsInChildren<SatelliteController>())
        {
            sc.HideGuideLine();
        }

        _blockingOffsettingRotation = true;

        foreach(TowerSatellitePair pair in CurrentAssignment)
        {
            pair.Tower.GetComponentInChildren<RadioTowerController>().StartPulsingSignal();
            Debug.Log($"Tower at {pair.Tower.transform.position} is paired with Satellite at {pair.Satellite.transform.position} (Distance: {pair.Distance})");
            DrawConnection(pair, MyPuzzleData.PlanetRadius * 2f * CurrentAssignment.Count);
            yield return new WaitForSeconds(1.5f);
            pair.Satellite.GetComponentInChildren<SatelliteController>().StartPulsingSignal($"{(pair.Distance*100f).ToString("F2")}km");
            yield return new WaitForSeconds(0.5f);
        }

        MyPuzzleData.CompletionPercentage = completionPercentage;
        Color percentageColour = completionPercentage >= MyPuzzleData.WinThresholdPercentage ? _puzzleCompleteColor : _puzzleIncompleteColor;
        
        _puzzleSignalPercentageTMP.SetText($"{completionPercentage}% SIGNAL");
        SignalCompletionTMP.SetText($"{completionPercentage}% SIGNAL");
        _puzzleSignalPercentageTMP.color = percentageColour;
        SignalCompletionTMP.color = new Color(percentageColour.r, percentageColour.g, percentageColour.b, 0);
        
        MySceneController.CurrentGameThreadStage = GameThreadStage.SolvingPuzzle;
    }


    public void ExitSolutionView()
    {
        foreach (var c in _activeConnections)
        {
            if (c.Line != null)
                Destroy(c.Line.gameObject);
        }
        _activeConnections.Clear();

        foreach (GameObject tower in RadioTowers)
        {
            tower.GetComponentInChildren<RadioTowerController>()?.StopPulsingSignal();
        }

        foreach (GameObject sat in Satellites)
        {
            sat.GetComponentInChildren<SatelliteController>()?.StopPulsingSignal();
            sat.GetComponentInChildren<SatelliteController>()?.ShowGuideLine(); 
        }

        _blockingOffsettingRotation = false;
        IsShowingSolution = false;
    }


    public void DrawConnection(TowerSatellitePair pair, float maxDistance)
    {
        Debug.Log($"DRAW CONNECTION: {pair.Tower.name} → {pair.Satellite.name}");

        GameObject lineObj = Instantiate(_connectionPrefab);
        lineObj.transform.SetParent(PuzzleParentTransform, true);

        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        var mat = Instantiate(lr.material);
        lr.material = mat;
        lr.useWorldSpace = true;
        lr.textureMode = LineTextureMode.Tile;
        lr.alignment = LineAlignment.View;
        lr.positionCount = 1;
        lr.SetPosition(0, pair.Tower.transform.position);

        float t = pair.Distance / maxDistance;
        Color baseColor = Color.Lerp(Color.yellow, Color.orange, t);

        lr.startColor = baseColor;
        lr.endColor = baseColor;

        var towerAnchor = pair.Tower.GetComponentInChildren<RadioTowerController>().SignalSphere.transform;
        var satelliteAnchor = pair.Satellite.GetComponentInChildren<SatelliteController>().SignalSphere.transform;
        Debug.Log($"TOWER: {towerAnchor.position} | SAT: {satelliteAnchor.position}");

        _activeConnections.Add(new ActiveConnection
        {
            Line = lr,
            Tower = towerAnchor,
            Satellite = satelliteAnchor,
            Center = PuzzleParentTransform,
            Distance = pair.Distance,
            MaxDistance = maxDistance,
            Alpha = 0f,
            Progress = 0f
        });
    }


    private void UpdateConnections()
    {
        foreach (var c in _activeConnections)
        {
            if (c.Line == null || c.Tower == null || c.Satellite == null || c.Center == null)
                continue;

            float offset = (Time.time * scrollSpeed) + (c.Distance * 0.01f);
            c.Line.material.mainTextureOffset = new Vector2(offset, 0f);


            c.Alpha = Mathf.Clamp01(c.Alpha + Time.deltaTime * _fadeSpeed);

            Color baseColor = Color.Lerp(Color.yellow, Color.orange, c.Distance / c.MaxDistance);
            Color finalColor = new Color(baseColor.r, baseColor.g, baseColor.b, c.Alpha);

            c.Line.startColor = finalColor;
            c.Line.endColor = finalColor;


            Vector3[] arc = BuildRadialSphereArc(
                c.Tower.position,
                c.Satellite.position,
                c.Center.position,
                _segments
            );


            float duration = Mathf.Lerp(0.5f, 2.0f, c.Distance / c.MaxDistance);
            c.Progress = Mathf.Clamp01(c.Progress + Time.deltaTime / duration);

            float eased = Mathf.SmoothStep(0f, 1f, c.Progress);

            int lastIndex = Mathf.FloorToInt(_segments * eased);
            lastIndex = Mathf.Clamp(lastIndex, 1, _segments);


            c.Line.positionCount = lastIndex + 1;


            for (int i = 0; i <= lastIndex; i++)
            {
                c.Line.SetPosition(i, arc[i]);
            }
        }
    }


    private Vector3[] BuildRadialSphereArc(
        Vector3 start,
        Vector3 end,
        Vector3 center,
        int segments)
    {
        Vector3[] points = new Vector3[segments + 1];

        Vector3 dirA = (start - center).normalized;
        Vector3 dirB = (end - center).normalized;

        float radiusA = Vector3.Distance(start, center);
        float radiusB = Vector3.Distance(end, center);

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;

            Vector3 dir = Vector3.Slerp(dirA, dirB, t).normalized;

            float radius = Mathf.Lerp(radiusA, radiusB, t);

            points[i] = center + dir * radius;
        }

        return points;
    }


    public int CalculateCurrentPuzzleCompletion()    
    {
        List<Vector3> towerPositions = new List<Vector3>();
        List<Vector3> satellitePositions = new List<Vector3>();

        foreach (GameObject tower in RadioTowers)
        {
            towerPositions.Add(PuzzleParentTransform.InverseTransformPoint(tower.transform.position));
        }

        foreach (GameObject satellite in Satellites)
        {
            satellitePositions.Add(PuzzleParentTransform.InverseTransformPoint(satellite.transform.position));
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

        List<TowerSatellitePair> pairs = new List<TowerSatellitePair>();

        for (int i = 0; i < n; i++)
        {
            pairs.Add(new TowerSatellitePair
            (
                RadioTowers[i],
                Satellites[assignment[i]],
                costMatrix[i, assignment[i]]
            ));
        }

        // Sort shortest → longest
        pairs.Sort((a, b) => a.Distance.CompareTo(b.Distance));
    
        CurrentAssignment = pairs;

        float totalDistance = 0f;

        for (int i = 0; i < n; i++)
        {
            totalDistance += costMatrix[i, assignment[i]];
        }

        // Use precomputed values from your puzzle data
        float best = MyPuzzleData.BestSolutionDistance;
        float worst = MyPuzzleData.WorstSolutionDistance;

        // Avoid divide-by-zero (important for tiny puzzles)
        float range = Mathf.Max(0.0001f, worst - best);

        // Normalize (invert because lower distance = better)
        float score01 = (worst - totalDistance) / range;
        score01 = Mathf.Clamp01(score01);

        return Mathf.RoundToInt(score01 * 100f);
    }


    public Vector3 GetCameraPosition()
    {
        return PuzzleParentTransform.transform.position + new Vector3(MyPuzzleData.CameraDistance * 0.25f, 0, -MyPuzzleData.CameraDistance);
    }
    
    
    public void TransitionToPuzzleMode(bool isSelectedPuzzle)
    {
        StartCoroutine(FadeOutMenuPercentageCompletion());
        PlanetSelectionSpriteRenderer.gameObject.SetActive(false);
        
        if (isSelectedPuzzle)
        {
            _buttonsParentGO.SetActive(true);
            StartCoroutine(FadeInPuzzleElements());
        }
    }
    
    
    public void TransitionToMenuMode(bool isSelectedPuzzle)
    {
        StartCoroutine(FadeInMenuPercentageCompletion());
        
        if (isSelectedPuzzle)
        {
            StartCoroutine(FadeOutPuzzleElements());
        }
    }
    
    
    private IEnumerator FadeOutMenuPercentageCompletion()
    {
        float t = 0;
        float totalTime = 1;
        Color startColor = SignalCompletionTMP.color;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            SignalCompletionTMP.color = new Color(startColor.r, startColor.g, startColor.b, 1 - t);
            
            yield return null;
        }
    }
    
    
    private IEnumerator FadeInMenuPercentageCompletion()
    {
        float t = 0;
        float totalTime = 1;
        Color startColor = SignalCompletionTMP.color;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            SignalCompletionTMP.color = new Color(startColor.r, startColor.g, startColor.b, t);
            
            yield return null;
        }
    }
    
    
    private IEnumerator FadeInPuzzleElements()
    {
        float t = 0;
        float totalTime = 1;
        Color startingPercentageColor = _puzzleSignalPercentageTMP.color;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            Color fadeColor = new Color(1, 1, 1, t);
            Color percentageFadeColor = new Color(startingPercentageColor.r, startingPercentageColor.g, startingPercentageColor.b, t);
            
            foreach (TextMeshPro tmp in _fadeablePuzzleTMPs)
            {
                tmp.color = fadeColor;
            }
            
            _puzzleSignalPercentageTMP.color = percentageFadeColor;
            
            _satelliteButtonSR.color = fadeColor;
            _backButtonSR.color = fadeColor;
            
            yield return null;
        }
    }
    
    
    private IEnumerator FadeOutPuzzleElements()
    {
        float t = 0;
        float totalTime = 1;
        Color startingPercentageColor = _puzzleSignalPercentageTMP.color;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            Color fadeColor = new Color(1, 1, 1, 1 - t);
            Color percentageFadeColor = new Color(startingPercentageColor.r, startingPercentageColor.g, startingPercentageColor.b, 1 - t);
            
            foreach (TextMeshPro tmp in _fadeablePuzzleTMPs)
            {
                tmp.color = fadeColor;
            }
            
            _puzzleSignalPercentageTMP.color = percentageFadeColor;
            
            _satelliteButtonSR.color = fadeColor;
            _backButtonSR.color = fadeColor;
            
            yield return null;
        }
        
        _buttonsParentGO.SetActive(false);
    }
}


public class TowerSatellitePair
{
    public GameObject Tower;
    public GameObject Satellite;
    public float Distance;

    public TowerSatellitePair(GameObject tower, GameObject satellite, float distance)
    {
        Tower = tower;
        Satellite = satellite;
        Distance = distance;
    }
}

public class ActiveConnection
{
    public LineRenderer Line;
    public Transform Tower;
    public Transform Satellite;
    public Transform Center;
    public float Distance;
    public float MaxDistance;
    public float Alpha;
    public float Progress;   
}

enum PuzzleState
{
    Idle,
    ShowingSolution,
    Interactive
}

