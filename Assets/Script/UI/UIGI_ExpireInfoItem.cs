using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ExpireInfoItem : UIT_GridItem {
    Text txt_Name, txt_ElapsedTime;
    ExpireBase m_target;
    protected override void Init()
    {
        base.Init();
        txt_Name = tf_Container.Find("Name").GetComponent<Text>();
        txt_ElapsedTime = tf_Container.Find("Elapsed").GetComponent<Text>();
    }
    public void SetInfo(ExpireBase expire)
    {
        m_target = expire;
        if (expire.m_ExpireType == enum_ExpireType.Action)
            txt_Name.text = (expire as ActionBase).GetNameLocalizeKey();
        else if (expire.m_ExpireType == enum_ExpireType.Buff)
            txt_Name.text = (expire as BuffBase).GetNameLocalizeKey();
        txt_ElapsedTime.text = "";
    }
    private void Update()
    {
        if (m_target!=null&& m_target.m_ExpireDuration != 0)
        {
            txt_ElapsedTime.text = m_target.f_expireCheck.ToString();
        }
    }
}
