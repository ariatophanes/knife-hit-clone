using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wheel Config", menuName = "ScriptableObjects/Wheel Config", order = 1)]
public class WheelConfig : ScriptableObject
{
    public GameObject wheelPrefab;
    public float rotSpeed;
    [HideInInspector] public int maxHP;
    [HideInInspector] public IRotation rotation;
}
