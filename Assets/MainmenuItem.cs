using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameSetting;

public class MainmenuItem : MonoBehaviour {
    public enum_Scene m_scene { get; private set; }
    Transform tf_Highlight;
    public void Init(enum_Scene _scene)
    {
        m_scene = _scene;
        tf_Highlight = transform.Find("Highlight");
        Highlight(false);
    }
    public void Highlight(bool highlight)
    {
        tf_Highlight.SetActivate(highlight);
    }
}
