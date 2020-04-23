using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IObjectpool<T>{
    void OnPoolItemInit(T identity,Action<T,MonoBehaviour> OnRecycle);
}
public class CObjectPoolMono<T> :MonoBehaviour,IObjectpool<T>
{
    public bool m_PoolItemInited { get; private set; }
    public T m_Identity { get; private set; }
    private Action<T,MonoBehaviour> OnSelfRecycle;
    public virtual void OnPoolItemInit(T _identity,Action<T,MonoBehaviour> _OnSelfRecycle)
    {
        m_Identity = _identity;
        m_PoolItemInited = true;
        OnSelfRecycle = _OnSelfRecycle;
    }
    public void DoItemRecycle() => OnSelfRecycle?.Invoke(m_Identity, this);
    private void OnEnable() { if (m_PoolItemInited) OnPoolItemEnable(); }
    private void OnDisable() { if (m_PoolItemInited) OnPoolItemDisable(); }
    protected virtual void OnPoolItemEnable() { }
    protected virtual void OnPoolItemDisable(){}

    protected virtual void OnPoolItemAdd()
    {
    }
    protected virtual void OnPoolItemRemove()
    {
    }
}
public class ObjectPoolManager
{
    protected static Transform tf_PoolSpawn { get; private set; }
    protected static Transform tf_PoolRegist { get; private set; }
    public static void Init()
    {
        tf_PoolSpawn= new GameObject("PoolSpawn").transform;
        tf_PoolRegist = new GameObject("PoolRegist").transform;
    }
}
public class ObjectPoolManager<T, Y> : ObjectPoolManager where Y : MonoBehaviour, IObjectpool<T>
{
    class ItemPoolInfo
    {
        public Y m_spawnItem;
        public Queue<Y> m_DeactiveQueue = new Queue<Y>();
        public List<Y> m_ActiveList = new List<Y>();

        public Y NewItem(T identity, Action<T, MonoBehaviour> OnRecycle)
        {
            Y item = GameObject.Instantiate(m_spawnItem, tf_PoolSpawn); ;
            item.name = m_spawnItem.name + "_" + (m_DeactiveQueue.Count + m_ActiveList.Count).ToString();
            item.OnPoolItemInit(identity, OnRecycle);
            return item;
        }

        public void Destroy()
        {
            GameObject.Destroy(m_spawnItem.gameObject);
            DestroyPoolItem();
        }
        public void DestroyPoolItem()
        {
            for (; m_DeactiveQueue.Count > 0;)
                GameObject.Destroy(m_DeactiveQueue.Dequeue().gameObject);
            for (int i = 0; i < m_ActiveList.Count; i++)
                GameObject.Destroy(m_ActiveList[i].gameObject);

            m_DeactiveQueue.Clear();
            m_ActiveList.Clear();
        }
    }

