using System;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(Collider))]
public class InteractDetector : MonoBehaviour {
    Action<InteractBase, bool> OnInteractCheck;
    public void Init(Action<InteractBase, bool> _OnInteractCheck)
    {
        OnInteractCheck = _OnInteractCheck;
        gameObject.layer = GameLayer.I_DynamicDetect;
    }
    private void OnTriggerEnter(Collider other)
    {
        InteractBase target = other.GetComponent<InteractBase>();
        if (target != null&&target.B_Interactable)
            OnInteractCheck(target, true);
    }
    private void OnTriggerExit(Collider other)
    {
        InteractBase target = other.GetComponent<InteractBase>();
        if (target != null && target.B_Interactable)
            OnInteractCheck(target, false);
    }
}
