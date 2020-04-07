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
}
