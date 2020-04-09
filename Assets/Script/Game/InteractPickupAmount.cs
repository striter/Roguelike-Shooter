﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

public class InteractPickupAmount : InteractPickup {
    public int m_Amount { get; private set; }
    public virtual InteractPickupAmount Play(int amount,bool moveTowardsPlayer)
    {
        base.Play(moveTowardsPlayer);
        m_Amount = amount;
        return this;
    }

    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        _interactor.OnInteractPickup(this,m_Amount);
        return false;
    }

}
