using System.Collections.Generic;
using System.Numerics;

public class GameProgress
{
    public List<PlanetPuzzleData> PlanetPuzzles;
    public int OverallCompletionPercentage;

    public GameProgress()
    {
        PlanetPuzzles = new List<PlanetPuzzleData>()
        {
            new TestPlanetPuzzleData()
        };
        OverallCompletionPercentage = 0;
    }


    public void UpdateOverallCompletionPercentage()
    {
        int totalCompletion = 100 * PlanetPuzzles.Count;
        int completionSoFar = 0;

        foreach(PlanetPuzzleData puzzle in PlanetPuzzles)
        {
            completionSoFar += puzzle.CompletionPercentage;
        }

        OverallCompletionPercentage = (int)((float)completionSoFar / totalCompletion * 100);
    }

}