using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractActionAdjustment : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.ActionAdjustment;
    public enum_StageLevel m_stage { get; private set; }
    public int m_upgradeCount { get; private set; }
    public int m_removeCount{get;private set;}
    public PlayerInfoManager m_Interactor { get; private set; }
    public void Play(enum_StageLevel _stage)
    {
        m_stage = _stage;
        m_upgradeCount = 0;
        m_removeCount = 0;
    }

    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        if (UIPageBase.m_PageOpening)
            return true;
        m_Interactor = _interactTarget.m_PlayerInfo;
        GameUIManager.Instance.ShowCoinsPage<UI_ActionAdjustment>(true, 1f).Play( this);
        return true;
    }

    public void OnRemovalEquipment(int index)
    {
        m_Interactor.RemoveEquipment(index);
        m_Interactor.OnCoinsRemoval(RemovePrice);
        m_removeCount+=1;
    }

    public void OnUpgradeEquipment(int index)
    {
        m_Interactor.UpgradeEquipment(index);
        m_Interactor.OnCoinsRemoval(UpgradePrice);
        m_upgradeCount += 1;
    }

    public enum_UI_ActionUpgradeType E_UpgradeType(int index)
    {
        if (m_Interactor.m_Coins < UpgradePrice)
            return enum_UI_ActionUpgradeType.LackOfCoins;
        else if (!m_Interactor.m_ActionEquipment[index].B_Upgradable)
            return enum_UI_ActionUpgradeType.MaxLevel;

        return enum_UI_ActionUpgradeType.Upgradeable;
    }
    public bool B_Removeable(int index) => m_Interactor.m_Coins >= RemovePrice;
    public int UpgradePrice => GameExpression.GetActionUpgradePrice(m_stage, m_upgradeCount);
    public int RemovePrice => GameExpression.GetActionRemovePrice(m_stage,m_removeCount);
}