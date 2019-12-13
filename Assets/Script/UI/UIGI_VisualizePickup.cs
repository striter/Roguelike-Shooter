using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_VisualizePickup : UIT_GridItem {
    Image m_Image;
    Text m_Amount, m_Projection;
    float f_expireCheck;
    Action<int> OnAnimFinished;
    RectTransform rtf_Container;
    Color m_Color;
    Vector3 m_pickupPos;
    public override void Init()
    {
        base.Init();
        rtf_Container = tf_Container as RectTransform;
        m_Amount = tf_Container.Find("Amount").GetComponent<Text>();
        m_Projection = tf_Container.Find("Projection").GetComponent<Text>();
        m_Image = tf_Container.Find("Image").GetComponent<Image>();
    }

    public void Play(Vector3 position,enum_Interaction interaction, int amount, Action<int> _OnAnimFinished)
    {
        m_pickupPos = position;
        rtf_RectTransform.SetWorldViewPortAnchor(m_pickupPos, CameraController.MainCamera);
        m_Image.sprite = GameUIManager.Instance.m_InGameSprites[interaction.GetPickupSpriteName()];
        m_Color = interaction.GetVisualizeAmountColor();
        m_Amount.color = m_Color;

        string integer = amount.ToString();
        m_Amount.text = integer;
        m_Projection.text = integer;
        f_expireCheck = 1f;
        OnAnimFinished = _OnAnimFinished;
    }

    private void Update()
    {
        rtf_RectTransform.SetWorldViewPortAnchor(m_pickupPos, CameraController.MainCamera,.1f);
        f_expireCheck -= Time.deltaTime;
        if (f_expireCheck < 0)
            OnAnimFinished(I_Index);
        rtf_Container.anchoredPosition = Vector2.Lerp(new Vector2(0, 100), Vector2.zero, f_expireCheck);
        m_Amount.color = TCommon.ColorAlpha(m_Color, Mathf.Lerp(0, 1, f_expireCheck));
    }
}
