using UnityEngine;
using GameSetting;

public class EntityBase : MonoBehaviour,ISingleCoroutine
{
    public int I_EntityID { get; private set; } = -1;
    HitCheckEntity[] m_HitChecks;
    Renderer[] m_Renderers;
    protected Transform tf_Model;
    protected SEntity m_EntityInfo;
    public float m_CurrentHealth { get; private set; }
    public bool b_IsDead { get; private set; }
    public virtual void Init(int id,SEntity entityInfo)
    {
        I_EntityID = id;
        tf_Model = transform.Find("Model");
        m_Renderers = tf_Model.GetComponentsInChildren<Renderer>();
        m_HitChecks = GetComponentsInChildren<HitCheckEntity>();
        TCommon.TraversalArray(m_HitChecks, (HitCheckEntity check) => { check.Attach(I_EntityID,TryTakeDamage); });
        m_EntityInfo = entityInfo;
        m_CurrentHealth = m_EntityInfo.m_MaxHealth;
        b_IsDead = false;
    }
    protected virtual void Start()
    {
        if (I_EntityID == -1)
            Debug.LogError("Please Init Entity Info!" + gameObject.name.ToString());
    }
    protected virtual void OnEnable()
    {
        m_CurrentHealth = m_EntityInfo.m_MaxHealth;
        b_IsDead = false;
    }
    protected virtual void OnDisable()
    {
        this.StopSingleCoroutine(0);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TryTakeDamage(1f);
    }
    protected bool TryTakeDamage(float damageAmount)
    {
        if (b_IsDead)
            return false;
        
        OnTakeDamage(damageAmount);

        m_CurrentHealth -= damageAmount;
        if (m_CurrentHealth <= 0)
        {
            b_IsDead = true;
            OnDead();
        }

        return true;
    }
    protected virtual void OnDead()
    {
        ObjectManager.RecycleEntity(m_EntityInfo.m_Type, this);
    }
    protected virtual void OnTakeDamage(float damageAmount)
    {
        this.StartSingleCoroutine(0,TIEnumerators.ChangeValueTo((float value)=> {
            Color targetColor = Color.Lerp(Color.red, Color.white, value);
            TCommon.TraversalArray(m_Renderers, (Renderer renderer) => {
                renderer.material.SetColor("_Color",targetColor); });
          },0,1,.5f));
    }
}
