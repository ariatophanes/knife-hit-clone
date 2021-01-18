using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class KnifeView : MonoBehaviour, IInteractiveObjectView
{
    // Fields
    private Rigidbody2D rb;
    private new BoxCollider2D collider;
    
    // Events
    public ColliderEvent OnCollide { get;  set; }
    
    private void Awake()
    {
        // Init Events
        OnCollide = new ColliderEvent();
        
        // Set Fields
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        // Subsctibtions
        gameObject.OnTriggerEnter2DAsObservable().TakeUntilDisable(this).Subscribe(collider => OnCollide.Invoke(collider));
    }
    
    public void Move(Vector2 pos)
    {
        transform.position = pos;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void Fall()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0.5f;
        rb.AddForce(-transform.up * 5, ForceMode2D.Impulse);
    }
    
    public void Rotate(float rot)
    {
        Vector3 newRot = transform.eulerAngles;
        newRot.z = rot;

        transform.eulerAngles = newRot;
    }

    public void ChangeTag(string tag)
    {
        gameObject.tag = tag;
    }
    
    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

    public void Collide(Collider2D collider)
    {
        if (collider.tag == "Knife" || collider.tag == "Throwing Knife") rb.angularVelocity = UnityEngine.Random.Range(300, 700) * (UnityEngine.Random.value > 0.5f ? -1 : 1);
    }

    public void SetColliderActivity(bool active)
    {
        collider.enabled = active;
    }
}
