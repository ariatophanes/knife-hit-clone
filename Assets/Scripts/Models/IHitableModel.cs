using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IHitableModel
{
    ReactiveCommand ReceiveHitCommand { get; set; }
    ReactiveProperty<int> HP { get; set; }
}
