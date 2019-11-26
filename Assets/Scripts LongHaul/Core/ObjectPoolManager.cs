using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ObjectPoolItem<T>{
    void OnPoolItemInit(T identity,Action<T,MonoBehaviour> OnRecycle);
}
public class ObjectPoolMonoItem<T> :MonoBehaviour,ObjectPoolItem<T>
{
    public bool m_IsPoolItem { get; private set; }
    public T m_Identity { get; private set; }
    public Action<T,MonoBehaviour> OnSelfRecycle;
    public virtual void OnPoolItemInit(T _identity,Action<T,MonoBehaviour> _OnSelfRecycle)
    {
        m_Identity = _identity;
        m_IsPoolItem = true;
        OnSelfRecycle = _OnSelfRecycle;
    }
    protected void DoPoolItemRecycle()
    {
        OnSelfRecycle?.Invoke(m_Identity, this);
    }
    private void OnEnable() { if (m_IsPoolItem) OnPoolItemEnable(); }
    private void OnDisable() { if (m_IsPoolItem) OnPoolItemDisable(); }
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
        public List<Y> l_Deactive=new List<Y>();
        public List<Y> l_Active=new List<Y>();

        public Y NewItem(T identity,Action<T,MonoBehaviour> OnRecycle)
        {
            Y item = GameObject.Instantiate(m_spawnItem, tf_PoolSpawn); ;
            item.name = m_spawnItem.name + "_"+(l_Deactive.Count + l_Active.Count).ToString();
            item.OnPoolItemInit(identity, OnRecycle);
            return item;
        }
    }

    static Dictionary<T, ItemPoolInfo> d_ItemInfos = new Dictionary<T, ItemPoolInfo>();
    public static bool Registed(T identity)
    {
        return d_ItemInfos.ContainsKey(identity);
    }
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
            info.l_Deactive.Add(spawnItem);
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
        if (info.l_Deactive.Count > 0)
        {
            item = info.l_Deactive[0];
            info.l_Deactive.RemoveAt(0);
        }
        else
        {
            item = info.NewItem(identity,SelfRecycle);
        }
        info.l_Active.Add(item);
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
        info.l_Active.Remove(obj);
        info.i_poolSaveAmount++;
        obj.SetActivate(false);
        obj.transform.SetParent(tf_PoolSpawn);
        info.l_Deactive.Add(obj);
    }

    public static void RecycleAll(T identity)
    {
        ItemPoolInfo info = d_ItemInfos[identity];
        info.l_Active.Traversal((Y temp) => {
            Recycle(identity,temp);
        }, true);
    }
    public static void RecycleAll(Predicate<Y>  predicate=null) 
    {
        d_ItemInfos.Traversal((T identity, ItemPoolInfo info)=> {
            info.l_Active.Traversal((Y target) =>
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

            for (int i = 0; i < info.l_Deactive.Count; i++)
                GameObject.Destroy(info.l_Deactive[i].gameObject);
            for (int i = 0; i < info.l_Active.Count; i++)
                GameObject.Destroy(info.l_Active[i].gameObject);

            d_ItemInfos.Remove(temp);
        }, true);
    }
}

public class ObjectPoolSimple<T,Y> 
{
    Transform transform;
    protected GameObject GridItem;
    public Dictionary<T, Y> m_ActiveItemDic { get; private set; } = new Dictionary<T, Y>();
    public List<Y> m_InactiveItemList { get; private set; } = new List<Y>();
    protected Func<Transform,T, Y> OnInitItem;
    protected Func<Y, Transform> GetItemTransform;
    public ObjectPoolSimple(GameObject obj, Transform poolTrans,Func<Transform,T,Y> _OnInitItem,Func<Y,Transform> _GetItemTransform)
    {
        GridItem = obj;
        GridItem.gameObject.SetActive(false);
        transform = poolTrans;
        OnInitItem = _OnInitItem;
        GetItemTransform = _GetItemTransform;
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
            targetItem = OnInitItem(GameObject.Instantiate(GridItem.gameObject, transform).transform, identity); 
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
}
