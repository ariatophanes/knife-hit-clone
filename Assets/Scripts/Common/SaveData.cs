using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int Level
    {
        get => PlayerPrefs.GetInt("Level", 0);
        set => PlayerPrefs.SetInt("Level", value);
    }
    
    public int BestScore
    {
        get => PlayerPrefs.GetInt("Best Score", 0);
        set => PlayerPrefs.SetInt("Best Score", value);
    }
    
    public int Scores
    {
        get => PlayerPrefs.GetInt("Scores", 0);
        set => PlayerPrefs.SetInt("Scores", value);
    }
    
}
