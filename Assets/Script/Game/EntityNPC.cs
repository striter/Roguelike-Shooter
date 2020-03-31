using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EntityNPC : EntityBase {
    public override enum_EntityControlType m_ControllType => enum_EntityControlType.None;
    public void OnActivate()
    {
        EntityActivate( enum_EntityFlag.Neutal);
    }
}
