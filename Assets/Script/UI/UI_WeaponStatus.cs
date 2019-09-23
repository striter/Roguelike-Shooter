using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;

public class UI_WeaponStatus : UIPageBase {
    Transform tf_StatusContainer;

    Transform tf_WeaponInfo;
    UIT_TextLocalization m_WeaponName;
    Image m_WeaponImage;
    UIC_Numeric m_ClipSize;
    Image m_Damage, m_FireRate, m_Stability, m_ProjectileSpeed;

    Transform tf_ActionInfo;
    UIT_TextLocalization m_ActionName, m_ActionIntro,m_ActionRarityText;
    UIC_RarityLevel m_ActionRarity;

    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        tf_StatusContainer = tf_Container.Find("StatusContainer");

        tf_WeaponInfo = tf_StatusContainer.Find("WeaponInfo");
        m_WeaponName = tf_WeaponInfo.Find("WeaponName").GetComponent<UIT_TextLocalization>();
        m_WeaponImage = tf_WeaponInfo.Find("WeaponImage").GetComponent<Image>();
        m_ClipSize = new UIC_Numeric(tf_WeaponInfo.Find("ClipSize"));
        m_Damage = tf_WeaponInfo.Find("Damage/Fill").GetComponent<Image>();
        m_FireRate = tf_WeaponInfo.Find("FireRate/Fill").GetComponent<Image>();
        m_Stability = tf_WeaponInfo.Find("Stability/Fill").GetComponent<Image>();
        m_ProjectileSpeed = tf_WeaponInfo.Find("ProjectileSpeed/Fill").GetComponent<Image>();

        tf_ActionInfo = tf_StatusContainer.Find("ActionInfo");
        m_ActionName = tf_ActionInfo.Find("ActionName").GetComponent<UIT_TextLocalization>(); 
        m_ActionIntro = tf_ActionInfo.Find("ActionIntro").GetComponent<UIT_TextLocalization>();
        m_ActionRarityText = tf_ActionInfo.Find("ActionRarityText").GetComponent<UIT_TextLocalization>();
        m_ActionRarity = new UIC_RarityLevel(tf_ActionInfo.Find("ActionRarity"));
    }
    public void SetInfo(WeaponBase weapon)
    {
        m_WeaponName.localizeText = weapon.m_WeaponInfo.m_Weapon.GetNameLocalizeKey();
        m_ClipSize.SetNumeric(string.Format("{0:D2}", weapon.I_ClipAmount));
        m_Damage.fillAmount = UIExpression.F_WeaponDamageValue(weapon.F_BaseDamage);
        m_FireRate.fillAmount = UIExpression.F_WeaponFireRateValue(weapon.F_BaseFirerate);
        m_ProjectileSpeed.fillAmount = UIExpression.F_WeaponProjectileSpeedValue(weapon.F_BaseSpeed);
        m_Stability.fillAmount = UIExpression.F_WeaponStabilityValue(weapon.F_Recoil);

        bool showAction = weapon.m_WeaponAction.Count > 0;
        tf_ActionInfo.SetActivate(showAction);
        if (showAction)
        {
            ActionBase action = weapon.m_WeaponAction[0];
            m_ActionName.localizeText = action.GetNameLocalizeKey();
            m_ActionIntro.formatText(action.GetIntroLocalizeKey(), action.F_Duration, action.Value1, action.Value2, action.Value3);
            m_ActionRarityText.localizeText = action.m_rarity.GetLocalizeKey();
            m_ActionRarity.SetLevel(action.m_rarity);
        }
    }
}
