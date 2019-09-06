using System;
using GameSetting;

public class EntityComponent : EntityBase {
    public override enum_EntityController m_Controller => enum_EntityController.None;
    Action OnItemDead;
    public override void OnActivate(enum_EntityFlag _flag)
    {
        base.OnActivate(_flag);
    }
    public void AttachComponent(Action _OnDead)
    {
        OnItemDead = _OnDead;
    }
    protected override void OnDead()
    {
        base.OnDead();
        OnItemDead?.Invoke();
    }
}
