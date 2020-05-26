using System;
using GameSetting;

public class InteractCampBattleEnter : InteractCampBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.CampBattleEnter;
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        CampManager.Instance.OnBattleStart(true);
        return false;
    }
}
