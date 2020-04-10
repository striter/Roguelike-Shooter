using System;
using GameSetting;

public class InteractCampGameEnter : InteractCampBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.CampGameEnter;
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        CampManager.Instance.OnStartGameInteract();
        return false;
    }
}
