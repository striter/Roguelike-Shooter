using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkLottery : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.PerkLottery;
    int m_perkID;
    public void Play(int price,int perkID)
    {
        base.Play();
        m_perkID = perkID;
        m_TradePrice =price;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        GameObjectManager.SpawnInteract<InteractPerkPickup>(transform.position, Quaternion.identity).Play(m_perkID);
        return false;
    }

}
