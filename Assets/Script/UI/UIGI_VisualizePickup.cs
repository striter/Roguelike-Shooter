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
    TSpecialClasses.AnimationClipControl m_Animation;
    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Animation = new TSpecialClasses.AnimationClipControl(transform.GetComponent<Animation>(), true);
        m_Amount = rtf_Container.Find("Amount").GetComponent<Text>();
        m_Projection = rtf_Container.Find("Projection").GetComponent<Text>();
        m_Image = rtf_Container.Find("Image").GetComponent<Image>();
        rtf_RectTransform.anchoredPosition = UIConst.V2_UINumericVisualizeOffset;
    }

    public void Play(InteractPickup pickup , Action<int> _OnAnimFinished)
    {
        m_Animation.Play(true);
        m_pickupPos = pickup.transform.position;
        rtf_RectTransform.SetWorldViewPortAnchor(m_pickupPos, CameraController.MainCamera);
        m_Image.sprite = BattleUIManager.Instance.m_InGameSprites[pickup.m_InteractType.GetNumericVisualizeIcon()];
        m_Amount.color = pickup.m_InteractType.GetVisualizeAmountColor();

        int amount = 1;
        switch(pickup.m_InteractType)
        {
            case enum_Interaction.PickupArmor:
            case enum_Interaction.PickupCoin:
            case enum_Interaction.PickupHealth:
                amount = (pickup as InteractPickupAmount).m_Amount;
                break;
        }

        string text =string.Format("+{0}", amount);
        m_Amount.text = text;
        m_Projection.text = text;
        OnAnimFinished = _OnAnimFinished;
    }

    void OnAnimFinish()
    {
        OnAnimFinished(m_Identity);
    }

    private void Update()
    {
        rtf_RectTransform.SetWorldViewPortAnchor(m_pickupPos, CameraController.MainCamera,.1f);
    }
}
