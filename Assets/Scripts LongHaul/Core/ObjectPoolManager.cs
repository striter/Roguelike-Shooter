using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class ObjectPoolManager<T,Y>:ObjectPoolManager where Y:MonoBehaviour {
    class ItemPoolInfo
    {
        public Y m_spawnItem;
        public int i_poolSaveAmount;
        public Action<Y> OnItemInstantiate;
        public List<Y> l_Deactive=new List<Y>();
        public List<Y> l_Active=new List<Y>();

        public Y Instantiate()
        {
            Y item = GameObject.Instantiate(m_spawnItem, tf_PoolSpawn); ;
            item.name = m_spawnItem.name + "_"+(l_Deactive.Count + l_Active.Count).ToString();
            OnItemInstantiate?.Invoke(item);
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
    public static void Register(T identity, Y registerItem, int poolStartAmount, Action<Y> OnItemInstantiate)
    {
        if (d_ItemInfos.ContainsKey(identity))
        {
            Debug.LogError("Same Element Already Registed:" + identity.ToString() + "/" + registerItem.gameObject.name);
            return;
        }
        d_ItemInfos.Add(identity, new ItemPoolInfo());
        registerItem.transform.SetParent(tf_PoolRegist);
        OnItemInstantiate?.Invoke(registerItem);
        registerItem.SetActivate(false);
        ItemPoolInfo info = d_ItemInfos[identity];
        info.m_spawnItem = registerItem;
        info.i_poolSaveAmount = poolStartAmount;
        info.OnItemInstantiate = OnItemInstantiate;
        for (int i = 0; i < info.i_poolSaveAmount; i++)
        {
            Y spawnItem = info.Instantiate();
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
            item = info.Instantiate();
        }
        info.l_Active.Add(item);
        item.transform.SetParentResetTransform(toTrans == null ? tf_PoolSpawn : toTrans);
        item.SetActivate(true);
        return item;
    }

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
    public static void RecycleAll() 
    {
        d_ItemInfos.Traversal((T temp, ItemPoolInfo info)=> {
            RecycleAll(temp);
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
public class ObjectPoolMono<T, Y> : ObjectPoolSimple<T> where Y : Component
{
    Action<Y> InitItem;
    public Dictionary<T, Y> m_ItemDic { get; private set; } = new Dictionary<T, Y>();
    public ObjectPoolMono(GameObject obj, Transform poolTrans, Action<Y> _OnInitItem=null) : base(obj,poolTrans)
    {
        InitItem = _OnInitItem;
    }

    protected override void OnInitItem(Transform trans, T identity)
    {
        base.OnInitItem(trans, identity);
        InitItem?.Invoke(trans.GetComponent<Y>());
    }
    public new Y AddItem(T identity)
    {
        Y item = base.AddItem(identity).GetComponent<Y>();
        m_ItemDic.Add(identity, item);
        return item;
    }
    public new Y GetItem(T identity) => m_ItemDic[identity];
    public override void RemoveItem(T identity)
    {
        base.RemoveItem(identity);
        m_ItemDic.Remove(identity);
    }
    public override void ClearPool()
    {
        base.ClearPool();
        m_ItemDic.Clear();
    }

}
public class ObjectPoolSimple<T> 
{
    Transform transform;
    protected GameObject GridItem;
    public Dictionary<T, Transform> m_ActiveItemDic { get; private set; } = new Dictionary<T, Transform>();
    public List<Transform> m_InactiveItemList { get; private set; } = new List<Transform>();
    protected virtual void OnInitItem(Transform trans, T identity) { }
    public ObjectPoolSimple(GameObject obj, Transform poolTrans)
    {
        GridItem = obj;
        GridItem.gameObject.SetActive(false);
        transform = poolTrans;
    }
    public bool ContainsItem(T identity) => m_ActiveItemDic.ContainsKey(identity);
    public Transform GetItem(T identity) => m_ActiveItemDic[identity];
    public Transform AddItem(T identity)
    {
        Transform toTrans;
        if (m_InactiveItemList.Count > 0)
        {
            toTrans = m_InactiveItemList[0];
            m_InactiveItemList.Remove(toTrans);
        }
        else
        {
            toTrans = GameObject.Instantiate(GridItem.gameObject, transform).transform;
            OnInitItem(toTrans,identity);
        }
        toTrans.name = identity.ToString();
        if (m_ActiveItemDic.ContainsKey(identity)) Debug.LogWarning(identity + "Already Exists In Grid Dic");
        else m_ActiveItemDic.Add(identity, toTrans);
        toTrans.SetActivate(true);
        return toTrans;
    }
    public virtual void RemoveItem(T identity)
    {
        m_InactiveItemList.Add(m_ActiveItemDic[identity]);
        m_ActiveItemDic[identity].SetActivate(false);
        m_ActiveItemDic.Remove(identity);
    }
    public virtual void ClearPool()
    {
        foreach (Transform trans in m_ActiveItemDic.Values)
        {
            trans.SetActivate(false);
            m_InactiveItemList.Add(trans);
        }
        m_ActiveItemDic.Clear();
    }
}
