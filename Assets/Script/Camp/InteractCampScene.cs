using System;
using GameSetting;

public class InteractCampScene : InteractCamp
{
    public override enum_Interaction m_InteractType => enum_Interaction.CampStage;
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        CampManager.Instance.OnSceneItemInteract();
        return false;
    }
}
