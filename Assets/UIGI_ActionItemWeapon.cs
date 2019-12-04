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

    public override void Init()
    {
        base.Init();
        m_Ready = tf_Container.Find("Ready");
        m_Blank = transform.Find("Blank");
        m_Cooldown = transform.Find("Cooldown").GetComponent<Image>();
        transform.Find("Button").GetComponent<Button>().onClick.AddListener(()=> { OnClick(); });
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
        m_Cooldown.fillAmount = weapon.m_ActionEnergyRequirementLeft;
        Debug.Log(weapon.m_ActionEnergyRequirementLeft);
        m_Ready.SetActivate(weapon.m_ActionAvailable);
    }
}
