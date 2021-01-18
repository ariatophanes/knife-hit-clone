using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameConfig gameConfig;
    
    private IStateMachine stateMachine;

    private SaveData saveData;
    
    void Start()
    {
        Application.targetFrameRate = 60;
        
        saveData = new SaveData();
        saveData.Scores = 0;
        
        stateMachine = new StateMachine(new MenuState(gameConfig, saveData));
        
        StartCoroutine(stateMachine.Execute().GetEnumerator());
    }

    private void OnApplicationQuit()
    {
        saveData.Scores = 0;
    }
}
