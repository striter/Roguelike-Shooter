using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampUIManager : UIManager {
    protected override void Init()
    {
        base.Init();
        btn_Reload.SetActivate(false);
    }
}
