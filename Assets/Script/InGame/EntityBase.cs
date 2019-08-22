using UnityEngine;
using GameSetting;

public class EntityBase : MonoBehaviour, ISingleCoroutine
{
    public int I_MaxHealth;
    public int I_DefaultArmor;
    public float F_MovementSpeed;
    public float F_AttackSpread;
    public int I_EntityID { get; private set; } = -1;
    public int I_PoolIndex { get; private set; } = -1;
    public Transform tf_Model { get; private set; }
    public Transform tf_Head { get; private set; }
    public enum_EntityFlag m_Flag { get; private set; }
    public EntityInfoManager m_EntityInfo { get; private set; }
    public EntityHealth m_HealthManager { get; private set; }
    public virtual bool B_IsPlayer => false;
    public HitCheckEntity m_HitCheck => m_HitChecks[0];
    HitCheckEntity[] m_HitChecks;
    Renderer[] m_SkinRenderers;
    public virtual Vector3 m_PrecalculatedTargetPos(float time) { Debug.LogError("Override This Please");return Vector2.zero; }
    protected virtual EntityInfoManager GetEntityInfo() => new EntityInfoManager(this, OnReceiveDamage, OnInfoChange);
    
    public virtual void Init(int presetIndex)
    {
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        I_PoolIndex=presetIndex;
        m_SkinRenderers = tf_Model.Find("Skin").GetComponentsInChildren<Renderer>();
        m_HitChecks = GetComponentsInChildren<HitCheckEntity>();
        m_EntityInfo = GetEntityInfo();
        m_HealthManager = new EntityHealth(this,OnHealthEffect,OnDead);
    }

    public virtual void OnSpawn(int _entityID,enum_EntityFlag _flag)
    {
        I_EntityID = _entityID;
        m_Flag = _flag;
        if (I_EntityID == -1)
            Debug.LogError("Please Init Entity Info!" + gameObject.name.ToString());

       m_EntityInfo.OnActivate();
       m_HealthManager.OnActivate(I_MaxHealth,I_DefaultArmor);
       m_HitChecks.Traversal((HitCheckEntity check) => { check.Attach(this,OnReceiveDamage);  check.SetEnable(true); });
       m_SkinRenderers.Traversal((Renderer renderer) => {renderer.materials.Traversal((Material mat)=> {mat.SetFloat("_Amount1", 0);}); });
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
        m_EntityInfo.Tick(Time.deltaTime);

    }

    protected bool OnReceiveDamage(DamageInfo damageInfo)
    {
        if (m_HealthManager.b_IsDead)
            return false;
        
        damageInfo.m_detail.m_BuffAplly.Traversal((int buffIndex) => { m_EntityInfo.AddBuff(damageInfo.m_detail.I_SourceID, buffIndex);});
        
        return m_HealthManager.OnReceiveDamage(damageInfo, m_EntityInfo.F_DamageReceiveMultiply);
    }


    protected virtual void OnDead()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnEntityDead,this);
        m_EntityInfo.OnDeactivate();
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.HideAllAttaches(); check.SetEnable(false); });
        this.StartSingleCoroutine(1, TIEnumerators.ChangeValueTo((float value) => {
            TCommon.Traversal(m_SkinRenderers, (Renderer renderer) => {
                renderer.materials.Traversal((Material mat) => {
                    mat.SetFloat("_Amount1", value);
                });
            });
        }, 0, 1, GameConst.F_EntityDeadFadeTime, OnRecycle));
    }
    protected virtual void OnRecycle()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnEntityRecycle, this);
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