    static Dictionary<T, ItemPoolInfo> d_ItemInfos = new Dictionary<T, ItemPoolInfo>();
    public static bool Registed(T identity)
    {
        return d_ItemInfos.ContainsKey(identity);
    }
    public static List<T> GetRegistedList() => d_ItemInfos.Keys.ToList();
    public static Y GetRegistedSpawnItem(T identity)
    {
        if (!Registed(identity))
            Debug.LogError("Identity:" + identity + "Unregisted");
        return d_ItemInfos[identity].m_spawnItem;
    }
    public static void Register(T identity, Y registerItem, int poolStartAmount)
    {
        if (d_ItemInfos.ContainsKey(identity))
        {
            Debug.LogError("Same Element Already Registed:" + identity.ToString() + "/" + registerItem.gameObject.name);
            return;
        }
        d_ItemInfos.Add(identity, new ItemPoolInfo());
        registerItem.transform.SetParent(tf_PoolRegist);
        registerItem.SetActivate(false);
        ItemPoolInfo info = d_ItemInfos[identity];
        info.m_spawnItem = registerItem;
        for (int i = 0; i < poolStartAmount; i++)
        {
            Y spawnItem = info.NewItem(identity, SelfRecycle);
            info.m_DeactiveQueue.Enqueue(spawnItem);
        }
    }
    public static Y Spawn(T identity, Transform toTrans, Vector3 toPos, Quaternion rot)
    {
        if (!d_ItemInfos.ContainsKey(identity))
        {
            Debug.LogError("PoolManager:" + typeof(T).ToString() + "," + typeof(Y).ToString() + " Error! Null Identity:" + identity + "Registed");
            return null;
        }
        ItemPoolInfo info = d_ItemInfos[identity];
        Y item;
        if (info.m_DeactiveQueue.Count > 0)
            item = info.m_DeactiveQueue.Dequeue();
        else
            item = info.NewItem(identity, SelfRecycle);
        info.m_ActiveList.Add(item);
        item.transform.SetParentResetTransform(toTrans == null ? tf_PoolSpawn : toTrans);
        item.transform.position = toPos;
        item.transform.rotation = rot;
        item.SetActivate(true);
        return item;
    }
    static void SelfRecycle(T identity, MonoBehaviour obj) => Recycle(identity, obj as Y);
    public static void Recycle(T identity, Y obj)
    {
        if (!d_ItemInfos.ContainsKey(identity))
        {
            Debug.LogWarning("Null Identity Of GameObject:" + obj.name + "/" + identity + " Registed(" + typeof(T).ToString() + "|" + typeof(Y).ToString() + ")");
            return;
        }
        ItemPoolInfo info = d_ItemInfos[identity];
        info.m_ActiveList.Remove(obj);
        obj.SetActivate(false);
        obj.transform.SetParent(tf_PoolSpawn);
        info.m_DeactiveQueue.Enqueue(obj);
    }
    public static void TraversalAllActive(Action<Y> OnEachItem) => d_ItemInfos.Traversal((ItemPoolInfo info) => { info.m_ActiveList.Traversal(OnEachItem); });
    public static void RecycleAll(T identity)
    {
        ItemPoolInfo info = d_ItemInfos[identity];
        info.m_ActiveList.Traversal((Y temp) => {
            Recycle(identity,temp);
        }, true);
    }
    public static void RecycleAll(Predicate<Y>  predicate=null) 
    {
        d_ItemInfos.Traversal((T identity, ItemPoolInfo info)=> {
            info.m_ActiveList.Traversal((Y target) =>
            {
                if (predicate == null || predicate(target))
                    Recycle(identity, target);
            },true);
        });
    }
    public static void OnSceneChange() => d_ItemInfos.Clear();
    public static void DestroyPoolItem()
    {
        RecycleAll();
        d_ItemInfos.Traversal((ItemPoolInfo info) => { info.DestroyPoolItem(); });
    }

    public static void Destroy()
    {
        RecycleAll();
        d_ItemInfos.Traversal(( ItemPoolInfo info) => { info.Destroy(); });
        d_ItemInfos.Clear();
    }
}
#region Pool Component
public class ObjectPoolListComponent<T, Y> : ObjectPoolListBase<T, Y> where Y : Component
{
    Action<Y> OnInitItem;
    Action<T, Y> OnAddItem;
    Action<Y> OnRemoveItem;
    public ObjectPoolListComponent(Transform poolTrans, string itemName, Action<Y> _OnInitItem = null, Action<T,Y> _OnAddItem = null, Action<Y> _OnRemoveItem = null) : base(poolTrans, itemName)
    {
        OnInitItem = _OnInitItem;
        OnAddItem = _OnAddItem;
        OnRemoveItem = _OnRemoveItem;
    }
    public override Y AddItem(T identity)
    {
        Y item = base.AddItem(identity);
        OnAddItem?.Invoke(identity, item);
        return item;
    }
    public override void RemoveItem(T identity)
    {
        Y item = base.GetItem(identity);
        OnRemoveItem?.Invoke(item);
        base.RemoveItem(identity);
    }
    protected override Y CreateNewItem(Transform instantiateTrans)
    {
        Y item = instantiateTrans.GetComponent<Y>();
        OnInitItem?.Invoke(item);
        return item;
    }
    protected override Transform GetItemTransform(Y targetItem) => targetItem.transform;
}
#endregion
#region Pool Class
public class CSimplePoolObject<T>
{
    public Transform transform { get; private set; }
    public T m_identity { get; private set; }
    public CSimplePoolObject()
    {
    }
    public virtual void OnPoolInit(Transform _transform)
    {
        transform = _transform;
    }
    public virtual void OnPoolAdd(T _identity)
    {
        m_identity = _identity;
    }
    public virtual void OnPoolRemove()
    {

    }

