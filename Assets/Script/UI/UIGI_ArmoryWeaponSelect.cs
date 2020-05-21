using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ArmoryWeaponSelect : UIT_GridItem{
    Transform m_Hightlight;
    Action<enum_PlayerWeaponIdentity> OnWeaponClick;
    UIC_WeaponName m_Name;
    UIT_GridControllerComponent<Image> m_ScoreGrid;
    Image m_Image;
    public SWeaponInfos m_WeaponInfo { get; private set; }

    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Hightlight = rtf_Container.Find("Highlight");
        m_Name = new UIC_WeaponName(rtf_Container.Find("Name"));
        m_Image = rtf_Container.Find("Image").GetComponent<Image>();
        m_ScoreGrid = new UIT_GridControllerComponent<Image>(rtf_Container.Find("ScoreGrid"));
        GetComponent<Button>().onClick.AddListener(()=> { OnWeaponClick((enum_PlayerWeaponIdentity)m_Identity); });
    }

    public void Play(bool unlocked, Action<enum_PlayerWeaponIdentity> OnWeaponClick)
    {
        this.OnWeaponClick = OnWeaponClick;
        SWeaponInfos weaponInfo = GameDataManager.GetWeaponProperties((enum_PlayerWeaponIdentity)m_Identity);

        m_ScoreGrid.ClearGrid();
        for (int i = 0; i < (int)weaponInfo.m_Rarity; i++)
            m_ScoreGrid.AddItem(i);
        m_Name.SetName(weaponInfo);
        m_Image.sprite = UIManager.Instance.m_WeaponSprites[weaponInfo.m_Weapon.GetSprite(unlocked)];
    }

    public void OnHighlight(bool highlight) => m_Hightlight.SetActivate(highlight);

}
