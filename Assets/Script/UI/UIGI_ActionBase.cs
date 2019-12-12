using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ActionBase : UIT_GridItem {
    Image m_Image;
    UIC_RarityLevel m_Rarity;
    public override void Init()
    {
        base.Init();
        m_Image = tf_Container.Find("Mask/Image").GetComponent<Image>();
        m_Rarity = new UIC_RarityLevel(tf_Container.Find("Rarity"));
    }
    public virtual void SetInfo(ActionBase action)
    {
        m_Image.sprite = GameUIManager.Instance.m_ActionSprites[action.m_Index.ToString()];
        m_Rarity.SetRarity(action.m_rarity);
    }
}
