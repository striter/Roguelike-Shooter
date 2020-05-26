using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkLottery : InteractBattleBase {
    public override enum_Interaction m_InteractType => enum_Interaction.PerkLottery;
    int m_perkID;
    public InteractPerkLottery Play(float price,int perkID)
    {
        base.Play();
        m_perkID = perkID;
        SetTradePrice(price);
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        GameObjectManager.SpawnInteract<InteractPerkPickup>(transform.position, Quaternion.identity).Play(m_perkID).PlayDropAnim(NavigationManager.NavMeshPosition(transform.position+TCommon.RandomXZCircle()*4f));
        return false;
    }

}
