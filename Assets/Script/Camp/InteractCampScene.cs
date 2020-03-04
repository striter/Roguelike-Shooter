using System;
using GameSetting;

public class InteractCampScene : InteractCamp
{
    public override enum_Interaction m_InteractType => enum_Interaction.CampStage;
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        CampManager.Instance.OnSceneItemInteract();
        return false;
    }
}
