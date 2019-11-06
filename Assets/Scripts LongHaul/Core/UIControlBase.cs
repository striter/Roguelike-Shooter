using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControlBase : MonoBehaviour {
    public static T Show<T>(Transform _parentTrans) where T : UIControlBase
    {
        T tempBase = TResources.Instantiate<T>("UI/Controls/" + typeof(T).ToString(), _parentTrans);
        tempBase.Init();
        return tempBase;
    }

    protected virtual void Init()
    {

    }
    protected virtual void OnDestroy()
    {

    }
    protected void Hide()
    {
        Destroy(this.gameObject);
    }
}
