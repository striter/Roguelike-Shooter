using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractEquipment : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Equipment;
    public EquipmentBase m_Equipment { get; private set; }
    protected override bool B_SelfRecycleOnInteract => false;

    public InteractEquipment Play(EquipmentBase _action)
    {
        base.Play();
        m_Equipment = _action;
        return this;
    }


    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        if (!_interactor.m_CharacterInfo.b_haveEmptyEquipmentSlot)
        {
            if (!UIPageBase.m_PageOpening)
                GameUIManager.Instance.ShowPage<UI_EquipmentSwap>(true, 0f).Play(_interactor.m_CharacterInfo, m_Equipment, OnEquipmentSwapPage);
        }
        else
        {
            OnRecycle();
            _interactor.m_CharacterInfo.OnEquipmentAcquire(m_Equipment);
        }
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
