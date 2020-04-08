using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampBillboard : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampBillboard;
    Transform m_CameraPos;
    protected override void Awake()
    {
        base.Awake();
        m_CameraPos = transform.Find("CameraPos");
    }
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        CampManager.Instance.OnBillboardInteract(m_CameraPos);
        return true;
    }
}
