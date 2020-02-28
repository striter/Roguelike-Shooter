using GameSetting;
using System;
using UnityEngine;
[RequireComponent(typeof(Collider),typeof(Rigidbody))]
public class EntityDetector : MonoBehaviour
{
    Collider m_Collider;
    Action<HitCheckEntity, bool> OnInteractCheck;
    public EntityDetector Init(Action<HitCheckEntity, bool> _OnInteractCheck)
    {
        m_Collider = GetComponent<Collider>();
           OnInteractCheck = _OnInteractCheck;
        gameObject.layer = GameLayer.I_EntityDetect;
        return this;
    }
    public void SetPlay(bool play)=> m_Collider.enabled = play;
    private void OnTriggerEnter(Collider other)
    {
        HitCheckEntity target = other.GetComponent<HitCheckEntity>();
        if (target != null)
            OnInteractCheck(target, true);
    }

    private void OnTriggerExit(Collider other)
    {
        HitCheckEntity target = other.GetComponent<HitCheckEntity>();
        if (target != null)
            OnInteractCheck(target, false);
    }

}
