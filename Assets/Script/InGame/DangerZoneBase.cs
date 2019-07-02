using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class DangerZoneBase : MonoBehaviour {
    List<HitCheckEntity> m_Targets=new List<HitCheckEntity>();
    protected virtual void OnHitCheckEntity(HitCheckEntity entity,bool enter)
    {
        if(enter)
            m_Targets.Add(entity);
        else
            m_Targets.Remove(entity);
    }
    private void OnTriggerEnter(Collider other)
    {
        HitCheckEntity entity = other.DetectEntity();
        if (entity != null) OnHitCheckEntity(entity,true);
    }
    private void OnTriggerExit(Collider other)
    {
        HitCheckEntity entity = other.DetectEntity();
        if (entity != null) OnHitCheckEntity(entity,false);
    }
}
