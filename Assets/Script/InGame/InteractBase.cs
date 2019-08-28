using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(HitCheckDynamic))]
public class InteractBase : MonoBehaviour {
    public virtual string S_InteractKeyword => "";
    public virtual bool B_InteractaOnce => false;
    public bool B_Interactable { get; private set; } = false;
    public virtual enum_Interaction m_InteractType => enum_Interaction.Invalid;
    public virtual void Init()
    {

    }
    protected void Play()
    {
        B_Interactable = true;
    }
    public virtual bool TryInteract(EntityPlayerBase _interactor)
    {
        if (B_InteractaOnce)
            B_Interactable = false;
        return true;
    }
}