    public Transform GetPoolItemTransform() => transform;
}
public class ObjectPoolListClass<T,Y>: ObjectPoolListBase<T,Y> where Y:CSimplePoolObject<T>,new()
{
    public ObjectPoolListClass(Transform poolTrans, string itemName) : base(poolTrans, itemName)
    {
    }
    protected override Y CreateNewItem(Transform instantiateTrans)
    {
        Y item = new Y();
        item.OnPoolInit(instantiateTrans);
        return item;
    }
    protected override Transform GetItemTransform(Y targetItem) => targetItem.transform;
}
#endregion
#region Pool Monobehaviour
public interface ISimplePoolObjectMono<T> 
{
     void OnPoolInit();

     void OnPoolAdd(T identity);

     void OnPoolRemove();
}

public class ObjectPoolListMonobehaviour<T, Y> : ObjectPoolListBase<T, Y> where Y :MonoBehaviour, ISimplePoolObjectMono<T>
{
    public ObjectPoolListMonobehaviour(Transform poolTrans, string itemName) : base(poolTrans, itemName)
    {
    }
    protected override Y CreateNewItem(Transform instantiateTrans)
    {
        Y item = instantiateTrans.GetComponent<Y>();
        item.OnPoolInit();
        return item;
    }
    public override Y AddItem(T identity)
    {
        Y item = base.AddItem(identity);
        item.OnPoolAdd(identity);
        return item;
    }
    public override void RemoveItem(T identity)
    {
        GetItem(identity).OnPoolRemove();
        base.RemoveItem(identity);
    }
    protected override Transform GetItemTransform(Y targetItem) => targetItem.transform;
}
#endregion
public class ObjectPoolListBase<T, Y>
{
    public Transform transform { get; private set; }
    protected GameObject m_PoolItem;
    public Dictionary<T, Y> m_ActiveItemDic { get; private set; } = new Dictionary<T, Y>();
    public List<Y> m_InactiveItemList { get; private set; } = new List<Y>();
    public int Count => m_ActiveItemDic.Count;
    public ObjectPoolListBase(Transform poolTrans, string itemName)
    {
        transform = poolTrans;
        m_PoolItem = poolTrans.Find(itemName).gameObject;
        m_PoolItem.gameObject.SetActive(false);
    }
    public Y GetOrAddItem(T identity)
    {
        if (ContainsItem(identity))
            return GetItem(identity);
        return AddItem(identity);
    }
    public bool ContainsItem(T identity) => m_ActiveItemDic.ContainsKey(identity);
    public Y GetItem(T identity) => m_ActiveItemDic[identity];

    public virtual Y AddItem(T identity)
    {
        Y targetItem;
        if (m_InactiveItemList.Count > 0)
        {
            targetItem = m_InactiveItemList[0];
            m_InactiveItemList.Remove(targetItem);
        }
        else
        {
            targetItem = CreateNewItem(UnityEngine.Object.Instantiate(m_PoolItem, transform).transform);
        }
        if (m_ActiveItemDic.ContainsKey(identity)) Debug.LogError(identity + "Already Exists In Grid Dic");
        else m_ActiveItemDic.Add(identity, targetItem);
        GetItemTransform(targetItem).name = identity.ToString();
        GetItemTransform(targetItem).SetActivate(true);
        return targetItem;
    }

    public virtual void RemoveItem(T identity)
    {
        m_InactiveItemList.Add(m_ActiveItemDic[identity]);
        GetItemTransform(m_ActiveItemDic[identity]).SetActivate(false);
        m_ActiveItemDic.Remove(identity);
    }

    public void Sort(Comparison<KeyValuePair<T,Y>> Compare)
    {
        List<KeyValuePair<T, Y>> list = m_ActiveItemDic.ToList();
        list.Sort(Compare);
        m_ActiveItemDic.Clear();
        list.Traversal((KeyValuePair<T,Y> pair) =>
        {
            GetItemTransform(pair.Value).SetAsLastSibling();
            m_ActiveItemDic.Add(pair.Key,pair.Value);
        });
    }

    public void Clear()
    {
        m_ActiveItemDic.Traversal(RemoveItem, true);
    } 

    protected virtual Y CreateNewItem(Transform instantiateTrans)
    {
        Debug.LogError("Override This Please");
        return default(Y);
    }
    protected virtual Transform GetItemTransform(Y targetItem)
    {
        Debug.LogError("Override This Please");
        return null;
    }
}

