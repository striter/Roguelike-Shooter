using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class DangerZoneBase : MonoBehaviour {
    List<HitCheckEntity> m_Targets=new List<HitCheckEntity>();
    HitCheckDetect m_Detect;
    protected bool b_IsTriggerEnter { get; private set; } = true;
    protected virtual void Awake()
    {
        gameObject.layer = GameLayer.I_EntityDetect;
        m_Detect = new HitCheckDetect(null, null,OnHitCheckEntity,null);
    }
    protected virtual void OnHitCheckEntity(HitCheckEntity entity)
    {
        if (b_IsTriggerEnter)
            m_Targets.Add(entity);
        else
            m_Targets.Remove(entity);
    }
    private void OnTriggerEnter(Collider other)
    {
        b_IsTriggerEnter = true;
        m_Detect.DoDetect(other);
    }
    private void OnTriggerExit(Collider other)
    {
        b_IsTriggerEnter = false;
        m_Detect.DoDetect(other);
    }
}
