using System;
using UnityEngine;
using UnityEngine.UI;
public class UIT_GridItem : MonoBehaviour
{
    Action<int> OnItemClick;
    protected int i_Index;
    protected bool b_highLight; 
    protected Transform tf_Container;
    protected RectTransform rtf_RectTransform;
    public RectTransform rectTransform
    {
        get
        {
            return rtf_RectTransform;
        }
    }
    public int I_Index
    {
        get
        {
            return i_Index;
        }
    }
    public bool B_HighLight
    {
        get
        {
            return b_highLight;
        }
    }
    protected virtual void Init()
    {
        if (rtf_RectTransform != null)
            return;
        rtf_RectTransform = transform.GetComponent<RectTransform>();
        tf_Container = transform.Find("Container");
    }
    public void SetGridControlledItem(int _index, Action<int> _OnItemClick)
    {
        Init();
        i_Index = _index;
        OnItemClick = _OnItemClick;
        SetHighLight(false);
    }
    public virtual void SetHighLight(bool highLight)
    {
        b_highLight = highLight;
    }
    public virtual void Reset()
    {
        //To Be Continued···
    }
    protected void OnItemTrigger()
    {
        OnItemClick?.Invoke(i_Index);
    }
}
