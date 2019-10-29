using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCampNPC : InteractCamp {
    Transform tf_CameraPos,tf_CameraLookPos;
    Transform tf_Model;
    EntityCharacterPlayer m_Interactor;
    Vector3 v3_offset;

    public override void Init()
    {
        base.Init();
        tf_CameraPos = transform.Find("CameraPos");
        tf_CameraLookPos = transform.Find("CameraLookPos");
        tf_Model = transform.Find("Model");
    }

    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        m_Interactor = _interactTarget;
        CameraController.Instance.Attach(tf_CameraPos,OnAttachSuccessful);
        CameraController.Instance.CameraLookAt(tf_CameraLookPos);
        v3_offset = TPSCameraController.Instance.TPSOffset;
        TPSCameraController.Instance.SetOffset(Vector3.zero);
        GameManagerBase.Instance.SetEffect_Focal(true, tf_Model.transform, 2f, 2f);
    }

    protected virtual void OnAttachSuccessful()
    {
        base.OnInteractSuccessful(m_Interactor);
    }
}
