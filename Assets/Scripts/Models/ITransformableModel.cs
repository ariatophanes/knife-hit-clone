using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface ITransformableModel
{
    ReactiveProperty<Vector2> Position { get; set; }
    ReactiveProperty<Transform> Parent { get; set; }
    ReactiveProperty<float> Rotation { get; set; }
}
