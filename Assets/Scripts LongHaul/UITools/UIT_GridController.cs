using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIT_GridController
{
    public Transform transform;
    protected GameObject GridItem;
    protected Dictionary<int, Transform> ActiveItemDic = new Dictionary<int, Transform>();
    protected List<Transform> InactiveItemList = new List<Transform>();
    public UIT_GridController(Transform _transform)
    {
        transform = _transform;
        GridItem = transform.Find("GridItem").gameObject;
        GridItem.gameObject.SetActive(false);
        InitItem(GridItem.transform);
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
            InitItem(toTrans);
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
    protected virtual void InitItem(Transform trans)
    {

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

public class UIT_GridControllerMono<T> : UIT_GridController where T : MonoBehaviour
{
    protected Dictionary<int, T> m_ItemDic = new Dictionary<int, T>();
    public UIT_GridControllerMono(Transform _transform) : base(_transform)
    {

    }

    public T GetOrAddItem(int identity)=>Contains(identity) ? GetItem(identity) : AddItem(identity);
    public new T GetItem(int identity)=> Contains(identity) ? m_ItemDic[identity] : null;
    public new T AddItem(int identity)
    {
        T item = base.AddItem(identity).GetComponent<T>();
        m_ItemDic.Add(identity, item);
        OnItemAdd(item, identity);
        item.SetActivate(true);
        item.transform.SetSiblingIndex(identity);
        return item;
    }

    public override void ClearGrid()
    {
        base.ClearGrid();
        m_ItemDic.Clear();
    }

    public override void RemoveItem(int identity)
    {
        base.RemoveItem(identity);
        OnItemRemove(m_ItemDic[identity],identity);
        m_ItemDic.Remove(identity);
    }

    public void TraversalItem(Action<int, T> onEach)
    {
        foreach (int i in ActiveItemDic.Keys)
        {
            onEach(i, m_ItemDic[i]);
        }
    }

    protected virtual void OnItemAdd(T item, int identity)
    {
    }
    protected virtual void OnItemRemove(T item, int identity)
    {

    }
}

public class UIT_GridControllerMonoItem<T>: UIT_GridControllerMono<T> where T:UIT_GridItem
{
    public int I_Count=> m_ItemDic.Count;
    public GridLayoutGroup m_GridLayout { get; private set; }
    public UIT_GridControllerMonoItem(Transform _transform) : base(_transform)
    {
        m_GridLayout = _transform.GetComponent<GridLayoutGroup>();
    }
    protected override void InitItem(Transform trans)
    {
        base.InitItem(trans);
        trans.GetComponent<T>().Init(this);
    }
    protected override void OnItemAdd(T item,int identity)
    {
        base.OnItemAdd(item, identity);
        item.OnActivate(identity);
    }
    protected override void OnItemRemove(T item, int identity)
    {
        base.OnItemRemove(item, identity);
        item.Reset();
    }
    public void SortChildrenSibling()
    {
        List<int> keyCollections = m_ItemDic.Keys.ToList();
        keyCollections.Sort((a,b)=> {return a > b?1:-1; });
        for (int i = 0; i < keyCollections.Count; i++)
            m_ItemDic[keyCollections[i]].transform.SetAsLastSibling();
    }
}
public class UIT_GridDefaultMulti<T> : UIT_GridControllerMonoItem<T> where T : UIT_GridDefaultItem
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

    protected override void OnItemAdd(T item, int identity)
    {
        base.OnItemAdd(item, identity);
        item.SetDefaultOnClick(OnItemClick);
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


        foreach (int item in m_ItemDic.Keys)
        {
            m_ItemDic[item].SetHighLight(m_Selecting.Contains(item));
        }
        OnItemSelect?.Invoke(index);
    }
}
public class UIT_GridDefaultSingle<T> : UIT_GridControllerMonoItem<T> where T : UIT_GridDefaultItem
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
    protected override void OnItemAdd(T item, int identity)
    {
        base.OnItemAdd(item, identity);
        item.SetDefaultOnClick(OnItemClick);
    }
    public void DeHighlightAll()
    {
        I_CurrentSelecting = -1;
    }
    public override void RemoveItem(int identity)
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