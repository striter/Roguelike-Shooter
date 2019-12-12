using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class UIT_GridControllerMono<T>  where T : Component
{
    public Transform transform { get; private set; }
    protected ObjectPoolSimple<int,T> m_Pool { get; private set; }
    public int I_Count => m_Pool.m_ActiveItemDic.Count;
    public UIT_GridControllerMono(Transform _transform) 
    {
        transform = _transform;
        m_Pool = new ObjectPoolSimple<int,T>(transform.Find("GridItem").gameObject, _transform, InitItem,(T item)=>item.transform);
    }
    public virtual T AddItem(int identity)
    {
        T item = m_Pool.AddItem(identity);
        item.transform.SetSiblingIndex(identity);
        return item;
    }
    public void RemoveItem(int identity) => m_Pool.RemoveItem(identity);
    public virtual void ClearGrid() => m_Pool.ClearPool();
    protected virtual T InitItem(Transform item,int identity) => item.GetComponent<T>();
    public bool Contains(int identity) => m_Pool.ContainsItem(identity);
    public T GetItem(int identity)=> Contains(identity) ? m_Pool.GetItem(identity) : null;
    public T AddItem(int xIdentity, int yIdentity) => AddItem(GetIdentity(xIdentity, yIdentity));
    public T GetItem(int xIdentity, int yIdentity) => GetItem(GetIdentity(xIdentity,yIdentity));
    int GetIdentity(int xIdentity, int yIdentity) => xIdentity + yIdentity * 1000;

    public T GetOrAddItem(int identity)=>  Contains(identity) ? GetItem(identity) : AddItem(identity);
    public void TraversalItem(Action<int, T> onEach)
    {
        foreach (int i in m_Pool.m_ActiveItemDic.Keys)
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
    protected override T InitItem(Transform trans, int identity)
    {
        T item = base.InitItem(trans, identity);
        item.Init();
        return item;
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
        List<int> keyCollections = m_Pool.m_ActiveItemDic.Keys.ToList();
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

public interface IGridHighlight
{
    void AttachSelectButton(Action<int> OnButtonClick);
    void OnHighlight(bool highlight);
}
public class UIT_GridControlledSingleSelect<T> : UIT_GridControllerGridItem<T> where T : UIT_GridItem, IGridHighlight
{
    public int m_curSelecting { get; private set; } = -1;
    Action<int> OnItemSelect;
    public UIT_GridControlledSingleSelect(Transform _transform,Action<int> _OnItemSelect) : base(_transform)
    {
        OnItemSelect = _OnItemSelect;
    }
    protected override T InitItem(Transform trans, int identity)
    {
        T item = base.InitItem(trans, identity);
        item.AttachSelectButton(OnItemClick);
        item.OnHighlight(false);
        return item;
    }
    
    public void OnItemClick(int index)
    {
        if (m_curSelecting != -1)
            GetItem(m_curSelecting).OnHighlight(false);
        m_curSelecting = index;
        GetItem(m_curSelecting).OnHighlight(true);
        OnItemSelect(index);
    }
}