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
        ShowControls<UIC_CurrencyStatus>();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
    
    public UIC_FarmStatus BeginFarm(Action<bool,Vector2> _OnDragDown, Action<Vector2> _OnDrag,Action _OnExit)
    {
        m_CharacterControl.AddDragBinding(_OnDragDown, _OnDrag);
        OverrideSetting(_OnExit);
        return ShowControls<UIC_FarmStatus>();
    }
    public void ExitFarm()
    {
        m_CharacterControl.RemoveDragBinding();
        OverrideSetting(null);
    }
}
