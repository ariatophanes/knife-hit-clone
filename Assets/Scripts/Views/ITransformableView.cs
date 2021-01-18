using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransformableView
{
    void Move(Vector2 newPos);
    void Rotate(float newRot);
    void SetParent(Transform parent);
}
