﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick : MonoBehaviour
{
    public void Set()
    {
        Model.Instance.SetPlayerShip();
    }

    public void Change()
    {
        Model.Instance.ChangeShip();
    }
}
