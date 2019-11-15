using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampFarmItem : MonoBehaviour, ObjectPoolItem<enum_CampFarmItemStatus>
{
    public void OnPoolItemInit(enum_CampFarmItemStatus identity)
    {
    }
}
