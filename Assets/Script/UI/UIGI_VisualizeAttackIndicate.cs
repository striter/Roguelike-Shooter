using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGI_VisualizeAttackIndicate : UIT_GridItem {
    
    Action<int> OnAnimFinished;
    Transform m_target;
    TSpecialClasses.AnimationClipControl m_Animation;
    public override void Init()
    {
        base.Init();
        m_Animation = new TSpecialClasses.AnimationClipControl(transform.GetComponent<Animation>(), true);
        rtf_RectTransform.anchoredPosition = UIConst.V2_UINumericVisualizeOffset;
    }

    public void Play(Transform target, Action<int> _OnAnimFinished)
    {
        m_Animation.Play(true);
        m_target = target;
        rtf_RectTransform.SetWorldViewPortAnchor(m_target.position, CameraController.MainCamera);
        OnAnimFinished = _OnAnimFinished;
    }

    void OnAnimFinish()
    {
        OnAnimFinished(m_Index);
    }

    private void Update()
    {
        rtf_RectTransform.SetWorldViewPortAnchor(m_target.position, CameraController.MainCamera, .1f);
    }

}
