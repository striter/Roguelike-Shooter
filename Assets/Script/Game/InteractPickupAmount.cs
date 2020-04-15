using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

public class InteractPickupAmount : InteractPickup
{
    public override bool B_InteractOnTrigger => true;
    public int m_Amount { get; private set; }
    public virtual InteractPickupAmount Play(int amount)
    {
        base.Play();
        m_Amount = amount;
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        _interactor.OnInteractPickup(this,m_Amount);
        return false;
    }

}
