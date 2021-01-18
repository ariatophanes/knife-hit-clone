using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class KnifePresenter : InteractiveObjectPresenter
{
    private KnifeModel model;
    private KnifeView view;
    public KnifePresenter(KnifeModel model, KnifeView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
        
        // Model Subsctibtions
        model.FallCommand.Subscribe(_ => view.Fall()).AddTo(disposables);
    }
}
