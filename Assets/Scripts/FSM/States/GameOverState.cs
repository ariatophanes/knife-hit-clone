using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameOverState : IState
{
    private GameObject ui;
    private SaveData saveData;
    private GameConfig config;

    private ReactiveCommand TryAgainCommand, GoMainMenuCommand;
    private CompositeDisposable disposables;

    public GameOverState(GameConfig config, SaveData saveData)
    {
        this.config = config;
        this.saveData = saveData;
        
        disposables = new CompositeDisposable();
        
        // Init Reactive Commands
        TryAgainCommand = new ReactiveCommand();
        GoMainMenuCommand = new ReactiveCommand();
    }
    
    public void BeginEnter()
    {
        ui = GameObject.Instantiate(Resources.Load<GameObject>("UI/Game Over UI"));
        ui.transform.Find("Scores").GetComponent<Text>().text = "Scores: " + saveData.Scores.ToString();

        saveData.Scores = 0;
        
        // Subscribtions
        TryAgainCommand.BindTo(ui.transform.Find("Buttons/Try Again Button").GetComponent<Button>()).AddTo(disposables);
        GoMainMenuCommand.BindTo(ui.transform.Find("Buttons/Menu Button").GetComponent<Button>()).AddTo(disposables);

        TryAgainCommand.Subscribe(_ =>
        {
            StateBeginExitEventArgs args = new StateBeginExitEventArgs(new GameState(config, saveData), new ScreenFadeTransition(0.5f));
            OnBeginExit(this, args);
        }).AddTo(disposables);
        
        GoMainMenuCommand.Subscribe(_ =>
        {
            StateBeginExitEventArgs args = new StateBeginExitEventArgs(new MenuState(config, saveData), new ScreenFadeTransition(0.5f));
            OnBeginExit(this, args);
        }).AddTo(disposables);
    }

    public void EndEnter()
    {
        
    }

    public IEnumerable Execute()
    {
        yield return null;
    }

    public event EventHandler<StateBeginExitEventArgs> OnBeginExit;
    public void EndExit()
    {
        GameObject.Destroy(ui);
        disposables.Clear();
    }
}
