using System;
using GameSetting;

public class EntityComponent : EntityBase {
    public override enum_EntityController m_Controller => enum_EntityController.None;

    Action OnEntityDead;
    public void ActionOnDead(Action _OnDead)
    {
        OnEntityDead = _OnDead;
    }
    protected override void OnDead()
    {
        base.OnDead();
        OnEntityDead?.Invoke();
        OnRecycle();
    }
}
