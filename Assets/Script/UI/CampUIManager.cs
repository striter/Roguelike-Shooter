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
        btn_Reload.SetActivate(false);

        ShowControls<UIC_CampStatus>();
        ShowControls<UIC_GamePlayerStatus>().SetInGame(false);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
    
    Action OnExitFarm;
    public UIC_FarmStatus BeginFarm(Action<bool,Vector2> _OnDragDown, Action<Vector2> _OnDrag)
    {
        m_TouchDelta.AddDragBinding(_OnDragDown, _OnDrag);
        tf_BaseControl.localScale = Vector3.zero;
        UIC_FarmStatus target = ShowControls<UIC_FarmStatus>();
        return target;
    }
    public void ExitFarm()
    {
        tf_BaseControl.localScale=Vector3.one;
        m_TouchDelta.RemoveExtraBinding();
        OnExitFarm?.Invoke();
    }
}
