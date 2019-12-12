using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_ActionExpireInfo : UIGI_ActionBase {
    UIT_TextExtend m_Duration;
    Image m_DurationFill,m_ActionType;
    ActionBase m_target;
    public override void Init()
    {
        base.Init();
        m_Duration = tf_Container.Find("Duration").GetComponent<UIT_TextExtend>();
        m_DurationFill = tf_Container.Find("DurationFill").GetComponent<Image>();
    }
    public override void SetInfo(ActionBase action)
    {
        base.SetInfo(action);
        m_target = action;
        m_Duration.text = "";
        m_DurationFill.fillAmount = 0;
        CheckFillAmount();
    }
    void CheckFillAmount()
    {
        if (m_target.m_ExpireDuration == 0)
            return;

        m_DurationFill.fillAmount = 1 - m_target.f_expireLeftScale;
        m_Duration.text = string.Format("{0:N1}s", m_target.f_expireCheck);
    }

    private void Update()
    {
        CheckFillAmount();
    }
}
