using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSpecialClasses;
using System;

public class UIGI_FarmProfit : UIT_GridItem {
    AnimationClipControl m_Animation;
    Action<int> OnAnimFinished;
    UIT_TextExtend m_Text;
    public override void Init()
    {
        base.Init();
        m_Animation = new AnimationClipControl(transform.GetComponent<Animation>());
        m_Text = rtf_Container.Find("Amount").GetComponent<UIT_TextExtend>();
    }

    public void Play(Vector3 pos,float profit,Action<int> _OnAnimFinished)
    {
        rectTransform.SetWorldViewPortAnchor(pos,CameraController.Instance.m_Camera,1f);
        m_Text.text = profit.ToString();
        m_Animation.Play(true);
        OnAnimFinished = _OnAnimFinished;
    }

    void OnAnimationFinish() => OnAnimFinished?.Invoke(m_Index);
}
