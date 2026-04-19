using UnityEngine;


public class PuzzleButton : MonoBehaviour
{
    public PlanetPuzzleController MyController;
    public ButtonType MyButtonType;
}


public enum ButtonType
{
    None,
    Satellite,
    LeavePuzzle
}