using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SlicableObjectPresenter : InteractiveObjectPresenter
{
    private SlicableObjectModel model;
    private SlicableObjectView view;
    
    public SlicableObjectPresenter(SlicableObjectModel model, SlicableObjectView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
        
        // Model Subsctibtions
        model.ReceiveHitCommand.Take(1).Subscribe(_ => view.ReceiveHit()).AddTo(disposables);
        model.FallCommand.Subscribe(_ => view.Fall()).AddTo(disposables);
    }
}
