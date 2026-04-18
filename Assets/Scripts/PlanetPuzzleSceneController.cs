using UnityEngine;

public class PlanetPuzzleSceneController : MonoBehaviour
{
    [SerializeField] private PlanetPuzzleController _planetPuzzleController;
    
    
    private void Start()
    {
        _planetPuzzleController.SetUpPuzzle(new TestPlanetPuzzleData());
    }
}
