using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(HitCheckInteract))]
public class InteractBase : MonoBehaviour {
    public virtual enum_Interaction m_InteractType => enum_Interaction.Invalid;
    public virtual bool B_InteractOnTrigger => false;
    protected virtual bool B_CanInteract(EntityCharacterPlayer _interactor) => true;
    protected virtual bool B_RecycleOnInteract => false;
    public bool B_Interactable { get; private set; } = true;
    public virtual bool B_InteractOnce { get; private set; } = true;
    public void SetInteractable(bool interactable) => B_Interactable = interactable;
    public virtual void Init()
    {

    }
    protected void Play()
    {
        B_Interactable = true;
    }
    public virtual bool TryInteract(EntityCharacterPlayer _interactor)
    {
        if (!B_CanInteract(_interactor))
            return false;

        OnInteractSuccessful(_interactor);
        if (B_InteractOnce)
            B_Interactable = false;
        if (B_RecycleOnInteract)
            GameObjectManager.RecycleInteract( this);
        return true;
    }
    protected virtual void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
    }
}
