using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIT_CampStatus : UIToolsBase {
    Text m_Credit;

    Transform tf_Farm;
    Button m_Buy, m_Exit;
    protected override void Init()
    {
        base.Init();
        m_Credit = transform.Find("Credit").GetComponent<Text>();
        tf_Farm = transform.Find("Farm");
        m_Buy = tf_Farm.Find("Buy").GetComponent<Button>();
    }

    void OnBuyClicked()
    {
    }
    void OnExitClicked()
    {
    }

    public void SetInFarm(bool inFarm)
    {
        tf_Farm.SetActivate(inFarm);
    }
}
