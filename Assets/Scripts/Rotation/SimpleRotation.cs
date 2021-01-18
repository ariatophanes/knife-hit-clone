using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : IRotation
{
    private float currentRotation;
    
    public float Rotate(float speed)
    {
        currentRotation -= speed * Time.deltaTime;
        
        return currentRotation;
    }
}
