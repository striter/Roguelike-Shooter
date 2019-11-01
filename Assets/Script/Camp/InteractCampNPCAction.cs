using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampNPCAction : InteractCamp {
    public override enum_Interaction m_InteractType => enum_Interaction.CampAction;
    Transform tf_CameraPos, tf_CameraLookPos;
    public override void Init()
    {
        base.Init();
        tf_CameraPos = transform.Find("CameraAnim/CameraPos");
        tf_CameraLookPos = transform.Find("CameraAnim/CameraLookPos");
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        CampManager.Instance.OnActionNPCChatted(tf_CameraPos,tf_CameraLookPos);
    }
}
