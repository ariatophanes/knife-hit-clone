using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class WheelPresenter : InteractiveObjectPresenter
{
    private WheelModel model;
    private WheelView view;
    
    public WheelPresenter(WheelModel model, WheelView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
        
        // Model Subscriptions
        model.HP.Where(val => val > 0).Skip(1).Subscribe(_ => view.ReceiveHit(model.hitEffectColor)).AddTo(disposables);
    }
    
}

