using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface ICollidableModel
{
    ReactiveCommand<Collider2D> CollideCommand { get; set; }
    ReactiveProperty<bool> IsCollidable { get; set; }
}
