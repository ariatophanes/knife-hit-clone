using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
public class WheelView : MonoBehaviour, IInteractiveObjectView, IHitableView
{
    // Reactive Commands
    public ReactiveCommand DestroyCommand { get; set; }
    
    // Events
    public ColliderEvent OnCollide { get; set; }
    
    // Fields
    private SpriteRenderer sr;
    private Explodable explodable;
    private new BoxCollider2D collider;
    
    //Factories
    private IInteractiveObjectFactory partFactory;
    
    private void Awake()
    {
        // Init Events
        OnCollide = new ColliderEvent();
        
        // Init Reactive Commands
        DestroyCommand = new ReactiveCommand();
        
        // Init Fields
        sr = GetComponent<SpriteRenderer>();
        explodable = GetComponent<Explodable>();
        collider = GetComponent<BoxCollider2D>();
        
        // Subscribtions
        IObservable<Collider2D> collidingStream = gameObject.OnTriggerEnter2DAsObservable();
        collidingStream.Buffer(collidingStream.ThrottleFirst(TimeSpan.FromSeconds(0.1f))).TakeUntilDestroy(this)
            .Subscribe(colliders => OnCollide.Invoke(colliders.LastOrDefault()));
    }

    public void SetPartFactory(IInteractiveObjectFactory partFactory)
    {
        this.partFactory = partFactory;
    }
    
    public void Move(Vector2 newPos)
    {
        transform.position = newPos;
    }

    public void Rotate(float rot)
    {
        Vector3 newRot = transform.eulerAngles;
        newRot.z = rot;
        
        transform.eulerAngles = newRot;
    }

    public void SetParent(Transform parent)
    {
        transform.parent = parent;
    }

    public void ReceiveHit(Color hitColor)
    {
        transform.DOShakePosition(0.1f,  0.2f, 2, 0);
        sr.DOColor(hitColor, 0.2f).From();
    }
    
    public void Destroy()
    {
        explodable.OnFragmentsGenerated.AsObservable().Subscribe(list => list.ForEach(go => partFactory.CreateModel(go)));
        explodable.explode();
    }

    public void Collide(Collider2D collider)
    {
        
    }

    public void SetColliderActivity(bool active)
    {
        collider.enabled = active;
    }

    public void ChangeTag(string tag)
    {
        gameObject.tag = tag;
    }

    public void ReceiveHit()
    {
        
    }
}
