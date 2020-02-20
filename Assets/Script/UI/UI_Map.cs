using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using LevelSetting;
public class UI_Map : UIPage {
    Transform m_MapContainer;
    UIC_Map m_MapBase;
    protected override void Init()
    {
        base.Init();
        m_MapContainer = rtf_Container.Find("MapContainer");
        m_MapBase = new UIC_Map( m_MapContainer.Find("Map"));
    }
}
