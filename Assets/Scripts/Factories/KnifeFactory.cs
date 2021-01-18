using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class KnifeFactory : IInteractiveObjectFactory
{
    public ReactiveCommand<InteractiveObjectModel> OnModelCreated { get; set; }

    private KnifeConfig config;
    private InteractiveObjectModel wheelModel;
    private bool moveOnStart;
    
    public KnifeFactory(KnifeConfig config, InteractiveObjectModel wheelModel, bool moveOnStart)
    {
        this.config = config;
        this.wheelModel = wheelModel;
        this.moveOnStart = moveOnStart;
        
        OnModelCreated = new ReactiveCommand<InteractiveObjectModel>();
    }
    
    public InteractiveObjectModel CreateModel(GameObject go)
    {
        KnifeView knifeView = go.GetComponent<KnifeView>();
        KnifeModel knifeModel = new KnifeModel(config, wheelModel, moveOnStart);
        KnifePresenter presenter = new KnifePresenter(knifeModel, knifeView);

        OnModelCreated.Execute(knifeModel);

        return knifeModel;
    }
}
