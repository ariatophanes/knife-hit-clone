using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class SlicableObjectModel : InteractiveObjectModel, IHitableModel
{
    public ReactiveCommand ReceiveHitCommand { get; set; }
    public ReactiveProperty<int> HP { get; set; }
    
    public ReactiveCommand FallCommand { get; private set; }
    
    public SlicableObjectModel(InteractiveObjectModel wheelModel)
    {
        // Init Reactive Commands
        ReceiveHitCommand = new ReactiveCommand();
        FallCommand = new ReactiveCommand();

        // Init Reactive Properties
        HP  = new ReactiveProperty<int>(1);
        
        // Subsctibtions
        FallCommand.Subscribe(_ => Parent.Value = null).AddTo(disposables);
        CollideCommand.Where(collider => collider.tag == "Throwing Knife").Subscribe(_ => ReceiveHitCommand.Execute());
        wheelModel.OnBeforeDestroy.AsObservable().Subscribe(_ => FallCommand.Execute()).AddTo(disposables);
    }
    
}
