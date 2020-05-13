using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastMeleeKatana : WeaponCastMelee {

    protected override void OnStoreTrigger(bool success)
    {
        if(success)
        {
        }
        base.OnStoreTrigger(success);
    }
}
