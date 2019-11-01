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

        ShowTools<UIT_CampStatus>();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
    
    Action OnExitFarm;
    public UIT_FarmStatus BeginFarm(Action<bool,Vector2> _OnDragDown, Action<Vector2> _OnDrag, Action _OnBuyClick, Action _OnExitFarm,Action _OnProfitClick)
    {
        m_TouchDelta.AddDragBinding(_OnDragDown, _OnDrag);
        tf_Control.localScale = Vector3.zero;
        OnExitFarm = _OnExitFarm;
        UIT_FarmStatus target = ShowTools<UIT_FarmStatus>();
        target.Play(ExitFarm, _OnBuyClick, _OnProfitClick);
        return target;
    }
    void ExitFarm()
    {
        tf_Control.localScale=Vector3.one;
        m_TouchDelta.RemoveExtraBinding();
        OnExitFarm?.Invoke();
    }
}
