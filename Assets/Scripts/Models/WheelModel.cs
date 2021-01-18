using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class WheelModel : InteractiveObjectModel, IHitableModel
{
    // Reactive Properties
    public ReactiveProperty<int> HP { get; set; }
    
    // Reactive Commands
    public ReactiveCommand ReceiveHitCommand { get; set; }

    // Fields
    public Color hitEffectColor { get; private set; } = Color.yellow;
    
    public IInteractiveObjectFactory interactiveObjectFactory;
    
    public WheelModel(WheelConfig config, IInteractiveObjectFactory  interactiveObjectFactory)
    {
        this.interactiveObjectFactory = interactiveObjectFactory;

        Tag.Value = "Wheel";

        // Init Reactive Properties
        HP = new ReactiveProperty<int>(config.maxHP);

        // Init Reactive Commands
        ReceiveHitCommand = new ReactiveCommand();
        
        // Subscribtions
        Observable.EveryUpdate().Subscribe(_ => Rotation.Value = config.rotation.Rotate(config.rotSpeed)).AddTo(disposables);
        
        ReceiveHitCommand.Subscribe(_ => HP.Value -= 1).AddTo(disposables);
        
        CollideCommand.Subscribe(collider =>
        {
            if (!collider.CompareTag("Throwing Knife")) return;
            ReceiveHitCommand.Execute();
        }).AddTo(disposables);
        
        HP.Where(val => val <= 0).Take(1).Subscribe(_ => OnBeforeDestroy.Invoke());
    }
}
