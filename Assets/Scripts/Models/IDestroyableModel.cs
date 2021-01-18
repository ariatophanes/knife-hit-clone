using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public interface IDestroyableModel
{
    ReactiveProperty<bool> Destroyed { get; set; }
    ReactiveCommand DestroyCommand { get; set; }
    UnityEvent OnBeforeDestroy { get; set; }

}
