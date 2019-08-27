using System;
using UnityEngine;
using UnityEngine.UI;
public class UIT_GridItem : MonoBehaviour
{
    protected Transform tf_Container;
    protected RectTransform rtf_RectTransform;
    protected UIT_GridController gc_Parent;
    public RectTransform rectTransform => rtf_RectTransform;
    public int I_Index { get; protected set; }
    protected virtual void Init()
    {
        if (rtf_RectTransform != null)
            return;
        rtf_RectTransform = transform.GetComponent<RectTransform>();
        tf_Container = transform.Find("Container");
    }
    public void SetGridControlledItem(int _index, UIT_GridController parent)
    {
        Init();
        gc_Parent = parent;
        I_Index = _index;
    }
    public virtual void Reset()
    {
        //To Be Continued···
    }
}
