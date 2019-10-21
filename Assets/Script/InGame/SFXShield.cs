using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXShield : SFXBase {
    protected override bool m_AutoRecycle => false;
    public EntityComponent m_Health { get; private set; }
    EntityCharacterBase m_Attacher;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Health = GetComponentInChildren<EntityComponent>();
        m_Health.Init(-1);
        m_Health.ActionOnDead(Recycle);
    }
    public void Attach(EntityCharacterBase _attacher)
    {
        base.PlaySFX(_attacher.I_EntityID, -1);
        transform.SetParent(_attacher.tf_Head);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        m_Health.OnActivate(_attacher.m_Flag);
        m_Attacher = _attacher;
    }
    protected override void Update()
    {
        base.Update();
        if (!b_Playing)
            return;
        if (m_Attacher.m_Health.b_IsDead)
            OnRecycle();
    }
}
