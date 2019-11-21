﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;

public class UI_WeaponStatus : UIPageBase {

    Transform tf_WeaponInfo;
    UIT_TextExtend m_WeaponName;
    Image m_WeaponImage;
    UIC_Numeric m_ClipSize;
    Image m_Damage, m_FireRate, m_Stability, m_ProjectileSpeed;
    UIT_TextExtend m_DamageAmount, m_FireRateAmount, m_StabilityAmount, m_ProjectileSpeedAmount;
    Transform tf_ActionInfo;
    UIT_TextExtend m_ActionName, m_ActionIntro,m_ActionRarityText;
    UIC_RarityLevel m_ActionRarity;

    protected override void Init()
    {
        base.Init();
        tf_WeaponInfo = tf_Container.Find("WeaponInfo");
        m_WeaponName = tf_WeaponInfo.Find("WeaponName").GetComponent<UIT_TextExtend>();
        m_WeaponImage = tf_WeaponInfo.Find("WeaponImage").GetComponent<Image>();
        m_ClipSize = new UIC_Numeric(tf_WeaponInfo.Find("ClipSize"));
        m_Damage = tf_WeaponInfo.Find("Damage/Fill").GetComponent<Image>();
        m_DamageAmount = tf_WeaponInfo.Find("Damage/Amount").GetComponent<UIT_TextExtend>();
        m_FireRate = tf_WeaponInfo.Find("FireRate/Fill").GetComponent<Image>();
        m_FireRateAmount = tf_WeaponInfo.Find("FireRate/Amount").GetComponent<UIT_TextExtend>();
        m_Stability = tf_WeaponInfo.Find("Stability/Fill").GetComponent<Image>();
        m_StabilityAmount = tf_WeaponInfo.Find("Stability/Amount").GetComponent<UIT_TextExtend>();
        m_ProjectileSpeed = tf_WeaponInfo.Find("ProjectileSpeed/Fill").GetComponent<Image>();
        m_ProjectileSpeedAmount = tf_WeaponInfo.Find("ProjectileSpeed/Amount").GetComponent<UIT_TextExtend>();

        tf_ActionInfo = tf_Container.Find("ActionInfo");
        m_ActionName = tf_ActionInfo.Find("ActionName").GetComponent<UIT_TextExtend>(); 
        m_ActionIntro = tf_ActionInfo.Find("ActionIntro").GetComponent<UIT_TextExtend>();
        m_ActionRarityText = tf_ActionInfo.Find("ActionRarityText").GetComponent<UIT_TextExtend>();
        m_ActionRarity = new UIC_RarityLevel(tf_ActionInfo.Find("ActionRarity"));
    }
    
    public void SetInfo(WeaponBase weapon)
    {
        m_WeaponImage.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_Weapon.GetSpriteName()];
        m_WeaponName.localizeKey = weapon.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();
        m_ClipSize.SetNumeric(string.Format("{0:D2}", weapon.I_ClipAmount));

        m_DamageAmount.text = string.Format("{0:N1}", weapon.m_WeaponInfo.m_UIDamage);
        m_Damage.fillAmount = UIExpression.GetUIWeaponDamageValue(weapon.m_WeaponInfo.m_UIDamage);
        m_StabilityAmount.text = string.Format("{0:N1}", weapon.m_WeaponInfo.m_UIStability);
        m_Stability.fillAmount = UIExpression.GetUIWeaponStabilityValue(weapon.m_WeaponInfo.m_UIStability);
        m_FireRateAmount.text = string.Format("{0:N1}", weapon.m_WeaponInfo.m_UIRPM);
        m_FireRate.fillAmount = UIExpression.GetUIWeaponRPMValue(weapon.m_WeaponInfo.m_UIRPM);
        m_ProjectileSpeedAmount.text = string.Format("{0:N1}", weapon.m_WeaponInfo.m_UISpeed);
        m_ProjectileSpeed.fillAmount = UIExpression.GetUIWeaponSpeedValue(weapon.m_WeaponInfo.m_UISpeed);

        bool showAction = weapon.m_WeaponAction != null;
        if (showAction)
        {
            ActionBase action = weapon.m_WeaponAction;
            m_ActionName.localizeKey = action.GetNameLocalizeKey();
            action.SetActionIntro(m_ActionIntro);
            m_ActionRarityText.localizeKey = action.m_rarity.GetLocalizeKey();
            m_ActionRarity.SetLevel(action.m_rarity);   
            return;
        }

        m_ActionName.localizeKey = "UI_WeaponStatus_ActionInvalidName";
        m_ActionIntro.localizeKey = "UI_WeaponStatus_ActionInvalidIntro";
        m_ActionRarityText.text = "";
        m_ActionRarity.SetLevel(enum_RarityLevel.Invalid);
    }
}
