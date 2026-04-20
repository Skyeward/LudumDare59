using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _solarWindAS;
    [SerializeField] private AudioSource _satellitePingAS;
    [SerializeField] private AudioSource _satelliteObstructedAS;
    [SerializeField] private GameObject _planetAudioPrefab;
    private float _solarWindTargetVolume = 0;
    private float _solarWindMaxVolume = 0.2f;
    private float _solarWindVolumeChangeSpeed = 0.25f;
    private Dictionary<Type, string> _planetClips = new Dictionary<Type, string>()
    {
        {typeof(Planet1Purple), "001"},
        {typeof(Planet2Seaglass), "002"},
        {typeof(Planet3Orange), "003"},
        {typeof(Planet4Red), "004"},
    };
    private Dictionary<Type, float> _planetVolumeMultipliers = new Dictionary<Type, float>()
    {
        {typeof(Planet1Purple), 1f},
        {typeof(Planet2Seaglass), 1f},
        {typeof(Planet3Orange), 1f},
        {typeof(Planet4Red), 0.5f},
    };
    private Dictionary<Type, PlanetAudio> _planetAudioInstances = new Dictionary<Type, PlanetAudio>();
    
    
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
    
    
    public void EnterPuzzle(PlanetPuzzleData puzzleData)
    {
        UpdatePuzzleSolution(puzzleData);
        
        foreach (KeyValuePair<Type, PlanetAudio> kvp in _planetAudioInstances)
        {
            if (kvp.Key == puzzleData.GetType())
            {
                continue;
            }
            
            kvp.Value.SetVolumes(0, 0);
        }
    }
    
    
    public void ExitPuzzle(List<PlanetPuzzleData> puzzleData)
    {
        foreach (PlanetPuzzleData puzzle in puzzleData)
        {
            Type puzzleType = puzzle.GetType();
            
            if (!_planetAudioInstances.ContainsKey(puzzleType))
            {
                continue;
            }
            
            PlanetAudio planetAudio = _planetAudioInstances[puzzleType];
            
            if (puzzle.CompletionPercentage >= puzzle.WinThresholdPercentage)
            {
                float totalVolume = 0.05f;
            
                float disconnectSignalTargetVolume = totalVolume - (puzzle.CompletionPercentage * totalVolume / 100f);
                float planetsTargetVolume = totalVolume - disconnectSignalTargetVolume;
                
                planetAudio.SetVolumes(planetsTargetVolume, disconnectSignalTargetVolume);
            }
            else
            {
                planetAudio.SetVolumes(0, 0);
            }
        }
    }
    
    
    public void PlaySatellitePing()
    {
        _satellitePingAS.pitch = UnityEngine.Random.Range(0.97f, 1.03f);
        _satellitePingAS.Play();
    }
    
    
    public void PlaySatelliteObstruction()
    {
        _satelliteObstructedAS.Play();
    }
    
    
    public void UpdatePuzzleSolution(PlanetPuzzleData puzzleData)
    {
        Type puzzleType = puzzleData.GetType();
        
        if (!_planetAudioInstances.ContainsKey(puzzleType))
        {
            GameObject newAudioInstance = Instantiate(_planetAudioPrefab);
            newAudioInstance.transform.parent = transform;
            _planetAudioInstances[puzzleType] = newAudioInstance.GetComponent<PlanetAudio>();
        }
        
        PlanetAudio planetAudio = _planetAudioInstances[puzzleType];
        planetAudio.SetPlanetAudioClip(Resources.Load($"PlanetAudio/{_planetClips[puzzleData.GetType()]}") as AudioClip);
        
        float totalVolume = 0.4f;
        
        float disconnectSignalTargetVolume = totalVolume - (puzzleData.CompletionPercentage * totalVolume / 100f);
        float planetsTargetVolume = (totalVolume - disconnectSignalTargetVolume) * _planetVolumeMultipliers[puzzleType];
        
        planetAudio.SetVolumes(planetsTargetVolume, disconnectSignalTargetVolume);
    }
}
