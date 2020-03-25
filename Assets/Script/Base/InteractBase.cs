using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(HitCheckInteract))]
public class InteractBase : MonoBehaviour
{
    public virtual enum_Interaction m_InteractType => enum_Interaction.Invalid;
    public int m_TradePrice { get; protected set; } = -1;
    public virtual bool B_InteractOnTrigger => false;
    protected HitCheckInteract m_InteractCheck { get; private set; }
    protected virtual bool E_InteractOnEnable => true;
    public virtual bool DnCheckInteractResponse(EntityCharacterPlayer _interactTarget) => m_InteractEnable;
    protected virtual bool DoCheckInteractSuccessful(EntityCharacterPlayer _interactor) => m_TradePrice<=0|| _interactor.m_CharacterInfo.CanCostCoins(m_TradePrice);
    public bool m_InteractEnable { get; private set; } = true;

    protected virtual void Play()
    {
        m_TradePrice = -1;
        m_InteractCheck = GetComponent<HitCheckInteract>();
        m_InteractCheck.Init();
        SetInteractable(E_InteractOnEnable);
    }
    protected virtual bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        if (m_TradePrice > 0)
            _interactor.m_CharacterInfo.OnCoinsCost(m_TradePrice);

        return true;
    }
    public virtual bool TryInteract(EntityCharacterPlayer _interactor)
    {
        if (!DoCheckInteractSuccessful(_interactor))
            return false;

        SetInteractable(OnInteractOnceCanKeepInteract(_interactor));
        return true;
    }
    public void SetInteractable(bool interactable)
    {
        m_InteractEnable = interactable;
        m_InteractCheck.SetEnable(interactable);
    }

    public virtual string GetUITitleKey() => m_InteractType.GetTitleLocalizeKey();
    public virtual string GetUIIntroKey() => m_InteractType.GetIntroLocalizeKey();
    public virtual string GetUIBottomKey() => m_InteractType.GetBottomLocalizeKey();
}
