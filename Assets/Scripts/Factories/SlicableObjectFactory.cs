using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SlicableObjectFactory : IInteractiveObjectFactory
{
    public ReactiveCommand<InteractiveObjectModel> OnModelCreated { get; set; }

    private InteractiveObjectModel wheelModel;
    
    public SlicableObjectFactory(InteractiveObjectModel wheelModel)
    {
        this.wheelModel = wheelModel;
        
        OnModelCreated = new ReactiveCommand<InteractiveObjectModel>();
    }
    
    public InteractiveObjectModel CreateModel(GameObject go)
    {
        SlicableObjectModel model = new SlicableObjectModel(wheelModel);
        SlicableObjectView view = go.GetComponent<SlicableObjectView>();
        SlicableObjectPresenter presenter = new SlicableObjectPresenter(model, view);

        OnModelCreated.Execute(model);

        return model;
    }
}
