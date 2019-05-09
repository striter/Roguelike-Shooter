using System;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIGI_SporeContainer : UIT_GridItem {
    RectTransform rtf_Level;
    Text txt_Level;
    Text txt_locked;
    Action<int,bool,Vector2> OnDragStatus;
    Action<int, Vector2> OnDrag;
    Action<int> OnRaycast,OnTickProfit;
    float f_tickTime,f_tickCheck;
    protected override void Init()
    {
        base.Init();
        txt_locked = tf_Container.Find("Locked").GetComponent<Text>();
        rtf_Level = tf_Container.Find("Level").GetComponent<RectTransform>();
        txt_Level = rtf_Level.GetComponentInChildren<Text>();
        UIT_EventTriggerListener listener = GetComponent<UIT_EventTriggerListener>();
        listener.D_OnDragStatus += DragStatus;
        listener.D_OnDrag += Drag;
        listener.D_OnRaycast = Raycast;
    }
    public void Init(Action<int,bool, Vector2> _OnDragStatus,Action<int,Vector2> _OnDrag,Action<int> _OnDrop,Action<int> _OnTickProfit)
    {
        OnDragStatus = _OnDragStatus;
        OnDrag = _OnDrag;
        OnRaycast = _OnDrop;
        OnTickProfit = _OnTickProfit;

        f_tickTime = 0f;
        f_tickCheck = UnityEngine.Random.Range(0, UIConst.I_SporeManagerContainerStartRandomEnd);
    }

    void DragStatus(bool begin, Vector2 position)
    {
        OnDragStatus(i_Index, begin, position);
    }
    void Drag(Vector2 position)
    {
        OnDrag(i_Index, position);
    }
    void Raycast()
    {
        OnRaycast(i_Index);
    }

    public void Tick(float deltaTime)
    {
        f_tickTime += deltaTime;
        while (f_tickTime > f_tickCheck)
        {
            OnTickProfit(I_Index);

            f_tickTime -= f_tickCheck;
            f_tickCheck = UIConst.I_SporeManagerTickOffsetEach;
        }
    }

    public void SetContainerInfo(int level)
    {
        rtf_Level.SetActivate(level != -1&&level!=0);
        txt_locked.SetActivate(level == -1);
        txt_Level.text = level.ToString();
    }
    public void SetPosition(Vector2 screenPosition)
    {
        rtf_Level.position = new Vector3( screenPosition.x,screenPosition.y,rtf_Level.position.z);
    }
    public void SetDragStatus(bool dragging)
    {
        if (dragging)
        {
            rtf_Level.transform.SetParent(gc_Parent.transform);
            rtf_Level.SetAsLastSibling();
        }
        else
        {
            rtf_Level.transform.SetParent(tf_Container);
            rtf_Level.anchoredPosition = Vector2.zero;
        }
    }
}
