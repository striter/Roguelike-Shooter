using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(HitCheckInteract))]
public class InteractBase : MonoBehaviour
{
    public AudioClip AC_OnPlay, AC_OnInteract;
    public int I_MuzzleOnInteract;

    public virtual enum_Interaction m_InteractType => enum_Interaction.Invalid;
    public virtual bool B_InteractOnTrigger => false;
    protected virtual bool B_CanInteract(EntityCharacterPlayer _interactor) => true;
    protected virtual bool B_RecycleOnInteract => false;
    public virtual bool B_InteractOnce { get; private set; } = true;
    public bool B_InteractEnable { get; private set; } = true;
    public void SetInteractable(bool interactable) => B_InteractEnable = interactable;
    public virtual void Init()
    {

    }
    protected void Play()
    {
        B_InteractEnable = true;
        if (AC_OnPlay)
            GameAudioManager.Instance.PlayClip(-1,AC_OnPlay,false, transform.position);
    }
    public virtual bool TryInteract(EntityCharacterPlayer _interactor)
    {
        if (!B_CanInteract(_interactor))
            return false;

        OnInteractSuccessful(_interactor);
        if (B_InteractOnce)
            B_InteractEnable = false;
        if (B_RecycleOnInteract)
            GameObjectManager.RecycleInteract( this);
        return true;
    }
    protected virtual void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        GameObjectManager.PlayMuzzle(_interactTarget.m_EntityID,transform.position,transform.up,I_MuzzleOnInteract,AC_OnInteract);
    }
}
