using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class UIT_GridControllerMono<T>  where T : Component
{
    public Transform transform { get; private set; }
    protected ObjectPoolMono<int,T> m_Pool { get; private set; }
    public int I_Count => m_Pool.m_ItemDic.Count;
    public UIT_GridControllerMono(Transform _transform) 
    {
        transform = _transform;
        m_Pool = new ObjectPoolMono<int,T>(transform.Find("GridItem").gameObject, _transform, InitItem);
    }
    public virtual T AddItem(int identity)
    {
        T item = m_Pool.AddItem(identity);
        item.transform.SetSiblingIndex(identity);
        return item;
    }
    public void RemoveItem(int identity) => m_Pool.RemoveItem(identity);
    public virtual void ClearGrid() => m_Pool.ClearPool();
    protected virtual void InitItem(T item) { }

    public bool Contains(int identity) => m_Pool.ContainsItem(identity);
    public T GetItem(int identity)=> Contains(identity) ? m_Pool.GetItem(identity) : null;
    public T AddItem(int xIdentity, int yIdentity) => AddItem(GetIdentity(xIdentity, yIdentity));
    public T GetItem(int xIdentity, int yIdentity) => GetItem(GetIdentity(xIdentity,yIdentity));
    int GetIdentity(int xIdentity, int yIdentity) => xIdentity + yIdentity * 1000;

    public T GetOrAddItem(int identity)=>  Contains(identity) ? GetItem(identity) : AddItem(identity);
    public void TraversalItem(Action<int, T> onEach)
    {
        foreach (int i in m_Pool.m_ItemDic.Keys)
        {
            onEach(i, m_Pool.GetItem(i));
        }
    }

    
}

public class UIT_GridControllerGridItem<T>: UIT_GridControllerMono<T> where T:UIT_GridItem
{
    public GridLayoutGroup m_GridLayout { get; private set; }
    public UIT_GridControllerGridItem(Transform _transform) : base(_transform)
    {
        m_GridLayout = _transform.GetComponent<GridLayoutGroup>();
    }

    protected override void InitItem(T trans)
    {
        base.InitItem(trans);
        trans.Init();
    }
    public override T AddItem(int identity)
    {
        T item = base.AddItem(identity);
        item.OnActivate(identity);
        return item;
    }
    public new void RemoveItem(int identity)
    {
        T item = GetItem(identity);
        item.Reset();
        base.RemoveItem(identity);
    }
    public void SortChildrenSibling()
    {
        List<int> keyCollections = m_Pool.m_ItemDic.Keys.ToList();
        keyCollections.Sort((a,b)=> {return a > b?1:-1; });
        for (int i = 0; i < keyCollections.Count; i++)
            GetItem(keyCollections[i]).transform.SetAsLastSibling();
    }
}

public class UIT_GridControllerGridItemScrollView<T> : UIT_GridControllerGridItem<T> where T : UIT_GridItem
{
    public UIT_GridControllerGridItemScrollView(Transform _transform) : base(_transform)
    {

    }

    public void CheckVisible(float verticalNormalized,int visibleSize)
    {
        int total = m_Pool.m_ActiveItemDic.Count;
        int current = (int)(verticalNormalized * total);
        int rangeMin = current - visibleSize;
        int rangeMax = current + visibleSize;
        foreach (int index in m_Pool.m_ActiveItemDic.Keys)
        {
            int position = total  - index;
            GetItem(index).SetShowScrollView(rangeMin< position && position < rangeMax);
        }
    }
}
public class UIT_GridDefaultMulti<T> : UIT_GridControllerGridItem<T> where T : UIT_GridDefaultItem
{
    public int m_selectAmount { get; private set; }=-1;
    public bool m_AllSelected => m_Selecting.Count == m_selectAmount;
    public List<int> m_Selecting { get; private set; } = new List<int>();
    Action<int> OnItemSelect;
    public UIT_GridDefaultMulti(Transform _transform,int _selectAmount=-1,Action<int> _OnItemSelect=null):base(_transform)
    {
        m_selectAmount = _selectAmount;
        OnItemSelect = _OnItemSelect;
    }
    public override void ClearGrid()
    {
        base.ClearGrid();
        m_Selecting.Clear();
    }

    public override T AddItem(int identity)
    {
        T item = base.AddItem(identity);
        item.SetDefaultOnClick(OnItemClick);
        return  item;
    }
    public void OnItemClick(int index)
    {
        if (!m_Selecting.Contains(index))
        {
            if ( m_Selecting.Count>= m_selectAmount)
                return;
            else
                m_Selecting.Add(index);
        }
        else
            m_Selecting.Remove(index);


        foreach (int identity in m_Pool.m_ItemDic.Keys)
        {
            GetItem(identity).SetHighLight(m_Selecting.Contains(identity));
        }
        OnItemSelect?.Invoke(index);
    }
}
public class UIT_GridDefaultSingle<T> : UIT_GridControllerGridItem<T> where T : UIT_GridDefaultItem
{
    bool b_btnEnable;
    bool b_doubleClickConfirm;
    bool b_activeHighLight;
    Action<int> OnItemSelected;
    public int I_CurrentSelecting { get; private set; }
    public UIT_GridDefaultSingle(Transform _transform, Action<int> _OnItemSelected = null, bool activeHighLight = true, bool doubleClickConfirm = false) : base(_transform)
    {
        b_btnEnable = true;
        b_activeHighLight = activeHighLight;
        b_doubleClickConfirm = doubleClickConfirm;
        OnItemSelected = _OnItemSelected;
        I_CurrentSelecting = -1;
    }
    public override void ClearGrid()
    {
        base.ClearGrid();
        I_CurrentSelecting = -1;
    }
    public override T AddItem(int identity)
    {
        T item = base.AddItem(identity);
        item.SetDefaultOnClick(OnItemClick);
        return item;
    }
    public new void RemoveItem(int identity)
    {
        base.RemoveItem(identity);
        if (identity == I_CurrentSelecting)
            I_CurrentSelecting = -1;
    }
    public void OnItemClick(int identity)
    {
        if (!b_btnEnable)
            return;
        if (b_doubleClickConfirm)
        {
            if (identity == I_CurrentSelecting)
            {
                OnItemSelected?.Invoke(identity);
                return;
            }
        }
        else
        {
            if (b_activeHighLight && identity == I_CurrentSelecting)
            {
                return;
            }
            OnItemSelected?.Invoke(identity);
        }

        if (b_activeHighLight && I_CurrentSelecting != -1)
            GetItem(I_CurrentSelecting).SetHighLight(false);

        I_CurrentSelecting = identity;

        if (b_activeHighLight)
            GetItem(I_CurrentSelecting).SetHighLight(true);
    }
    public void SetBtnsEnable(bool active)
    {
        b_btnEnable = active;
    }
}