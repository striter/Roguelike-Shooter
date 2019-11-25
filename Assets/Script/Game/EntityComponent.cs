using GameSetting;
using System;
using UnityEngine;
public class EntityComponent : EntityBase {     //Temporaty Solution
    public override enum_EntityController m_Controller => enum_EntityController.None;
    Action OnEntityDead;
    public void Play(Action _OnDead)
    {
        OnEntityDead = _OnDead;
        EnableHitbox(false);
    }
    protected override void OnDead()
    {
        OnEntityDead?.Invoke();
    }
    public void OnPlay()
    {
        EnableHitbox(true);
    }
    public void OnStop()
    {
        base.OnDead();
    }
    public new void OnRecycle()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnEntityDeactivate, this as EntityBase);
    }
}
