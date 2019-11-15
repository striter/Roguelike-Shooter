using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(HitCheckInteract))]
public class InteractBase : MonoBehaviour
{
    public virtual enum_Interaction m_InteractType => enum_Interaction.Invalid;
    public virtual string m_ExternalLocalizeKeyJoint=>"";
    public virtual bool B_InteractOnTrigger => false;
    protected virtual bool B_CanInteract(EntityCharacterPlayer _interactor) => true;
    public virtual bool B_InteractOnce { get; private set; } = true;
    public bool B_InteractEnable { get; private set; } = true;
    public void SetInteractable(bool interactable) => B_InteractEnable = interactable;
    protected virtual void Play()
    {
        B_InteractEnable = true;
    }
    public virtual bool TryInteract(EntityCharacterPlayer _interactor)
    {
        if (!B_CanInteract(_interactor))
            return false;

        OnInteractSuccessful(_interactor);
        return true;
    }
    protected virtual void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        if (B_InteractOnce)
            B_InteractEnable = false;
    }
}
