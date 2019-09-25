using System;
using GameSetting;

public class CampInteractScene : CampInteract
{
    public override bool B_InteractOnce => true;
    public override enum_Interaction m_InteractType => enum_Interaction.CampStage;
    public enum_Scene m_TargetScene = enum_Scene.Invalid;

    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        CampManager.Instance.OnSceneItemInteract(m_TargetScene);
    }
}
