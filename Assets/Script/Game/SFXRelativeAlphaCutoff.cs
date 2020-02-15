using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXRelativeAlphaCutoff : SFXRelativeBase,ICoroutineHelper {
    Material m_Material;
    public float F_StartCutoff;
    public float F_EndCutoff;
    public float F_Time;
    int HS_Cutoff = Shader.PropertyToID("_CutOff");
    public override void Init()
    {
        base.Init();
        m_Material = GetComponent<MeshRenderer>().material;
        m_Material.SetFloat(HS_Cutoff, F_StartCutoff);
    }
    public override void OnPlay()
    {
        base.OnPlay();
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>{
            m_Material.SetFloat(HS_Cutoff,value);
        }, F_StartCutoff, F_EndCutoff, F_Time));
    }
    public override void OnStop()
    {
        base.OnStop();
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
            m_Material.SetFloat(HS_Cutoff, value);
        }, F_EndCutoff, F_StartCutoff, F_Time));
    }
    public override void OnRecycle()
    {
        base.OnRecycle();
        this.StopSingleCoroutine(0);
    }
}
