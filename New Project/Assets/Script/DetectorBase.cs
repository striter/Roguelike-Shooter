using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DetectorBase<T> : MonoBehaviour where T:MonoBehaviour
{
    protected List<T> l_targets = new List<T>();
    protected virtual void Awake()
    {
        Collider c_current = GetComponent<Collider>();
        c_current.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        T target = GetDetectTarget(other);
        if (target != null && !l_targets.Contains(target))
        {
            l_targets.Add(target);
            OnDetectChanged();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        T target = GetDetectTarget(other);
        if (target != null && l_targets.Contains(target))
        {
            l_targets.Remove(target);
            OnDetectChanged();
        }
    }
    protected virtual void OnDetectChanged()
    {

    }
    protected virtual T GetDetectTarget(Collider other)
    {
        Debug.LogError("Override This Please");
        return null;
    }
}
