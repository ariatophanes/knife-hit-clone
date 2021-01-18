using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class KnifeModel : InteractiveObjectModel
{
    // Reactive Commands
    public ReactiveCommand StartMovingCommand { get; private set; }
    
    public ReactiveCommand StopMovingCommand { get; private set; }
    
    public ReactiveCommand FallCommand { get; private set; }
    
    // Fields
    
    private IDisposable movingDisposable;

    public KnifeModel(KnifeConfig config, InteractiveObjectModel wheelModel, bool moveOnStart)
    {
        Tag.Value = "Knife";
        
        // Init Commands
        StartMovingCommand  = new ReactiveCommand();
        StopMovingCommand = new ReactiveCommand();
        FallCommand = new ReactiveCommand();

        // Subscribtions
        
        FallCommand.Subscribe(_ =>
        {
            IsCollidable.Value = false;
            Parent.Value = null;
            StopMovingCommand.Execute();
        }).AddTo(disposables);
        
        wheelModel.OnBeforeDestroy.AsObservable().Subscribe(_ => FallCommand.Execute()).AddTo(disposables);
        
        StartMovingCommand.Subscribe(_ =>
        {
            movingDisposable = Observable.EveryUpdate().Subscribe(__ => Position.Value += Vector2.up * config.speed * Time.deltaTime);
            disposables.Add(movingDisposable);
        }).AddTo(disposables);

        StopMovingCommand.Subscribe(_ => movingDisposable?.Dispose()).AddTo(disposables);
        
        CollideCommand.Where(collider => collider.tag == "Wheel").Where(_ => Tag.Value == "Throwing Knife").Subscribe(
            collider =>
            {
                StopMovingCommand.Execute();
                Parent.Value = collider.transform;
                Position.Value = collider.transform.position + Vector3.down * 1.5f;
            }).AddTo(disposables);

        CollideCommand.Where(collider => collider.tag == "Throwing Knife" || collider.tag == "Knife").Subscribe(_ => FallCommand.Execute()).AddTo(disposables);
        
        if (moveOnStart) StartMovingCommand.Execute();
    }
}
