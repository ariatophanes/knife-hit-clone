using System;
using System.Collections;
using System.Collections.Generic;
using NatSuite.Sharing;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MenuState : IState
{
    public event EventHandler<StateBeginExitEventArgs> OnBeginExit;

    private GameObject ui;

    private ReactiveCommand GoGameplayStateCommand, ShareCommand, RateUsCommand;

    private CompositeDisposable disposables;
    
    private SaveData saveData;

    private GameConfig config;

    public MenuState(GameConfig config, SaveData saveData)
    {
        this.saveData = saveData;
        this.config = config;
        
        disposables = new CompositeDisposable();
        
        GoGameplayStateCommand = new ReactiveCommand();
        ShareCommand = new ReactiveCommand();
        RateUsCommand = new ReactiveCommand();
    }
    
    public void BeginEnter()
    {
        ui = GameObject.Instantiate(Resources.Load<GameObject>("UI/Menu UI"));
        ui.transform.Find("Best Score").GetComponent<Text>().text = "Best: " + saveData.BestScore.ToString();
        
        // Subscribtions
        GoGameplayStateCommand.BindTo(ui.transform.Find("Buttons/Play Button").GetComponent<Button>()).AddTo(disposables);
        ShareCommand.BindTo(ui.transform.Find("Buttons/Share Button").GetComponent<Button>()).AddTo(disposables);
        RateUsCommand.BindTo(ui.transform.Find("Buttons/Rate Us Button").GetComponent<Button>()).AddTo(disposables);
        
        GoGameplayStateCommand.Subscribe(_ =>
        {
            StateBeginExitEventArgs args = new StateBeginExitEventArgs(new GameState(config, saveData), new ScreenFadeTransition(0.5f));
            OnBeginExit(this, args);
        }).AddTo(disposables);

        ShareCommand.Subscribe(_ => new SharePayload().AddText("Check out Vadim Smirnov's Knife Hit Clone!").Commit()).AddTo(disposables);

        RateUsCommand.Subscribe(_ => Application.OpenURL("https://play.google.com/store")).AddTo(disposables);
    }

    public void EndEnter()
    {
        
    }

    public IEnumerable Execute()
    {
        yield return null;
    }

    public void EndExit()
    {
        GameObject.Destroy(ui);
        disposables.Clear();
    }
}
