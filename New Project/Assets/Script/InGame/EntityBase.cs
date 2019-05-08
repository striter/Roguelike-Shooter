using UnityEngine;
using GameSetting;

public class EntityBase : MonoBehaviour,ISingleCoroutine
{
    HitCheckBase[] m_HitChecks;
    Renderer[] m_Renderers;
    protected Transform tf_Model;
    protected SEntity m_EntityInfo;
    public float m_CurrentHealth { get; private set; }
    public bool b_IsDead { get; private set; }
    public virtual void Init(SEntity entityInfo)
    {
        tf_Model = transform.Find("Model");
        m_Renderers = tf_Model.GetComponentsInChildren<Renderer>();
        m_HitChecks = GetComponentsInChildren<HitCheckBase>();
        TCommon.TraversalArray(m_HitChecks, (HitCheckBase check) => { check.Attach(TryTakeDamage); });
        m_EntityInfo = entityInfo;
        m_CurrentHealth = m_EntityInfo.m_MaxHealth;
        b_IsDead = false;
    }
    protected virtual void Start()
    {
        if (m_EntityInfo.m_Type == 0)
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
