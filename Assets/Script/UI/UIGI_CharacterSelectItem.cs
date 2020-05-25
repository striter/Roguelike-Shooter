using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_CharacterSelectItem : UIT_GridItem,IGridHighlight {
    Image m_Selected,m_Normal;
    Image m_Image;
    Transform m_Equipping;
    Action<int> OnButtonClick;
    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Normal = rtf_Container.Find("Normal").GetComponent<Image>();
        m_Image = rtf_Container.Find("Image").GetComponent<Image>();
        m_Selected = rtf_Container.Find("Selected").GetComponent<Image>();
        m_Equipping = rtf_Container.Find("Equipping");
        GetComponent<Button>().onClick.AddListener(() => { OnButtonClick(m_Identity); });
        m_Image.material = new Material(m_Image.material);
    }

    public override void OnAddItem(int identity)
    {
        base.OnAddItem(identity);
        enum_PlayerCharacter character = (enum_PlayerCharacter)identity;
        m_Image.sprite = UIManager.Instance.m_CharacterSprites[character.GetIconSprite()];
        m_Equipping.SetActivate(GameDataManager.CheckCharacterEquipping(character));
        
        bool unlocked = GameDataManager.CheckCharacterUnlocked(character);
        Color colorAlpha = TCommon.ColorAlpha(Color.white, unlocked ? 1f : .6f);
        m_Normal.color = colorAlpha;
        m_Image.color = colorAlpha;
        m_Image.material.SetVector("_BSC", unlocked ?Vector4.one:new Vector4(.6f,0f,1f));
    }

    public void AttachSelectButton(Action<int> OnButtonClick)=>this.OnButtonClick = OnButtonClick;

    public void OnHighlight(bool highlight)
    {
        m_Selected.SetActivate(highlight);
        m_Normal.SetActivate(!highlight);
    }

}
