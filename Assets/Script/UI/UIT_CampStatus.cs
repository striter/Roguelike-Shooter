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
        m_Buy.onClick.AddListener(OnFarmBuyClicked);
        m_Exit = tf_Farm.Find("Exit").GetComponent<Button>();
        m_Exit.onClick.AddListener(OnFarmExitClicked);
    }

    void OnFarmBuyClicked()
    {
    }
    void OnFarmExitClicked()
    {
    }

    public void SetInFarm(bool inFarm)
    {
        tf_Farm.SetActivate(inFarm);
    }
}
