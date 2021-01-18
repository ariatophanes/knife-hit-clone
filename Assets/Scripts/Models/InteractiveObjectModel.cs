using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObjectModel : ITransformableModel, IDestroyableModel, ICollidableModel, ITagableModel
{
    // Events
    public UnityEvent OnBeforeDestroy { get; set; }

    // Reactive Properties
    public ReactiveProperty<float> Rotation { get; set; }

    public ReactiveProperty<Vector2> Position { get; set; }

    public ReactiveProperty<Transform> Parent { get; set; }
    
    public ReactiveProperty<string> Tag { get; set; }

    public ReactiveCommand<Collider2D> CollideCommand { get; set; }
    
    public ReactiveProperty<bool> IsCollidable { get; set; }

    // Reactive Commands
    public ReactiveProperty<bool> Destroyed { get; set; }
    public ReactiveCommand DestroyCommand { get; set; }

    protected CompositeDisposable disposables;

    public InteractiveObjectModel()
    {
        // Init Fields
        disposables = new CompositeDisposable();
        
        // Init Events
        OnBeforeDestroy = new UnityEvent();
        
        // Init Reactive Properties
        Destroyed = new ReactiveProperty<bool>(false);
        Rotation = new ReactiveProperty<float>();
        Position = new ReactiveProperty<Vector2>();
        Parent = new ReactiveProperty<Transform>();
        IsCollidable = new ReactiveProperty<bool>(true);
        Tag = new ReactiveProperty<string>("Untagged");
        
        // Init Reactive Commands
        DestroyCommand = new ReactiveCommand();
        CollideCommand = new ReactiveCommand<Collider2D>();
        
        OnBeforeDestroy.AsObservable().Subscribe(_ =>
        {
            Destroyed.Value = true;
            DestroyCommand.Execute();
        });
        
        DestroyCommand.Subscribe(_ => disposables.Clear()).AddTo(disposables);
    }
}
