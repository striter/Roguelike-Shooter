using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractActionAdjustment : InteractBase {
    public override enum_Interaction m_InteractType => enum_Interaction.ActionAdjustment;
    public enum_StageLevel m_stage { get; private set; }
    public int m_upgradeCount { get; private set; }
    public int m_removeCount{get;private set;}
    public PlayerInfoManager m_Interactor { get; private set; }
    public override bool B_InteractOnce => false;
    public void Play(enum_StageLevel _stage)
    {
        m_stage = _stage;
        m_upgradeCount = 0;
        m_removeCount = 0;
    }

    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        UI_ActionAdjustment adjustment = UIManager.Instance.ShowPage<UI_ActionAdjustment>(true);
        if (adjustment == null)
            return;
        m_Interactor = _interactTarget.m_PlayerInfo;
        adjustment.Play( this);
    }

    public void OnRemovalAction(int index)
    {
        m_Interactor.RemoveStoredAction(index);
        m_Interactor.OnCoinsRemoval(RemovePrice);
        m_removeCount+=1;
    }
    public void OnUpgradeAction(int index)
    {
        m_Interactor.UpgradeStoredAction(index);
        m_Interactor.OnCoinsRemoval(UpgradePrice);
        m_upgradeCount += 1;
    }
    public enum_UI_ActionUpgradeType E_UpgradeType(int index)
    {
        if (m_Interactor.m_Coins <= UpgradePrice)
            return enum_UI_ActionUpgradeType.LackOfCoins;
        else if (!m_Interactor.m_ActionStored[index].B_Upgradable)
            return enum_UI_ActionUpgradeType.MaxLevel;

        return enum_UI_ActionUpgradeType.Upgradeable;
    }
    public bool B_Removeable(int index) => m_Interactor.m_Coins >= RemovePrice;
    public int UpgradePrice => GameExpression.GetActionUpgradePrice(m_stage, m_upgradeCount);
    public int RemovePrice => GameExpression.GetActionRemovePrice(m_stage,m_removeCount);
}