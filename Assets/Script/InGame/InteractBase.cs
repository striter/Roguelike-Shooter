﻿using System.Collections;
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
    protected bool B_Interacted = false;
    public void SetInteractable(bool interactable) => B_Interactable = interactable;
    public virtual void Init()
    {

    }
    protected void Play()
    {
        B_Interactable = true;
        B_Interacted = false;
    }
    public virtual bool TryInteract(EntityCharacterPlayer _interactor)
    {
        if (!B_CanInteract(_interactor))
            return false;

        OnInteractSuccessful(_interactor);
        B_Interacted = true;
        if (B_RecycleOnInteract)
            GameObjectManager.RecycleInteract( this);
        return true;
    }
    protected virtual void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
    }
}
