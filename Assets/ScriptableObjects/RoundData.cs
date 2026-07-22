using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoundScore
{
    public int roundNumber = 1;

    public int scoreNeededToBeat = 30;

    public int maxBallsAllowedToThrow = 3;
}

[CreateAssetMenu(fileName = "NewRoundData", menuName = "Game Data/Round Data")]
public class RoundData : ScriptableObject
{
    [Header("Round Settings")]
    public List<RoundScore> rounds = new List<RoundScore>();

    public RoundScore GetRoundInfo(int targetRoundNumber)
    {
        // Search through the list for a matching round number
        foreach (RoundScore round in rounds)
        {
            if (round.roundNumber == targetRoundNumber)
            {
                return round;
            }
        }

        Debug.LogWarning($"Round {targetRoundNumber} was not found");
        return null;
    }

    public int GetMaxRoundNumber()
    {
        int maxRound = 0;
        
        foreach (RoundScore round in rounds)
        {
            if (round.roundNumber > maxRound)
            {
                maxRound = round.roundNumber;
            }
        }
        
        return maxRound;
    }
}