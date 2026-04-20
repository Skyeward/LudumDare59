using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _solarWindAS;
    private float _solarWindTargetVolume = 0;
    private float _solarWindMaxVolume = 0.2f;
    private float _solarWindVolumeChangeSpeed = 0.25f;
    private Dictionary<Type, string> _planetClips = new Dictionary<Type, string>()
    {
        {typeof(TestPlanetPuzzleData), "001_E2"},
        {typeof(TestPlanetPuzzleData2), "B2"},
    };
    
    private void Update()
    {
        ChangeSolarWindVolume();
    }
    
    
    private void ChangeSolarWindVolume()
    {
        if (_solarWindAS.volume < _solarWindTargetVolume)
        {
            _solarWindAS.volume += _solarWindVolumeChangeSpeed * Time.deltaTime;
            _solarWindAS.volume = Mathf.Clamp(_solarWindAS.volume, 0, _solarWindMaxVolume);
        }
        else if (_solarWindAS.volume > _solarWindTargetVolume)
        {
            _solarWindAS.volume -= _solarWindVolumeChangeSpeed * Time.deltaTime;
            _solarWindAS.volume = Mathf.Clamp(_solarWindAS.volume, 0, _solarWindMaxVolume);
        }
    }    
    
    
    public void EnterMainMenu()
    {
        _solarWindTargetVolume = _solarWindMaxVolume;
    }
    
    
    public void ExitMainMenu()
    {
        _solarWindTargetVolume = 0f;
    }
    
    
    public void EnterPuzzle()
    {
        
    }
}
