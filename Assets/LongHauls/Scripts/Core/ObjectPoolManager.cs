using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ObjectPoolItem<T>{
    void OnPoolItemInit(T identity,Action<T,MonoBehaviour> OnRecycle);
}
public class ObjectPoolMonoItem<T> :MonoBehaviour,ObjectPoolItem<T>
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
public class ObjectPoolManager<T,Y>:ObjectPoolManager where Y: MonoBehaviour,ObjectPoolItem<T>
{
    class ItemPoolInfo
    {
        public Y m_spawnItem;
        public int i_poolSaveAmount;
        public Queue<Y> m_DeactiveQueue=new Queue<Y>();
        public List<Y> m_ActiveList=new List<Y>();

        public Y NewItem(T identity,Action<T,MonoBehaviour> OnRecycle)
        {
            Y item = GameObject.Instantiate(m_spawnItem, tf_PoolSpawn); ;
            item.name = m_spawnItem.name + "_"+(m_DeactiveQueue.Count + m_ActiveList.Count).ToString();
            item.OnPoolItemInit(identity, OnRecycle);
            return item;
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
            Debug.LogError("Identity:"+identity +"Unregisted");
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
        info.i_poolSaveAmount = poolStartAmount;
        for (int i = 0; i < info.i_poolSaveAmount; i++)
        {
            Y spawnItem = info.NewItem(identity,SelfRecycle);
            info.m_DeactiveQueue.Enqueue(spawnItem);
        }
    }
    public static Y Spawn(T identity,Transform toTrans)
    {
        if (!d_ItemInfos.ContainsKey(identity))
        {
            Debug.LogWarning("PoolManager:"+typeof(T).ToString()+","+typeof(Y).ToString()+ " Error! Null Identity:" + identity + "Registed");
            return null;
        }
        ItemPoolInfo info = d_ItemInfos[identity];
        Y item;
        if (info.m_DeactiveQueue.Count > 0)
        {
            item = info.m_DeactiveQueue.Dequeue();
        }
        else
        {
            item = info.NewItem(identity,SelfRecycle);
        }
        info.m_ActiveList.Add(item);
        item.transform.SetParentResetTransform(toTrans == null ? tf_PoolSpawn : toTrans);
        item.SetActivate(true);
        return item;
    }
    static void SelfRecycle(T identity, MonoBehaviour obj) => Recycle(identity, obj as Y);
    public static void Recycle(T identity,Y obj)
    {
        if (!d_ItemInfos.ContainsKey(identity))
        {
            Debug.LogWarning("Null Identity Of GameObject:"+obj.name+"/"+identity+" Registed("+typeof(T).ToString()+"|"+typeof(Y).ToString()+")");
            return;
        }
        ItemPoolInfo info = d_ItemInfos[identity];
        info.m_ActiveList.Remove(obj);
        info.i_poolSaveAmount++;
        obj.SetActivate(false);
        obj.transform.SetParent(tf_PoolSpawn);
        info.m_DeactiveQueue.Enqueue(obj);
    }

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
    public static void DestroyAll()
    {
        d_ItemInfos.Traversal((T temp, ItemPoolInfo info) => {
            GameObject.Destroy(info.m_spawnItem.gameObject);

            for (int i = 0; i < info.m_DeactiveQueue.Count; i++)
                GameObject.Destroy(info.m_DeactiveQueue.Dequeue().gameObject);
            for (int i = 0; i < info.m_ActiveList.Count; i++)
                GameObject.Destroy(info.m_ActiveList[i].gameObject);

            d_ItemInfos.Remove(temp);
        }, true);
    }
}
public class CSimplePool<T>
{
    public Transform transform { get; private set; }
    public T m_identity { get; private set; }
    public CSimplePool()
    {
    }
    public virtual void OnPoolInit(Transform _transform, T _identity)
    {
        transform = _transform;
        m_identity = _identity;
    }
}
public class ObjectPoolSimpleClass<T,Y>: ObjectPoolSimpleBase<T,Y> where Y:CSimplePool<T>,new()
{
    public ObjectPoolSimpleClass(Transform poolTrans, string itemName) : base(poolTrans, itemName)
    {
    }
    protected override Y CreateNewItem(Transform instantiateTrans, T identity)
    {
        Y item = new Y();
        item.OnPoolInit(instantiateTrans, identity);
        return item;
    }
    protected override Transform GetItemTransform(Y targetItem) => targetItem.transform;
}
public class ObjectPoolSimpleComponent<T,Y>:ObjectPoolSimpleBase<T,Y> where Y:Component
{
    Action<Y> OnCreateNewInit;
    public ObjectPoolSimpleComponent(Transform poolTrans, string itemName,Action<Y> _OnCreateNewInit=null):base(poolTrans,itemName)
    {
        OnCreateNewInit = _OnCreateNewInit;
    }
    protected override Y CreateNewItem(Transform instantiateTrans, T identity)
    {
        Y item= instantiateTrans.GetComponent<Y>(); ;
        OnCreateNewInit?.Invoke(item);
        return item;
    } 
    protected override Transform GetItemTransform(Y targetItem)=>targetItem.transform;
}
public class ObjectPoolSimpleBase<T, Y>
{
    public Transform transform { get; private set; }
    protected GameObject m_PoolItem;
    public Dictionary<T, Y> m_ActiveItemDic { get; private set; } = new Dictionary<T, Y>();
    public List<Y> m_InactiveItemList { get; private set; } = new List<Y>();

    public ObjectPoolSimpleBase(Transform poolTrans, string itemName)
    {
        transform = poolTrans;
        m_PoolItem = poolTrans.Find(itemName).gameObject;
        m_PoolItem.gameObject.SetActive(false);
    }
    public bool ContainsItem(T identity) => m_ActiveItemDic.ContainsKey(identity);
    public Y GetItem(T identity) => m_ActiveItemDic[identity];
    public Y AddItem(T identity)
    {
        Y targetItem;
        if (m_InactiveItemList.Count > 0)
        {
            targetItem = m_InactiveItemList[0];
            m_InactiveItemList.Remove(targetItem);
        }
        else
        {
            targetItem = CreateNewItem(UnityEngine.Object.Instantiate(m_PoolItem, transform).transform, identity);
        }
        if (m_ActiveItemDic.ContainsKey(identity)) Debug.LogWarning(identity + "Already Exists In Grid Dic");
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
    public virtual void ClearPool()
    {
        foreach (Y target in m_ActiveItemDic.Values)
        {
            GetItemTransform(target).SetActivate(false);
            m_InactiveItemList.Add(target);
        }
        m_ActiveItemDic.Clear();
    }
    protected virtual Y CreateNewItem(Transform instantiateTrans, T identity)
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

