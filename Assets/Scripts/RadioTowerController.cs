using UnityEngine;

public class RadioTowerController : MonoBehaviour
{
    public GameObject SignalSphere;

    private bool _pulsingSignal = false;
    [SerializeField] private float pulseAmplitude = 0.5f;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] Material _lightMaterial;
    [SerializeField] Material _baseMaterial;
    [SerializeField] private Renderer signalRenderer;
    [SerializeField] private Color baseColor = Color.yellow;
    [SerializeField] private float emissionStrength = 3f;

    private Vector3 _baseScale;

    public void SetSignalSphereActive(bool isActive)
    {
        if (SignalSphere != null)
        {
            SignalSphere.SetActive(isActive);
        }
    }


    void Update()
    {
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


    public void StartPulsingSignal()
    {
        _baseScale = SignalSphere.transform.localScale;
        _pulsingSignal = true;
        _baseMaterial = signalRenderer.material;
        if(_lightMaterial != null)
        {
            SignalSphere.GetComponent<MeshRenderer>().material = _lightMaterial;
        }
    }

    public void StopPulsingSignal()
    {
        _pulsingSignal = false;
        SignalSphere.transform.localScale = _baseScale;

        if(_baseMaterial != null)
        {
            SignalSphere.GetComponent<MeshRenderer>().material = _baseMaterial;
        }
    }
}