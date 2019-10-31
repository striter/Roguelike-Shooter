using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToolsBase : MonoBehaviour {
    public static T Show<T>(Transform _parentTrans) where T : UIToolsBase
    {
        T tempBase = TResources.Instantiate<T>("UI/Tools/" + typeof(T).ToString(), _parentTrans);
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
