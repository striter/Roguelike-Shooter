﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractEquipment : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Equipment;
    public EquipmenBase m_Equipment { get; private set; }
    protected override bool B_SelfRecycleOnInteract => false;

    public InteractEquipment Play(EquipmenBase _action)
    {
        base.Play();
        m_Equipment = _action;
        return this;
    }

    
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        if (!_interactTarget.m_CharacterInfo.b_haveEmptyEquipmentSlot)
        {
            if (!UIPageBase.m_PageOpening)
                GameUIManager.Instance.ShowPage<UI_EquipmentSwap>(true, 0f).Play(_interactTarget.m_CharacterInfo, m_Equipment, OnEquipmentSwapPage);
        }
        else
        {
            OnRecycle();
            _interactTarget.m_CharacterInfo.OnEquipmentAcquire(m_Equipment);
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
