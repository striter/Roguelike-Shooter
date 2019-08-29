using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(HitCheckDynamic))]
public class InteractBase : MonoBehaviour {
    public bool B_Interactable(EntityPlayerBase _interactor) => (!B_Interacted||!B_InteractaOnce) && B_CanInteract(_interactor);
    public virtual enum_Interaction m_InteractType => enum_Interaction.Invalid;
    public virtual bool B_InteractOnTrigger => false;
    protected bool B_Interacted  = false;
    protected virtual bool B_InteractaOnce => false;
    protected virtual bool B_CanInteract(EntityPlayerBase _interactor) => true;
    protected virtual bool B_RecycleOnInteract => false;
    public virtual void Init()
    {

    }
    protected void Play()
    {
        B_Interacted = false;
    }
    public bool TryInteract(EntityPlayerBase _interactor)
    {
        B_Interacted = true;
        OnInteractSuccessful(_interactor);
        if (B_RecycleOnInteract)
            ObjectManager.RecycleInteract(m_InteractType,this);
        return true;
    }
    protected virtual void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {

    }
}
