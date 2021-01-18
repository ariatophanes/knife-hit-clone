using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SlicableObjectView : SimpleInteractiveObjectView
{
    private Rigidbody2D rb1, rb2, rb3;
    private ParticleSystem ps;

    private void Start()
    {
        rb1 = transform.GetChild(0).GetComponent<Rigidbody2D>();
        rb2 = transform.GetChild(1).GetComponent<Rigidbody2D>();
        rb3 = transform.GetChild(2).GetComponent<Rigidbody2D>();

        ps = GetComponentInChildren<ParticleSystem>();
    }

    public void ReceiveHit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);

        ps.Play();
    }

    public void Fall()
    {
        if (rb1.gameObject.activeSelf)
        {
            rb1.bodyType = RigidbodyType2D.Dynamic;
            rb1.gravityScale = 0.5f;
            rb1.AddForce(transform.up * 5 * UnityEngine.Random.value * (UnityEngine.Random.value + 0.3f), ForceMode2D.Impulse);
        }
        else
        {
            rb2.bodyType = RigidbodyType2D.Dynamic;
            rb2.gravityScale = 0.5f;
            rb2.AddForce(transform.up * 5 * (UnityEngine.Random.value + 0.3f) + new Vector3(UnityEngine.Random.Range(-1,1), 0), ForceMode2D.Impulse);

            rb3.bodyType = RigidbodyType2D.Dynamic;
            rb3.gravityScale = 0.5f;
            rb3.AddForce(transform.up * 5 * UnityEngine.Random.value * (UnityEngine.Random.value + 0.3f) + new Vector3(UnityEngine.Random.Range(-1,1), 0), ForceMode2D.Impulse);
        }
    }
}