using System;
using UnityEngine;
using UnityEngine.UI;
public class UIT_GridItem : MonoBehaviour
{
    protected Transform tf_Container;
    protected RectTransform rtf_RectTransform;
    public RectTransform rectTransform => rtf_RectTransform;
    public int m_Index { get; protected set; }
    public virtual void Init()
    {
        rtf_RectTransform = transform.GetComponent<RectTransform>();
        tf_Container = transform.Find("Container");
    }
    public virtual void OnActivate(int _index)
    {
        m_Index = _index;
    }
    public virtual void Reset()
    {
        //To Be Continued···
    }

    public void SetShowScrollView(bool show)
    {
        tf_Container.SetActivate(show);
    }
}
