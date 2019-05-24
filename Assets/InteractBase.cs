using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractBase : MonoBehaviour {
    public bool B_InteractOnEnter = false;
    public virtual string S_InteractKeyword => "";
    HitCheckDetect m_Detector;
    bool isEnter;
    protected virtual void Init()
    {
        m_Detector = new HitCheckDetect(null,null, OnEntityDetect,null);
    }
    private void OnTriggerEnter(Collider other)
    {
        isEnter = true;
        m_Detector.DoDetect(other);
    }
    private void OnTriggerExit(Collider other)
    {
        isEnter = false;
        m_Detector.DoDetect(other);
    }
    void OnEntityDetect(HitCheckEntity entity)
    {
        if (entity.m_Attacher.B_IsPlayer)       //Only Interact With Player
            (entity.m_Attacher as EntityPlayerBase).OnCheckInteractor(this,isEnter);
    }
    public virtual void TryInteract()
    {
        Debug.LogError("Override This Please");
    }
}
