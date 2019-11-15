using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXShield : SFXBase {
    protected override bool m_AutoRecycle => false;
    public EntityComponent m_Health { get; private set; }
    EntityCharacterBase m_Attacher;
    public override void OnPoolItemInit(int identity)
    {
        base.OnPoolItemInit(identity);
        m_Health = GetComponentInChildren<EntityComponent>();
        m_Health.OnPoolItemInit(-1);
        m_Health.Play(Recycle);
    }
    public void Attach(EntityCharacterBase _attacher)
    {
        base.PlaySFX(_attacher.m_EntityID, -1,-1);
        transform.SetParent(_attacher.tf_Head);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        m_Health.OnActivate(_attacher.m_Flag);
        m_Attacher = _attacher;
    }
    protected override void Update()
    {
        base.Update();
        if (!B_Playing)
            return;
        if (m_Attacher.m_Health.b_IsDead)
            OnRecycle();
    }
}
