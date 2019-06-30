using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestructedEvent : MonoBehaviour,IDestructible
{
    public event Action IDied;

    public void OnDestruction(GameObject destroyer)
    {
        if (IDied != null)
        {
            IDied();
        }
    }
}
