using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_ExpireInfoItem : UIT_GridItem {
    UIT_TextExtend m_Duration;
    Image m_Image,m_DurationFill;
    UIC_RarityLevel_BG m_Rarity;
    ActionBase m_target;
    public override void Init()
    {
        base.Init();
        m_Duration = tf_Container.Find("Duration").GetComponent<UIT_TextExtend>();
        m_DurationFill = tf_Container.Find("DurationFill").GetComponent<Image>();
        m_Rarity = new UIC_RarityLevel_BG(tf_Container.Find("Rarity"));
        m_Image = tf_Container.Find("Mask/Image").GetComponent<Image>();
    }
    public void SetInfo(ActionBase action)
    {
        m_target = action;
        m_Duration.text = "";
        m_Rarity.SetRarity(action.m_rarity);
        m_Image.sprite = GameUIManager.Instance.m_ActionSprites[action.m_Index.ToString()];
        m_DurationFill.fillAmount = 0;
    }
    private void Update()
    {
        if (m_target != null && m_target.m_ExpireDuration != 0)
        {
            m_DurationFill.fillAmount = m_target.f_expireScale;
            m_Duration.text = string.Format("{0:N2}s", m_target.f_expireCheck);
        }
    }
}
