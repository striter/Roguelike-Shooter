using System;
using UnityEngine;
using UnityEngine.UI;
public class UIT_GridItem : MonoBehaviour
{
    protected RectTransform rtf_Container;
    protected RectTransform rtf_RectTransform;
    public RectTransform rectTransform => rtf_RectTransform;
    public int m_Index { get; protected set; }
    public virtual void Init()
    {
        rtf_RectTransform = transform.GetComponent<RectTransform>();
        rtf_Container = transform.Find("Container") as RectTransform;
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
        rtf_Container.SetActivate(show);
    }
}
