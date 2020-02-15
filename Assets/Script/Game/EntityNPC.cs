using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EntityNPC : EntityBase {
    public override enum_EntityController m_ControllType => enum_EntityController.None;
    public void OnActivate()
    {
        base.OnActivate( enum_EntityFlag.Neutal);
    }
}
