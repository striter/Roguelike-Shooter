using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkPickup : InteractPickup {
    public List<InteractPerkPickup> m_interactPerkPickupList =null;
    public override enum_Interaction m_InteractType => enum_Interaction.PerkPickup;
    public int m_PerkID { get; private set; }
    protected override bool B_SelfRecycleOnInteract => true;

    public InteractPerkPickup Play(int _perkID)
    {
        base.Play();
        m_PerkID = _perkID;
        m_interactPerkPickupList = null;
        ExpirePlayerPerkBase perk = GameDataManager.GetPlayerPerkData(_perkID);

        if (m_visualizeItem != null)
        {
            m_visualizeItem.Play(TLocalization.GetKeyLocalized(perk.GetNameLocalizeKey()),
            this);
        }
        else
        {
            Debug.Log("创建" + TLocalization.GetKeyLocalized(perk.GetNameLocalizeKey()));
            BattleUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().CreateItemInformation(TLocalization.GetKeyLocalized(perk.GetNameLocalizeKey()),
                    this);
        }

        return this;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    /// <param name="_interactor"></param>
    /// <returns></returns>
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        if (m_interactPerkPickupList == null)
        {
            base.OnInteractedContinousCheck(_interactor);
            if(_interactor!=null)
                _interactor.m_CharacterInfo.OnActionPerkAcquire(m_PerkID);
        }
        else
        {
            for (int i = 0; i < m_interactPerkPickupList.Count; i++)
            {
                if (m_interactPerkPickupList[i])
                {
                    m_interactPerkPickupList[i].m_interactPerkPickupList = null;
                    m_interactPerkPickupList[i].OnInteractedContinousCheck(null);
                }
            }
            base.OnInteractedContinousCheck(_interactor);
            _interactor.m_CharacterInfo.OnActionPerkAcquire(m_PerkID);
        }
        return false;
    }
}
