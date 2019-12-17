using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractAction : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Action;
    public ActionBase m_Action { get; private set; }
    protected override bool B_SelfRecycleOnInteract => false;
    Transform tf_PlayerEquipment, tf_WeaponAbility;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        tf_PlayerEquipment = transform.Find("Container/Model/PlayerEquipment");
        tf_WeaponAbility = transform.Find("Container/Model/WeaponAbility");
    }

    public InteractAction Play(ActionBase _action)
    {
        base.Play();
        m_Action = _action;
        tf_WeaponAbility.SetActivate(_action.m_ActionType == enum_ActionType.Ability);
        tf_PlayerEquipment.SetActivate(_action.m_ActionType == enum_ActionType.Equipment);
        return this;
    }

    
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        if (m_Action.m_ActionType == enum_ActionType.Equipment && !_interactTarget.m_PlayerInfo.b_haveEmptyEquipmentSlot)
        {
            if (!UIPageBase.m_PageOpening)
                GameUIManager.Instance.ShowPage<UI_EquipmentSwap>(true, 0f).Play(_interactTarget.m_PlayerInfo, m_Action, OnEquipmentSwapPage);
            return true;
        }
        OnRecycle();
        _interactTarget.OnActionInteract(m_Action);
        return false;
    }

    void OnEquipmentSwapPage(bool confirm)
    {
        if (confirm)
            OnRecycle();
        else
            SetInteractable(true);
    }
}
