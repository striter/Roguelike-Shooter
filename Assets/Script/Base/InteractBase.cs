using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(HitCheckInteract))]
public class InteractBase : MonoBehaviour
{
    public virtual enum_Interaction m_InteractType => enum_Interaction.Invalid;
    public virtual bool B_InteractOnTrigger => false;
    protected HitCheckInteract m_InteractCheck { get; private set; }
    protected virtual bool E_InteractOnEnable => true;
    public bool m_InteractEnable { get; private set; } = true;


    protected virtual void Play()
    {
        m_InteractCheck = GetComponent<HitCheckInteract>();
        m_InteractCheck.Init();
        SetInteractable(E_InteractOnEnable);
        //Debug.Log("创建" + m_InteractCheck.name);
    }

    protected virtual bool OnTryInteractCheck(EntityCharacterPlayer _interactor) => true;
    protected virtual bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor) => true;
    public bool TryInteract(EntityCharacterPlayer _interactor)
    {
        if (!OnTryInteractCheck(_interactor))
            return false;

        SetInteractable(OnInteractedContinousCheck(_interactor));
        return true;
    }

    public void SetInteractable(bool interactable)
    {
        m_InteractEnable = interactable;
        m_InteractCheck.SetEnable(interactable);
    }

    public virtual string GetUITitleKey() => m_InteractType.GetTitleLocalizeKey();
    public virtual string GetUIIntroKey() => m_InteractType.GetIntroLocalizeKey();
}
