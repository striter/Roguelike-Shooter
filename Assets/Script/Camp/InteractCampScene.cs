using System;
using GameSetting;

public class InteractCampScene : InteractCamp
{
    public override bool B_InteractOnce => true;
    public override enum_Interaction m_InteractType => enum_Interaction.CampStage;
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        CampManager.Instance.OnSceneItemInteract();
    }
}
