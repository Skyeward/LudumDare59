using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    [Header("Main Rotation")]
    public float yRotationSpeed = 30f;

    private float baseY;
    private float timeOffset;
    public Collider ObstacleDetectionCollider;
    public GameObject SignalSphere;
    [SerializeField] private LineRenderer _guideLine;
    private bool _pulsingSignal = false;
    [SerializeField] private float pulseAmplitude = 0.5f;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] Material _lightMaterial;
    [SerializeField] Material _baseMaterial;
    [SerializeField] TextMeshPro _distanceText;
    [SerializeField] private Renderer signalRenderer;
    [SerializeField] private Color baseColor = Color.yellow;
    [SerializeField] private float emissionStrength = 3f;
    [SerializeField] private LayerMask _obstacleLayer;
    private List<Collider> _triggerList = new List<Collider>();
    
    private string _currentDistanceText = "";

    private Vector3 _baseScale;

    void Start()
    {
        // Random starting Y rotation
        baseY = UnityEngine.Random.Range(0f, 360f);
        yRotationSpeed *= UnityEngine.Random.Range(0.6f, 1.4f);

        // Optional: desync wobble too (prevents identical motion)
        timeOffset = UnityEngine.Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Time.time + timeOffset;

        float y = baseY + t * yRotationSpeed;

        transform.localRotation = Quaternion.Euler(0, y, 0);

        if(_pulsingSignal)
        {

            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;

            // scale
            SignalSphere.transform.localScale = _baseScale * (1f + pulse * pulseAmplitude);

            // glow
            Color emission = baseColor * Mathf.Lerp(0.3f, 4f, pulse);
            signalRenderer.material.SetColor("_EmissionColor", emission);
        }
    }
    
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        
        if ((_obstacleLayer == (_obstacleLayer | (1 << other.gameObject.layer))) && !_triggerList.Contains(other))
        {
            _triggerList.Add(other);
            Debug.Log("adding collider to list");
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (_obstacleLayer == (_obstacleLayer | (1 << other.gameObject.layer)))
        {
            if (_triggerList.Contains(other))
            {
                _triggerList.Remove(other);
            }
        }
    }
    

    public void ShowGuideLine()
    {
        if (_guideLine != null)
            _guideLine.enabled = true;
    }

    public void HideGuideLine()
    {
        if (_guideLine != null)
            _guideLine.enabled = false;
    }


    public void StartPulsingSignal(string distanceText)
    {
        if(!_pulsingSignal)
        {
            _baseScale = SignalSphere.transform.localScale;
            _pulsingSignal = true;
            if(_lightMaterial != null)
            {
                SignalSphere.GetComponent<MeshRenderer>().material = _lightMaterial;
            }

        }

        if(_currentDistanceText == "")
        {
                _currentDistanceText = distanceText;
                _distanceText.text = distanceText;
                _distanceText.gameObject.SetActive(true); 
        }
        else
        {
            _currentDistanceText += " & " + distanceText;
            _distanceText.text = _currentDistanceText;
        }
        
    }


    public void StopPulsingSignal()
    {
        _pulsingSignal = false;
        SignalSphere.transform.localScale = _baseScale;
        SignalSphere.GetComponent<MeshRenderer>().material = _baseMaterial;

        _currentDistanceText = "";
        _distanceText.gameObject.SetActive(false);
    }


    public int GetObstacleCollisionCount()
    {
        return _triggerList.Count;
    }
}