using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface ITagableModel
{
    ReactiveProperty<string> Tag { get; set; }
}
