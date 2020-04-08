using System;
using GameSetting;

public class InteractCampGameEnter : InteractCampBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.CampGameEnter;
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        CampManager.Instance.OnGameSceneInteract();
        return false;
    }
}
