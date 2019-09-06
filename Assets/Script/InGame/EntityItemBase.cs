using System;
using GameSetting;

public class EntityItemBase : EntityBase {
    public override enum_EntityController m_Controller => enum_EntityController.None;
    Action OnItemDead;
    public void AddEvent(Action _OnDead)
    {
        OnItemDead = _OnDead;
    }
    protected override void OnDead()
    {
        base.OnDead();
        OnItemDead?.Invoke();
    }
}
