using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UniRx.Toolkit;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class GameState : IState
{
    // Events
    public event EventHandler<StateBeginExitEventArgs> OnBeginExit;

    // Fields
    private GameConfig gameConfig;
    private GameObject ui, knivesSpawnPoint;
    private ParticleSystem wheelHitPS;
    private KnivesCountIndicator indicator;
    private CompositeDisposable disposables;
    private int maxKnivesCount;
    private Queue<Vector2> freePosOnWheel;
    private List<InteractiveObjectModel> interactiveObjectModels;
    private InteractiveObjectModel wheelModel;
    private SaveData saveData;

    // Reactive Properties
    private ReactiveProperty<int> CurrentKnivesCount, Scores;
    private ReactiveProperty<bool> GameEnded;

    // Reactive Commands
    private ReactiveCommand WinCommand;
    private ReactiveCommand FailCommand;

    // Factories
    private IInteractiveObjectFactory wheelFactory,
        initialKnifeFactory,
        throwingKnifeFactory,
        simpleInteractiveObjectFactory,
        slicableObjectFactory;

    public GameState(GameConfig gameConfig, SaveData saveData)
    {
        this.saveData = saveData;
        this.gameConfig = gameConfig;

        // Init Fields
        freePosOnWheel = new Queue<Vector2>();
        disposables = new CompositeDisposable();
        interactiveObjectModels = new List<InteractiveObjectModel>();

        // Init Reactive Properties
        CurrentKnivesCount = new ReactiveProperty<int>(maxKnivesCount);
        Scores = new ReactiveProperty<int>(saveData.Scores);
        GameEnded = new ReactiveProperty<bool>(false);

        // Init Reactive Commands
        FailCommand = new ReactiveCommand();
        WinCommand = new ReactiveCommand();
    }

    public void BeginEnter()
    {
        saveData.Level++;

        // Initialization
        maxKnivesCount = UnityEngine.Random.Range(5, 12);
        knivesSpawnPoint = GameObject.Instantiate(Resources.Load<GameObject>("Objects/Knife Icon"));
        ui = GameObject.Instantiate(Resources.Load<GameObject>("UI/Gameplay UI"));
        ui.transform.Find("Level").GetComponent<Text>().text = "LVL " + saveData.Level;
        indicator = ui.GetComponentInChildren<KnivesCountIndicator>();
        indicator.Init(maxKnivesCount);
        Vibration.Init();

        SpawnWheel();
        GenerateFreeWheelPositions();
        InitFactories();

        if (UnityEngine.Random.value <= gameConfig.chanceToSpawnApple) SpawnApple();
        Observable.Range(1, UnityEngine.Random.Range(1, 4)).Subscribe(i => SpawnInitialKnife());

        // Subsctibtions
        Scores.SubscribeToText(ui.transform.Find("Scores").GetComponent<Text>()).AddTo(disposables);
        wheelModel.DestroyCommand.Subscribe(_ =>
        {
            Vibration.Vibrate(300);
            Win();
        }).AddTo(disposables);

        FailCommand.Subscribe(_ =>
        {
            SaveProgress();
            Flash();
            GoGameOverState();

            GameEnded.Value = true;
        }).AddTo(disposables);

        WinCommand.Subscribe(_ =>
        {
            SaveProgress();
            GoGameState();

            GameEnded.Value = true;
        });

        IObservable<long> clickStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0))
            .Where(_ => GameEnded.Value == false);
        clickStream.Buffer(clickStream.ThrottleFirst(TimeSpan.FromSeconds(gameConfig.reloadingTime)))
            .Subscribe(_ => ThrowKnife()).AddTo(disposables);
    }

    private void InitFactories()
    {
        initialKnifeFactory = new KnifeFactory(gameConfig.knifeConfig, wheelModel, false);
        throwingKnifeFactory = new KnifeFactory(gameConfig.knifeConfig, wheelModel, true);
        slicableObjectFactory = new SlicableObjectFactory(wheelModel);

        simpleInteractiveObjectFactory.OnModelCreated.Subscribe(model => interactiveObjectModels.Add(model));
        initialKnifeFactory.OnModelCreated.Subscribe(model => interactiveObjectModels.Add(model));
        throwingKnifeFactory.OnModelCreated.Subscribe(model => interactiveObjectModels.Add(model));
        slicableObjectFactory.OnModelCreated.Subscribe(model =>
            interactiveObjectModels.Add(model)); //TODO: unparent from wheel and fall
    }

    private void SpawnApple()
    {
        InteractiveObjectModel appleModel =
            slicableObjectFactory.CreateModel(GameObject.Instantiate(gameConfig.applePrefab));
        appleModel.CollideCommand.Where(collider => collider.tag == "Throwing Knife").Subscribe(_ => Scores.Value += 2)
            .AddTo(disposables);

        PutToWheel(appleModel, 0.2f, 90);
    }

    private void SpawnInitialKnife()
    {
        InteractiveObjectModel knifeModel =
            initialKnifeFactory.CreateModel(GameObject.Instantiate(gameConfig.knifeConfig.prefab));

        PutToWheel(knifeModel, 0, -90);
    }

    private void SpawnWheel()
    {
        gameConfig.wheelConfig.maxHP = maxKnivesCount;
        gameConfig.wheelConfig.rotation = UnityEngine.Random.value > 0.5f
            ? (IRotation) new IntervalRotation()
            : (IRotation) new SimpleRotation();

        simpleInteractiveObjectFactory = new SimpleInteractiveObjectFactory();

        wheelFactory = new WheelFactory(gameConfig.wheelConfig, simpleInteractiveObjectFactory);

        wheelModel = wheelFactory.CreateModel(GameObject.Instantiate(gameConfig.wheelConfig.wheelPrefab));
        wheelModel.Position.Value = new Vector2(0, 1.5f);
        interactiveObjectModels.Add(wheelModel);

        wheelHitPS = GameObject.Instantiate(Resources.Load<GameObject>("VFX/Wheel Hit PS"))
            .GetComponent<ParticleSystem>();
        wheelHitPS.transform.position = wheelModel.Position.Value + Vector2.down * 1.5f;
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
        disposables.Clear();
        GameObject.Destroy(ui);
        GameObject.Destroy(knivesSpawnPoint);
        while (interactiveObjectModels.Count > 0) DestroyAllInteractiveObjectsOnScene();
    }

    private void ThrowKnife()
    {
        InteractiveObjectModel spawnedKnife =
            throwingKnifeFactory.CreateModel(GameObject.Instantiate(gameConfig.knifeConfig.prefab));

        spawnedKnife.Tag.Value = "Throwing Knife";

        spawnedKnife.Position.Value = knivesSpawnPoint.transform.position;

        spawnedKnife.CollideCommand.Where(collider => collider.tag == "Wheel").Subscribe(_ =>
        {
            Vibration.Vibrate(50);
            wheelHitPS.Play();
            Scores.Value++;
        }).AddTo(disposables);

        spawnedKnife.CollideCommand.Where(collider => collider.tag == "Throwing Knife" || collider.tag == "Knife")
            .Where(_ => GameEnded.Value == false).Subscribe(_ =>
            {
                Vibration.Vibrate(50);
                Fail();
            }).AddTo(disposables);

        CurrentKnivesCount.Value--;

        indicator.MinusOne();

        TweenKnifeIconPos();
    }

    private void Fail()
    {
        // We need (Game Ended == true) condition to be sure we don't execute fail and win command if knife
        // collided with another knife and wheel at the same time

        if (GameEnded.Value == true) return;

        ParticleSystem vfx = GameObject.Instantiate(Resources.Load<GameObject>("VFX/Knife Hit PS"))
            .GetComponent<ParticleSystem>();

        vfx.transform.position = wheelModel.Position.Value + Vector2.down * 1.8f;

        FailCommand.Execute();
    }

    private void Win()
    {
        // We need Game Ended == false condition to be sure we don't execute fail and win command if knife
        // collided with another knife and wheel at the same time

        if (GameEnded.Value == true) return;

        WinCommand.Execute();
    }

    private void TweenKnifeIconPos()
    {
        knivesSpawnPoint.transform.position -= Vector3.up * 1.25f;
        knivesSpawnPoint.transform.DOMove(knivesSpawnPoint.transform.position + Vector3.up * 1.25f,
            gameConfig.reloadingTime);
    }

    private void GoGameOverState()
    {
        Observable.Interval(TimeSpan.FromSeconds(0.35f)).Take(1).Subscribe(_ =>
        {
            StateBeginExitEventArgs args =
                new StateBeginExitEventArgs(new GameOverState(gameConfig, saveData), new ScreenFadeTransition(0.5f));
            OnBeginExit(this, args);
        });
    }

    private void GoGameState()
    {
        Observable.Interval(TimeSpan.FromSeconds(1)).Take(1).Subscribe(_ =>
        {
            StateBeginExitEventArgs args =
                new StateBeginExitEventArgs(new GameState(gameConfig, saveData), new ScreenFadeTransition(0.5f));
            OnBeginExit(this, args);
        });
    }

    private void Flash()
    {
        Color newColor = Color.white;
        newColor.a = 0.75f;

        ui.transform.Find("Flash").GetComponent<Image>().DOColor(newColor, 0.25f).From();
    }

    private void PutToWheel(ITransformableModel model, float additiveH, float additiveAngle)
    {
        Vector2 newPosOnWheel = freePosOnWheel.Dequeue();
        Vector2 dirToWheelCenter = wheelModel.Position.Value - newPosOnWheel;

        float newAngle =
            Mathf.Atan2(wheelModel.Position.Value.y - newPosOnWheel.y, wheelModel.Position.Value.x - newPosOnWheel.x) *
            Mathf.Rad2Deg;

        model.Position.Value = newPosOnWheel - dirToWheelCenter.normalized * additiveH;
        model.Rotation.Value = newAngle + additiveAngle;
        model.Parent.Value = GameObject.FindGameObjectWithTag("Wheel").transform;
    }

    private void GenerateFreeWheelPositions()
    {
        freePosOnWheel.Enqueue(wheelModel.Position.Value + UnityEngine.Random.insideUnitCircle.normalized * 1.5f);

        for (int i = 0; i < 3; i++)
        {
            Vector2 newFreePos = Vector2.zero;
            bool validPos = false;

            while (validPos == false)
            {
                newFreePos = wheelModel.Position.Value + UnityEngine.Random.insideUnitCircle.normalized * 1.5f;
                foreach (var pos in freePosOnWheel)
                    if (Vector2.Distance(newFreePos, pos) > 0.75f) validPos = true;
                    else
                    {
                        validPos = false;
                        break;
                    }
            }
            
            freePosOnWheel.Enqueue(newFreePos);
        }
    }

    private void SaveProgress()
    {
        saveData.Scores = Scores.Value;
        saveData.BestScore = Scores.Value > saveData.BestScore ? Scores.Value : saveData.BestScore;
    }

    private void DestroyAllInteractiveObjectsOnScene()
    {
        List<InteractiveObjectModel> models = interactiveObjectModels.ToList();

        foreach (InteractiveObjectModel model in models)
        {
            if (!model.Destroyed.Value) model.OnBeforeDestroy.Invoke();
            interactiveObjectModels.Remove(model);
        }
    }
}