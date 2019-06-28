using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXIndicator : SFXBase,ISingleCoroutine {
    Projector m_Indicator;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Indicator =GetComponent<Projector>();
        m_Indicator.orthographic = true;
        m_Indicator.enabled = false;
    }
    public void Play(int sourceID, Vector3 origin,Vector3 destination,float speed,float radius)
    {
        float distance = Vector3.Distance(origin, destination)+ radius;
        Vector3 direction = destination - origin;
        RaycastHit hit;
        Debug.DrawRay(origin, direction, Color.red, 5);
        if (Physics.SphereCast(origin, radius/2, direction, out hit, distance, GameLayer.Physics.I_Static))
        {
            transform.position = hit.point;
            distance = Vector3.Distance(hit.point,origin);
            float duration = distance / speed;
            Debug.Log(duration);
            Vector3 endPos = hit.point;
            m_Indicator.enabled = true;
            m_Indicator.orthographicSize = radius;
            m_Indicator.transform.position = hit.point + hit.normal * radius/2;
            m_Indicator.transform.rotation = Quaternion.LookRotation(direction);
            m_Indicator.farClipPlane = radius;
            this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>{
                m_Indicator.orthographicSize = radius * value;
            }, 0, 1, duration));
            base.PlaySFX(sourceID, duration);
        }
        else
        {
            OnPlayFinished();
        }
    }
    protected void OnDisable()
    {
        this.StopSingleCoroutine(0);
    }
}
