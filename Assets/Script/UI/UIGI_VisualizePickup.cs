using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_VisualizePickup : UIT_GridItem {
    Image m_Image;
    Text m_Amount, m_Projection;
    Action<int> OnAnimFinished;
    Vector3 m_pickupPos;
    TSpecialClasses.AnimationControlBase m_Animation;
    public override void Init()
    {
        base.Init();
        m_Animation = new TSpecialClasses.AnimationControlBase(transform.GetComponent<Animation>(), true);
        m_Amount = tf_Container.Find("Amount").GetComponent<Text>();
        m_Projection = tf_Container.Find("Projection").GetComponent<Text>();
        m_Image = tf_Container.Find("Image").GetComponent<Image>();
    }

    public void Play(Vector3 position,enum_Interaction interaction, int amount, Action<int> _OnAnimFinished)
    {
        m_Animation.Play(true);
        m_pickupPos = position;
        rtf_RectTransform.SetWorldViewPortAnchor(m_pickupPos, CameraController.MainCamera);
        m_Image.sprite = GameUIManager.Instance.m_InGameSprites[interaction.GetNumericVisualizeIcon()];
        m_Amount.color = interaction.GetVisualizeAmountColor();

        string text =string.Format("+{0}", amount);
        m_Amount.text = text;
        m_Projection.text = text;
        OnAnimFinished = _OnAnimFinished;
    }

    void OnAnimFinish()
    {
        OnAnimFinished(m_Index);
    }

    private void Update()
    {
        rtf_RectTransform.SetWorldViewPortAnchor(m_pickupPos, CameraController.MainCamera,.1f);
    }
}
