using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;

public class UI_WeaponStatus : UIPage {

    Transform tf_WeaponInfo;
    UIT_TextExtend m_WeaponName;
    Image m_WeaponImage;
    Image  m_WeaponBackgroundShadow;
    UIT_TextExtend m_ClipSize;
    Transform tf_StatusInfo;
    Image m_Damage, m_FireRate, m_Stability, m_ProjectileSpeed;
    UIT_TextExtend m_DamageAmount, m_FireRateAmount, m_StabilityAmount, m_ProjectileSpeedAmount;
    Transform tf_ActionInfo;
    UIT_TextExtend m_ActionName, m_ActionIntro;
    UIC_RarityLevel m_ActionRarity;

    protected override void Init()
    {
        base.Init();
        tf_WeaponInfo = rtf_Container.Find("WeaponInfo");
        m_WeaponBackgroundShadow = tf_WeaponInfo.Find("Shadow").GetComponent<Image>();
        m_WeaponName = tf_WeaponInfo.Find("WeaponName").GetComponent<UIT_TextExtend>();
        m_WeaponImage = tf_WeaponInfo.Find("WeaponImage").GetComponent<Image>();
        m_ClipSize = tf_WeaponInfo.Find("ClipSize").GetComponent<UIT_TextExtend>();

        tf_StatusInfo = rtf_Container.Find("StatusInfo");
        m_Damage = tf_StatusInfo.Find("Damage/Fill").GetComponent<Image>();
        m_DamageAmount = tf_StatusInfo.Find("Damage/Amount").GetComponent<UIT_TextExtend>();
        m_FireRate = tf_StatusInfo.Find("FireRate/Fill").GetComponent<Image>();
        m_FireRateAmount = tf_StatusInfo.Find("FireRate/Amount").GetComponent<UIT_TextExtend>();
        m_Stability = tf_StatusInfo.Find("Stability/Fill").GetComponent<Image>();
        m_StabilityAmount = tf_StatusInfo.Find("Stability/Amount").GetComponent<UIT_TextExtend>();
        m_ProjectileSpeed = tf_StatusInfo.Find("ProjectileSpeed/Fill").GetComponent<Image>();
        m_ProjectileSpeedAmount = tf_StatusInfo.Find("ProjectileSpeed/Amount").GetComponent<UIT_TextExtend>();

        tf_ActionInfo = rtf_Container.Find("ActionInfo");
        m_ActionName = tf_ActionInfo.Find("ActionName").GetComponent<UIT_TextExtend>(); 
        m_ActionIntro = tf_ActionInfo.Find("ActionIntro").GetComponent<UIT_TextExtend>();
        m_ActionRarity = new UIC_RarityLevel(tf_ActionInfo.Find("ActionRarity"));
    }
    
    public void Play(SWeapon weapon,ExpirePlayerPerkBase weaponAction)
    {
        m_WeaponImage.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_Weapon.GetUIControlDetailSpriteName()];
        m_WeaponName.localizeKey = weapon.m_Weapon.GetLocalizeNameKey();
        m_WeaponName.color = TCommon.GetHexColor(weapon.m_Rarity.GetUITextColor());
        m_WeaponBackgroundShadow.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_Rarity.GetUIStatusShadowBackground()];
        m_ClipSize.text = string.Format("{0:D2}", weapon.m_ClipAmount);

        m_DamageAmount.text = string.Format("{0:N1}", weapon.m_UIDamage);
        m_Damage.fillAmount = UIExpression.GetUIWeaponDamageValue(weapon.m_UIDamage);
        m_StabilityAmount.text = string.Format("{0:N1}", weapon.m_UIStability);
        m_Stability.fillAmount = UIExpression.GetUIWeaponStabilityValue(weapon.m_UIStability);
        m_FireRateAmount.text = string.Format("{0:N1}", weapon.m_UIRPM);
        m_FireRate.fillAmount = UIExpression.GetUIWeaponRPMValue(weapon.m_UIRPM);
        m_ProjectileSpeedAmount.text = string.Format("{0:N1}", weapon.m_UISpeed);
        m_ProjectileSpeed.fillAmount = UIExpression.GetUIWeaponSpeedValue(weapon.m_UISpeed);

        bool showAction = weaponAction != null;
        m_ActionRarity.transform.SetActivate(showAction);
        if (showAction)
        {
            ExpirePlayerPerkBase action = weaponAction;
            m_ActionName.localizeKey = action.GetNameLocalizeKey();
            action.SetActionIntro(m_ActionIntro);
            m_ActionRarity.SetRarity(action.m_Rarity);   
            return;
        }

        m_ActionName.localizeKey = "UI_Weapon_ActionInvalidName";
        m_ActionIntro.localizeKey = "UI_Weapon_ActionInvalidIntro";
    }
}
