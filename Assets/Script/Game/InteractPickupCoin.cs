﻿using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupCoin : InteractPickupAmount {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupCoin;
}
