using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Config", menuName = "ScriptableObjects/Game Config", order = 1)]
public class GameConfig : ScriptableObject
{
    public float reloadingTime;
    [Range(0,1)]  public float chanceToSpawnApple;
    public KnifeConfig knifeConfig;
    public WheelConfig wheelConfig;
    public GameObject applePrefab;
}
