using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInteractiveObjectFactory : IInteractiveObjectFactory 
{
    public ReactiveCommand<InteractiveObjectModel> OnModelCreated { get; set; }

    public SimpleInteractiveObjectFactory()
    {
        OnModelCreated = new ReactiveCommand<InteractiveObjectModel>();
    }
    
    public InteractiveObjectModel CreateModel(GameObject go)
    {
        InteractiveObjectModel model = new InteractiveObjectModel();
        IInteractiveObjectView view = go.GetComponent<SimpleInteractiveObjectView>();
        InteractiveObjectPresenter presenter = new InteractiveObjectPresenter(model, view);

        OnModelCreated.Execute(model);
        
        return model;
    }
}
