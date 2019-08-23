using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(HitCheckDynamic))]
public class InteractBase : MonoBehaviour {
    public virtual string S_InteractKeyword => "";
    public virtual void Init()
    {
    }
    public virtual bool TryInteract()
    {
        Debug.LogError("Override This Please");
        return false;
    }
}
