﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructible
{
    void OnDestruction(GameObject destroyer);
}
