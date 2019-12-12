using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ActionControlInfo : UIGI_ActionBase {
    UIT_TextExtend m_Name;
    Image m_Fill;
    Action OnClick;
    public override void Init()
    {
        base.Init();
        m_Name = tf_Container.Find("Name").GetComponent<UIT_TextExtend>();
        m_Fill = tf_Container.Find("Fill").GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(()=> { OnClick(); });
    }

    public void SetInfo(WeaponBase weapon,Action _OnClick)
    {
        OnClick = _OnClick;
        if (weapon==null||weapon.m_WeaponAction == null)
            return;
        m_Name.localizeKey = weapon.m_WeaponAction.GetNameLocalizeKey();
    }

    public void Tick(float fill)
    {
        m_Fill.fillAmount = fill;
    }
}
