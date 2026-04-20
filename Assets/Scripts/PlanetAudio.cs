using UnityEngine;


public class PlanetAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _planetAS;
    [SerializeField] private AudioSource _disconnectSignalAS;
    private float _planetsTargetVolume = 0;
    private float _planetsVolumeChangeSpeed = 0.5f;
    private float _disconnectSignalTargetVolume = 0;
    private float _disconnectSignalVolumeChangeSpeed = 0.5f;
    private float _maxVolume = 1;
    
    
    private void Update()
    {
        if (_planetAS.volume < _planetsTargetVolume)
        {
            _planetAS.volume += _planetsVolumeChangeSpeed * Time.deltaTime;
            _planetAS.volume = Mathf.Clamp(_planetAS.volume, 0, _planetsTargetVolume);
        }
        else if (_planetAS.volume > _planetsTargetVolume)
        {
            _planetAS.volume -= _planetsVolumeChangeSpeed * Time.deltaTime;
            _planetAS.volume = Mathf.Clamp(_planetAS.volume, 0, _maxVolume);
        }
        
        if (_disconnectSignalAS.volume < _disconnectSignalTargetVolume)
        {
            _disconnectSignalAS.volume += _disconnectSignalVolumeChangeSpeed * Time.deltaTime;
            _disconnectSignalAS.volume = Mathf.Clamp(_disconnectSignalAS.volume, 0, _disconnectSignalTargetVolume);
        }
        else if (_disconnectSignalAS.volume > _disconnectSignalTargetVolume)
        {
            _disconnectSignalAS.volume -= _disconnectSignalVolumeChangeSpeed * Time.deltaTime;
            _disconnectSignalAS.volume = Mathf.Clamp(_disconnectSignalAS.volume, 0, _maxVolume);
        }
    }
    
    
    public void SetPlanetAudioClip(AudioClip clip)
    {
        _planetAS.clip = clip;
        _planetAS.Play();
    }
    
    
    public void SetVolumes(float planetVol, float disconnectVol)
    {
        _planetsTargetVolume = planetVol;
        _disconnectSignalTargetVolume = disconnectVol;
        
        //_disconnectSignalVolumeChangeSpeed = _disconnectSignalTargetVolume * 1.25f;
        //_planetsVolumeChangeSpeed = _planetsTargetVolume * 1.25f;
    }
}
