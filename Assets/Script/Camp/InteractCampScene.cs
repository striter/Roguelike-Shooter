using System;
using GameSetting;

public class InteractCampScene : InteractCamp
{
    public override enum_Interaction m_InteractType => enum_Interaction.GameStart;
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        CampManager.Instance.OnSceneItemInteract();
        return false;
    }
}
