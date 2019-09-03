using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractContainer : InteractBase {

    public InteractBase m_InteractTarget { get; private set; }
    Transform tf_Model;
    public override void Init()
    {
        base.Init();
        tf_Model = transform.Find("Container/Model");
    }
    public override bool TryInteract(EntityPlayerBase _interactor)
    {
        if (!B_CanInteract(_interactor) || !m_InteractTarget.TryInteract(_interactor))
            return false;
        return base.TryInteract(_interactor);
    }
    protected void Attach(InteractBase _interactItem)
    {
        m_InteractTarget = _interactItem;
        m_InteractTarget.SetInteractable(false);
        m_InteractTarget.transform.SetParentResetTransform(tf_Model);
    }
    protected void Detach()
    {
        m_InteractTarget.SetInteractable(true);
        m_InteractTarget.transform.SetParent(transform.parent);
        m_InteractTarget = null;
    }
}
