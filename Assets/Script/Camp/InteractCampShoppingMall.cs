using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampShoppingMall : InteractCampBase
{

    public override enum_Interaction m_InteractType => enum_Interaction.CampShoppingMall;
    Transform m_CameraPos;
    public override void Init()
    {
        base.Init();
        m_CameraPos = transform.Find("CameraPos");
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        CampManager.Instance.OnShoppingMallInteract(m_CameraPos);
        return true;
    }
}
