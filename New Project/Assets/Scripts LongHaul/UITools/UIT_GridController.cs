using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIT_GridController
{
    public Transform transform;
    protected GameObject GridItem;
    protected Dictionary<int, Transform> ActiveItemDic = new Dictionary<int, Transform>();
    protected List<Transform> InactiveItemList = new List<Transform>();
    public UIT_GridController(Transform _transform)
    {
        transform = _transform;
        GridItem = transform.GetChild(0).gameObject;
        GridItem.gameObject.SetActive(false);
        InactiveItemList.Add(GridItem.transform);
    }
    public virtual Transform AddItem(int identity)
    {
        Transform toTrans;
        if (InactiveItemList.Count > 0)
        {
            toTrans = InactiveItemList[0];
            InactiveItemList.Remove(toTrans);
        }
        else
        {
            toTrans = GameObject.Instantiate(GridItem.gameObject, this.transform).transform;
        }
        toTrans.name = identity.ToString();
        if (ActiveItemDic.ContainsKey(identity))
        {
            Debug.LogWarning(identity + "Already Exists In Grid Dic");
        }
        else
        {
            ActiveItemDic.Add(identity, toTrans);
        }
        return toTrans;
    }
    public virtual Transform GetItem(int identity)
    {
        return ActiveItemDic[identity];
    }
    public virtual void RemoveItem(int identity)
    {
        InactiveItemList.Add(ActiveItemDic[identity]);
        ActiveItemDic[identity].SetActivate(false);
        ActiveItemDic.Remove(identity);
    }
    public virtual void ClearGrid ()
    {
        foreach (Transform trans in ActiveItemDic.Values)
        {
            trans.SetActivate(false);
            InactiveItemList.Add(trans);
        }
        ActiveItemDic.Clear();
    }
    public bool Contains(int identity)
    {
        return ActiveItemDic.ContainsKey(identity);
    }
}
public class UIT_GridControllerMono<T>:UIT_GridController where T:UIT_GridItem
{
    bool b_btnEnable;
    bool b_doubleClickConfirm;
    bool b_activeHighLight;
    Action<int> OnItemSelected;
    Dictionary<int, T> MonoItemDic = new Dictionary<int, T>();
    int i_currentSelecting;
    public int I_CurrentSelecting
    {
        get
        {
            return i_currentSelecting;
        }
    }
    public int I_Count
    {
        get
        {
            return MonoItemDic.Count;
        }
    }
    public UIT_GridControllerMono(Transform _transform, Action<int> _OnItemSelected,bool activeHighLight=true,bool doubleClickConfirm=false) : base(_transform)
    {
        b_btnEnable = true;
        b_activeHighLight = activeHighLight;
        b_doubleClickConfirm = doubleClickConfirm;
        OnItemSelected = _OnItemSelected;
        i_currentSelecting = -1;
    }
    public new T AddItem(int identity) 
    {
        T item = base.AddItem(identity).GetComponent<T>();
        item.SetGridControlledItem(identity, OnItemSelect);
        MonoItemDic.Add(identity,item);
        item.SetActivate(true);
        item.transform.SetSiblingIndex(identity); 
        return item;
    }
    public new T GetItem(int identity)
    {
        return Contains(identity)?MonoItemDic[identity]:null;
    }
    public new void ClearGrid()
    {
        base.ClearGrid();
        i_currentSelecting = -1;
        MonoItemDic.Clear();
    }
    public void DeHighlightAll()
    {
        foreach (T template in MonoItemDic.Values)
        {
            if(template.B_HighLight)
            template.SetHighLight(false);
        }
        i_currentSelecting = -1;
    }
    public new void RemoveItem(int identity)
    {
        base.RemoveItem(identity);
        MonoItemDic[identity].Reset();
        MonoItemDic.Remove(identity);
        if (identity == i_currentSelecting)
            i_currentSelecting = -1;
    }
    public void OnItemSelect(int identity)
    {
        if (!b_btnEnable)
            return;
        if (b_doubleClickConfirm)
        {
            if (identity == i_currentSelecting)
            {
                OnItemSelected(identity);
                return;
            }
        }
        else
        {
            if (b_activeHighLight&&identity == i_currentSelecting)
            {
                return;
            }
            if (OnItemSelected != null)
                OnItemSelected(identity);
        }

        if (b_activeHighLight && i_currentSelecting != -1)
            GetItem(i_currentSelecting).SetHighLight(false);

        i_currentSelecting = identity;

        if (b_activeHighLight)
            GetItem(i_currentSelecting).SetHighLight(true);
    }
    public void SetBtnsEnable(bool active)
    {
        b_btnEnable = active;
    }
}
