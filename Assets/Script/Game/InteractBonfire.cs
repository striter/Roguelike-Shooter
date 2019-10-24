using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractBonfire : InteractBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Bonfire;
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor) => false;
    public new void Play()
    {
        base.Play();
    }
}
