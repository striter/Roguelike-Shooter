using UnityEngine;
using GameSetting;

public class EntityBase : MonoBehaviour, ISingleCoroutine
{
    public int I_MaxHealth;
    public int I_MaxArmor;
    public float F_MovementSpeed;
    public int I_EntityID { get; private set; } = -1;
    public int I_PoolIndex { get; private set; } = -1;
    HitCheckEntity[] m_HitChecks;
    Renderer[] m_SkinRenderers;
    protected Transform tf_Model;
    public Transform tf_Head { get; private set; }
    public enum_EntityFlag m_Flag { get; private set; }
    protected DamageBuffInfo GetDamageBuffInfo() => m_EntityInfo.m_DamageBuffProperty;
    public EntityInfoManager m_EntityInfo { get; private set; }
    public EntityHealth m_HealthManager { get; private set; }
    public virtual Vector3 m_PrecalculatedTargetPos(float time) { Debug.LogError("Override This Please");return Vector2.zero; }
    public virtual void Init(int presetIndex)
    {
        Debug.LogError("Override This Please");
    }
    protected virtual void Init(int presetIndex,enum_EntityFlag flag= enum_EntityFlag.Invalid)
    {
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        I_PoolIndex = presetIndex;
        m_Flag = flag;
        m_SkinRenderers = tf_Model.Find("Skin").GetComponentsInChildren<Renderer>();
        m_HitChecks = GetComponentsInChildren<HitCheckEntity>();
        m_EntityInfo = new EntityInfoManager(this,OnReceiveDamage, OnInfoChange);
        m_HealthManager = new EntityHealth(this,OnHealthEffect,OnDead);
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.Attach(this, OnReceiveDamage); });
    }
    public virtual void OnSpawn(int id)
    {
        I_EntityID = id;
        if (I_EntityID == -1)
            Debug.LogError("Please Init Entity Info!" + gameObject.name.ToString());
        m_HealthManager.OnActivate();
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.SetEnable(true); });
        TCommon.Traversal(m_SkinRenderers, (Renderer renderer) => {renderer.materials.Traversal((Material mat)=> {mat.SetFloat("_Amount1", 0);}); });
    }
    protected virtual void OnInfoChange()
    {

    }
    protected virtual void OnEnable()
    {
    }
    protected virtual void OnDisable()
    {
        m_EntityInfo.OnDeactivate();
        this.StopSingleCoroutine(0);
    }
    protected virtual void Update()
    {
        m_HealthManager.Tick(Time.deltaTime);
        m_EntityInfo.Tick(Time.deltaTime);
    }
    public void OnReceiveBuff(int buffIndex) => m_EntityInfo.AddBuff(buffIndex);
    protected bool OnReceiveDamage(DamageInfo damageInfo)
    {
        if (m_HealthManager.b_IsDead)
            return false;
        
        damageInfo.m_BuffApply.m_BuffAplly.Traversal((int buffIndex) => {OnReceiveBuff(buffIndex); });
        
        return m_HealthManager.OnReceiveDamage(damageInfo, m_EntityInfo.F_DamageReceiveMultiply);
    }
    public virtual void OnActivate()
    {
    }


    protected virtual void OnDead()
    {
        m_EntityInfo.OnDeactivate();
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.HideAllAttaches(); check.SetEnable(false); });
        this.StartSingleCoroutine(1, TIEnumerators.ChangeValueTo((float value) => {
            TCommon.Traversal(m_SkinRenderers, (Renderer renderer) => {
                renderer.materials.Traversal((Material mat) => {
                    mat.SetFloat("_Amount1", value);
                });
            });
        }, 0, 1, 1f, OnRecycle));
    }
    protected virtual void OnRecycle()
    {
        ObjectManager.RecycleEntity(I_PoolIndex, this);
    }
    protected virtual void OnHealthEffect(enum_HealthMessageType type)
    {
        Color targetColor = Color.white;
        switch (type)
        {
            case enum_HealthMessageType.DamageArmor:
                targetColor = Color.yellow;
                break;
            case enum_HealthMessageType.DamageHealth:
                targetColor = Color.red;
                break;
            case enum_HealthMessageType.ReceiveArmor:
                targetColor = Color.blue;
                break;
            case enum_HealthMessageType.ReceiveHealth:
                targetColor = Color.green;
                break;
        }
        this.StartSingleCoroutine(0,TIEnumerators.ChangeValueTo((float value)=> {
            TCommon.Traversal(m_SkinRenderers, (Renderer renderer) => {
                renderer.material.SetColor("_Color",Color.Lerp(targetColor, Color.white, value)); });
          },0,1,1f));
    }
}
