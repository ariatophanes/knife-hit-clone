using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IInteractiveObjectFactory
{
    ReactiveCommand<InteractiveObjectModel> OnModelCreated { get; set; }
    InteractiveObjectModel CreateModel(GameObject go);
}
