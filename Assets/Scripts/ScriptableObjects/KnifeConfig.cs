using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Knife Config", menuName = "ScriptableObjects/Knife Config", order = 1)]
public class KnifeConfig : ScriptableObject
{
    public GameObject prefab;
    public float speed;
}
