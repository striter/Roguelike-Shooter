﻿using GameSetting;
using System;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_Damage : UIT_GridItem {
    Text m_Amount,m_Projection;
    float f_expireCheck;
    Action<int> OnAnimFinished;
    RectTransform rtf_Container,rtf_SubContainer;
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        rtf_Container = tf_Container.GetComponent<RectTransform>();
        rtf_SubContainer = tf_Container.Find("SubContainer").GetComponent<RectTransform>();
        m_Amount = rtf_SubContainer.Find("Amount").GetComponent<Text>();
        m_Projection = rtf_SubContainer.Find("Projection").GetComponent<Text>();
    }

    public void Play(EntityCharacterBase damageEntity,float amount,Action<int> _OnAnimFinished)
    {
        rtf_RectTransform.SetWorldViewPortAnchor(damageEntity.tf_Head.position, CameraController.MainCamera);
        rtf_SubContainer.anchoredPosition = new Vector2(0,80f)+TCommon.RandomVector2(UIConst.F_UIDamageStartOffset);
        string integer = Mathf.CeilToInt(amount).ToString();
        m_Amount.text = integer;
        m_Projection.text = integer;
        f_expireCheck = 1f;
        OnAnimFinished = _OnAnimFinished;
    }

    private void Update()
    {
        f_expireCheck -= Time.deltaTime;
        rtf_Container.anchoredPosition = Vector2.Lerp(new Vector2(0,100),Vector2.zero,f_expireCheck);
        m_Amount.color = Color.Lerp(TCommon.ColorAlpha(Color.red,0f),Color.red,f_expireCheck);
        if (f_expireCheck < 0)
            OnAnimFinished(I_Index);
    }
}
