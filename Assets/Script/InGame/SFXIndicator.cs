using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXIndicator : SFXParticles {
    public void PlayDuration(int sourceID, Vector3 origin, Vector3 normal, float duration)
    {
        transform.position = origin;
        transform.rotation = Quaternion.LookRotation(normal);
        base.Play(sourceID,duration);
    }
    public void PlayLandMark(int sourceID, Vector3 origin,Vector3 direction,float speed,float radius)
    {
        RaycastHit hit;
        Debug.DrawRay(origin, direction, Color.red, 5);
        if (Physics.SphereCast(origin, radius, direction, out hit,GameConst.I_ProjectileMaxDistance, GameLayer.Physics.I_Static))
        {
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(hit.normal);
            float distance = Vector3.Distance(hit.point,origin);
            float duration = distance / speed;
            Vector3 endPos = hit.point;
            base.Play(sourceID,duration);
        }
        else
        {
            OnPlayFinished();
        }
    }
}
