using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class WheelFactory : IInteractiveObjectFactory
{
    public ReactiveCommand<InteractiveObjectModel> OnModelCreated { get; set; }
    private WheelConfig config;
    private IInteractiveObjectFactory partFactory;
    
    public WheelFactory(WheelConfig config, IInteractiveObjectFactory partFactory)
    {
        this.config = config;
        this.partFactory = partFactory;
        
        OnModelCreated = new ReactiveCommand<InteractiveObjectModel>();
    }
    
    public InteractiveObjectModel CreateModel(GameObject go)
    {
        WheelView wheelView = go.GetComponent<WheelView>();
        WheelModel wheelModel = new WheelModel(config, partFactory);
        WheelPresenter presenter = new WheelPresenter(wheelModel, wheelView);
        
        wheelView.SetPartFactory(partFactory);
        
        OnModelCreated.Execute(wheelModel);
        
        return wheelModel;
    }
}
