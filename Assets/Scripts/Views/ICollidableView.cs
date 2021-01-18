using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidableView
{
    ColliderEvent OnCollide { get; set; }

    void SetColliderActivity(bool active);
    
    void Collide(Collider2D collider);
}
