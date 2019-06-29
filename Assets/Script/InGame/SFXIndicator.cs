using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXIndicator : SFXParticles {
//    Projector m_Indicator;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        //m_Indicator =GetComponent<Projector>();
        //m_Indicator.orthographic = true;
        //m_Indicator.enabled = false;
    }
    public void Play(int sourceID, Vector3 origin,Vector3 direction,float speed,float radius)
    {
        RaycastHit hit;
        Debug.DrawRay(origin, direction, Color.red, 5);
        if (Physics.SphereCast(origin, radius, direction, out hit,GameConst.I_BarrageProjectileMaxDistance, GameLayer.Physics.I_Static))
        {
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(hit.normal);
            float distance = Vector3.Distance(hit.point,origin);
            float duration = distance / speed;
            Vector3 endPos = hit.point;
            //m_Indicator.enabled = true;
            //m_Indicator.orthographicSize = radius;
            //m_Indicator.transform.position = hit.point + hit.normal * radius/2;
            //m_Indicator.transform.rotation = Quaternion.LookRotation(direction);
            //m_Indicator.farClipPlane = radius;
            //m_Indicator.orthographicSize = radius ;
            base.Play(sourceID,duration);
        }
        else
        {
            OnPlayFinished();
        }
    }
}
