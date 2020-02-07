using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(HitCheckInteract))]
public class InteractBase : MonoBehaviour
{
    public int m_TradePrice = -1;
    public virtual enum_Interaction m_InteractType => enum_Interaction.Invalid;
    public virtual string m_ExternalLocalizeKeyJoint=>"";
    public virtual bool B_InteractOnTrigger => false;
    protected virtual bool B_CanInteract(EntityCharacterPlayer _interactor) => m_TradePrice<=0|| _interactor.m_CharacterInfo.CanCostCoins(m_TradePrice);
    public bool m_InteractEnable { get; private set; } = true;
    public void SetInteractable(bool interactable) => m_InteractEnable = interactable;
    protected virtual void Play()
    {
        m_TradePrice = -1;
        m_InteractEnable = true;
    }
    public virtual bool OnCheckResponse(EntityCharacterPlayer _interactTarget)
    {
        return m_InteractEnable;
    }
    protected virtual bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        if (m_TradePrice > 0)
            _interactor.m_CharacterInfo.OnCoinsCost(m_TradePrice);

        return true;
    }
    public virtual bool TryInteract(EntityCharacterPlayer _interactor)
    {
        if (!B_CanInteract(_interactor))
            return false;

        SetInteractable(OnInteractOnceCanKeepInteract(_interactor));
        return true;
    }
}
