using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class CampFarmItem : MonoBehaviour, IObjectpool<enum_CampFarmItemStatus>
{
    public void OnPoolItemInit(enum_CampFarmItemStatus identity, Action<enum_CampFarmItemStatus, MonoBehaviour> OnRecycle)
    {
    }
}
