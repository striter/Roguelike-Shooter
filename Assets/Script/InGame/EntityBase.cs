using UnityEngine;
using GameSetting;

public class EntityBase : MonoBehaviour, ISingleCoroutine
{
    public int I_EntityID { get; private set; } = -1;
    HitCheckEntity[] m_HitChecks;
    Renderer[] m_SkinRenderers;
    protected Transform tf_Model;
    public EntityBuffManager m_BuffManager { get; private set; }
    public Transform tf_Head { get; private set; }
    public SEntity m_EntityInfo { get; private set; }
    public float m_CurrentHealth { get; private set; }
    public float m_CurrentArmor { get; private set; }
    public float m_CurrentMana { get; private set; }
    public bool b_IsDead => m_CurrentHealth <= 0;
    public bool B_IsPlayer { get; private set; }
    public float F_TotalHealth => m_EntityInfo.m_MaxArmor + m_EntityInfo.m_MaxHealth;
    private float f_ArmorRegenCheck;
    protected EntityBase m_Target;
    protected DamageBuffInfo GetDamageBuffInfo() => m_BuffManager.m_DamageBuffProperty;
    public virtual Vector3 m_PrecalculatedTargetPos(float time) { Debug.LogError("Override This Please");return Vector2.zero; }
    public virtual void Init(SEntity entityInfo)
    {
        Debug.LogError("Override This Please!");
    }
    protected void Init( SEntity entityInfo,bool isPlayer)
    {
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        B_IsPlayer = isPlayer;
        m_SkinRenderers = tf_Model.Find("Skin").GetComponentsInChildren<Renderer>();
        m_HitChecks = GetComponentsInChildren<HitCheckEntity>();
        m_BuffManager = new EntityBuffManager(OnReceiveDamage);
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.Attach(this, OnReceiveDamage); });
        m_EntityInfo = entityInfo;
    }
    public virtual void OnSpawn(int id)
    {
        I_EntityID = id;
        if (I_EntityID == -1)
            Debug.LogError("Please Init Entity Info!" + gameObject.name.ToString());

        m_CurrentHealth = m_EntityInfo.m_MaxHealth;
        m_CurrentArmor = m_EntityInfo.m_MaxArmor;
        m_CurrentMana = m_EntityInfo.m_MaxMana;
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.SetEnable(true); });
        TCommon.Traversal(m_SkinRenderers, (Renderer renderer) => {renderer.materials.Traversal((Material mat)=> {mat.SetFloat("_Amount1", 0);}); });
    }

    protected virtual void OnEnable()
    {
        m_CurrentHealth = m_EntityInfo.m_MaxHealth;
    }
    protected virtual void OnDisable()
    {
        m_BuffManager.OnDeactivate();
        this.StopSingleCoroutine(0);
    }
    protected virtual void Update()
    {
        f_ArmorRegenCheck -= Time.deltaTime;
        if (f_ArmorRegenCheck < 0 && m_CurrentArmor != m_EntityInfo.m_MaxArmor)
            OnReceiveDamage(new DamageInfo( -1*m_EntityInfo.m_ArmorRegenSpeed * Time.deltaTime, enum_DamageType.Regen));
        m_BuffManager.Tick(Time.deltaTime);
    }
    public void OnReceiveBuff(int buffIndex) => m_BuffManager.AddBuff(buffIndex);
    protected bool OnReceiveDamage(DamageInfo damageInfo)
    {
        if (b_IsDead)
            return false;

        damageInfo.m_BuffApply.m_BuffAplly.Traversal((int buffIndex) => {OnReceiveBuff(buffIndex); });

        if (damageInfo.m_AmountApply > 0)    //Damage
        {
            m_CurrentArmor -= damageInfo.m_AmountApply * m_BuffManager.m_EntityBuffProperty.F_DamageReduceMultiply;
            if (m_CurrentArmor < 0)
            {
                m_CurrentHealth += m_CurrentArmor;
                m_CurrentArmor = 0;
            }

            f_ArmorRegenCheck = m_EntityInfo.m_ArmorRegenDuration;

            OnHealthEffect(true, m_CurrentArmor > 0);
            if (b_IsDead)
                OnDead();
        }
        else if(damageInfo.m_AmountApply<0)    //Healing
        {
            m_CurrentArmor -= damageInfo.m_AmountApply ;
            if (m_CurrentArmor > m_EntityInfo.m_MaxArmor)
                m_CurrentArmor = m_EntityInfo.m_MaxArmor;
            OnHealthEffect(false);
        }

        return true;
    }
    public virtual void SetTarget(EntityBase target)
    {
        m_Target = target;
    }

    protected virtual void OnCostMana(float manaCost)
    {
        m_CurrentMana -= manaCost;
    }

    protected virtual void OnDead()
    {
        m_BuffManager.OnDeactivate();
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.HideAllAttaches(); check.SetEnable(false); });
        this.StartSingleCoroutine(1, TIEnumerators.ChangeValueTo((float value) => {
            TCommon.Traversal(m_SkinRenderers, (Renderer renderer) => {
                renderer.materials.Traversal((Material mat) => {
                    mat.SetFloat("_Amount1", value);
                });
            });
        }, 0, 1, 1f, () => {
            ObjectManager.RecycleEntity(m_EntityInfo.m_Index, this);
        }));
    }
    protected virtual void OnHealthEffect(bool isDamage,bool armorDamage=true)
    {
        this.StartSingleCoroutine(0,TIEnumerators.ChangeValueTo((float value)=> {
            Color targetColor = Color.Lerp(isDamage?(armorDamage ? Color.yellow:Color.red): Color.green, Color.white, value);
            TCommon.Traversal(m_SkinRenderers, (Renderer renderer) => {
                renderer.material.SetColor("_Color",targetColor); });
          },0,1,1f));
    }
}
