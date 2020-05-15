using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EntityNPC : EntityBase {
    public override enum_EntityType m_ControllType => enum_EntityType.None;
    public EntityNPC OnActivate()
    {
        OnEntityActivate(enum_EntityFlag.Neutal);
        return this;
    }
}
