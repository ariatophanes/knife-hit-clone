using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class InteractiveObjectPresenter
{
    private InteractiveObjectModel model;
    private IInteractiveObjectView view;

    protected CompositeDisposable disposables;
    public InteractiveObjectPresenter(InteractiveObjectModel model, IInteractiveObjectView view)
    {
        this.model = model;
        this.view = view;
        
        disposables = new CompositeDisposable();
        
        // Model Subscribtions
        model.Position.Subscribe(pos => view.Move(pos));
        model.Rotation.Subscribe(rot => view.Rotate(rot));
        model.CollideCommand.Subscribe(collider => view.Collide(collider)).AddTo(disposables);
        model.IsCollidable.Subscribe(val => view.SetColliderActivity(val)).AddTo(disposables);
        model.Parent.Subscribe(parent => view.SetParent(parent)).AddTo(disposables);
        model.Tag.Subscribe(tag => view.ChangeTag(tag)).AddTo(disposables);
        model.IsCollidable.Subscribe(val => view.SetColliderActivity(val)).AddTo(disposables);

        model.DestroyCommand.Subscribe(_ =>
        {
            disposables.Clear();
            view.Destroy();
        }).AddTo(disposables);
        
        // View Subscribtions
        view.OnCollide.AsObservable().Subscribe(collider => model.CollideCommand.Execute(collider)).AddTo(disposables);
    }
}
