using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SimpleInteractiveObjectView :MonoBehaviour, IInteractiveObjectView
{
    public ColliderEvent OnCollide { get; set; }
    private Collider2D collider;

    private void Awake()
    {
        OnCollide = new ColliderEvent();

        collider = GetComponent<Collider2D>();
        
        gameObject.OnTriggerEnter2DAsObservable().TakeUntilDestroy(this).Subscribe(collider2D => OnCollide.Invoke(collider2D));
    }
    
    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

    public void SetColliderActivity(bool active)
    {
        collider.enabled = active;
    }

    public void Collide(Collider2D collider)
    {
        
    }

    public void ChangeTag(string tag)
    {
        transform.tag = tag;
    }

    public void Move(Vector2 newPos)
    {
        transform.position = newPos;
    }

    public void Rotate(float newRot)
    {
        Vector3 rot = transform.eulerAngles;
        rot.z = newRot;

        transform.eulerAngles = rot;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }
}
