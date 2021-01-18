using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntervalRotation : IRotation
{
    private float currentRotation, k;
    private const float h = 1.2f;
    public float Rotate(float speed)
    {
        k = Mathf.Clamp01(Mathf.Abs(Mathf.Sin(Time.time) + h));
        currentRotation -= speed * Time.deltaTime * k;
        
        return currentRotation;
    }
}