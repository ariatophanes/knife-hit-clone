using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class ExplodableFragments : ExplodableAddon{
    public override void OnFragmentsGenerated(List<GameObject> fragments)
    {
        foreach (GameObject fragment in fragments)
        {
            fragment.AddComponent<SimpleInteractiveObjectView>();
            
            Explodable fragExp = fragment.AddComponent<Explodable>();
            fragExp.shatterType = explodable.shatterType;
            fragExp.fragmentLayer = explodable.fragmentLayer;
            fragExp.sortingLayerName = explodable.sortingLayerName;
            fragExp.orderInLayer = explodable.orderInLayer;

            fragment.layer = explodable.gameObject.layer;

            fragExp.fragmentInEditor();

            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.up * 5 + new Vector2(UnityEngine.Random.Range(-0.5f,0.5f),  UnityEngine.Random.Range(0,1)) * 5, ForceMode2D.Impulse);
        }
    }
}
