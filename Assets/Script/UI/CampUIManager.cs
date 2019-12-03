using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampUIManager : UIManager {
    public static new CampUIManager Instance { get; private set; }
    protected override void Init()
    {
        base.Init();
        Instance = this;
    }
    protected override void InitGameControls(bool inGame)
    {
        base.InitGameControls(inGame);
        ShowControls<UIC_CurrencyStatus>(false);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
    
    public UIC_FarmStatus BeginFarm(Action<bool,Vector2> _OnDragDown, Action<Vector2> _OnDrag,Action _OnExit)
    {
        m_UIControl.AddDragBinding(_OnDragDown, _OnDrag);
        m_UIControl.OverrideSetting(_OnExit);
        return ShowControls<UIC_FarmStatus>();
    }
    public void ExitFarm()
    {
        m_UIControl.RemoveDragBinding();
        m_UIControl.OverrideSetting(null);
    }
}
