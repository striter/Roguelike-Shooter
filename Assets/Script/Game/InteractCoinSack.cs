using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCoinSack : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CoinSack;
    int m_CoinCount;
    public InteractCoinSack Play( int coinCount)
    {
        base.Play();
        m_CoinCount = coinCount;
        return this;
    }
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        GameObjectManager.SpawnInteract<InteractPickupCoin>(transform.position, transform.rotation).Play(m_CoinCount).PlayDropAnim(transform.position + TCommon.RandomXZCircle() * 4f).PlayMoveAnim(_interactor.transform);
        return false;
    }
}
