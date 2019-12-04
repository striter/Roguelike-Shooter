using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ActionItemWeapon : UIGI_ActionItemBase {
    Transform m_Ready,m_Blank;
    Image m_Cooldown;
    Action OnClick;
    TSpecialClasses.ValueChecker<float, bool> m_AvailableCheck;
    public override void Init()
    {
        base.Init();
        m_Ready = tf_Container.Find("Ready");
        m_Blank = transform.Find("Blank");
        m_Cooldown = transform.Find("Cooldown").GetComponent<Image>();
        transform.Find("Button").GetComponent<Button>().onClick.AddListener(()=> { OnClick(); });
        m_AvailableCheck = new TSpecialClasses.ValueChecker<float, bool>(-1, false);
    }
    public void Play(ActionBase action,Action _OnClick)
    {
        OnClick = _OnClick;
        bool showAction = action != null;
        tf_Container.SetActivate(showAction);
        m_Blank.SetActivate(!showAction);
        m_Cooldown.SetActivate(showAction);
        m_Ready.SetActivate(false);
        if(showAction)
            base.SetInfo(action);
    }

    public void Tick(WeaponBase weapon)
    {
        if (weapon.m_WeaponAction == null)
            return;

        if (m_AvailableCheck.Check(weapon.m_ActionEnergyRequirementLeft, weapon.m_ActionAvailable))
        {
            m_Cooldown.fillAmount = m_AvailableCheck.check1;
            m_Ready.SetActivate(m_AvailableCheck.check2);
        }
    }
}
