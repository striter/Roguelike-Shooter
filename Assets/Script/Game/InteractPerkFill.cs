using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkFill : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.PerkFill;
    public List<int> m_PerkIDs;
    public InteractPerkFill Play(List<int> perkIDs)
    {
        base.Play();
        m_PerkIDs = perkIDs;
        return this;
    }
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        m_PerkIDs.Traversal((int perkID) => { _interactor.m_CharacterInfo.OnActionPerkAcquire(perkID); });
        return false;
    }
}
