using UnityEngine;
using GameSetting;

public class EntityCharacterBase : EntityBase, ISingleCoroutine
{
    public int I_DefaultArmor;
    public float F_MovementSpeed;
    public float F_AttackSpread;
    public Transform tf_Model { get; private set; }
    public Transform tf_Head { get; private set; }
    public CharacterInfoManager m_CharacterInfo { get; private set; }
    Renderer[] m_SkinRenderers;
    public virtual Vector3 m_PrecalculatedTargetPos(float time) { Debug.LogError("Override This Please");return Vector2.zero; }
    protected virtual CharacterInfoManager GetEntityInfo() => new CharacterInfoManager(this, OnReceiveDamage, OnExpireChange);
    protected override HealthBase GetHealthManager()
    {
        m_HealthManager = new EntityHealth(this, OnHealthChanged, OnDead);
        return m_HealthManager;
    }
    public EntityHealth m_HealthManager { get; private set; }
    public override bool B_IsCharacter => true;
    public override void Init(int _poolIndex)
    {
        base.Init(_poolIndex);
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        m_SkinRenderers = tf_Model.Find("Skin").GetComponentsInChildren<Renderer>();
        m_CharacterInfo = GetEntityInfo();
    }

    public override void OnSpawn(int _entityID,enum_EntityFlag _flag)
    {
        base.OnSpawn(_entityID,_flag);
        if (I_EntityID == -1)
            Debug.LogError("Please Init Entity Info!" + gameObject.name.ToString());
       m_CharacterInfo.OnActivate();
       m_HealthManager.OnActivate(I_MaxHealth,I_DefaultArmor,true);
       m_SkinRenderers.Traversal((Renderer renderer) => {renderer.materials.Traversal((Material mat)=> {mat.SetFloat("_Amount1", 0);}); });
    }

    protected virtual void OnExpireChange(){}
    protected virtual void OnEnable(){}
    protected virtual void OnDisable()
    {
        this.StopSingleCoroutines(0,1);
    }
    protected virtual void Update()
    {
        m_CharacterInfo.Tick(Time.deltaTime);
    }

    protected override bool OnReceiveDamage(DamageInfo damageInfo)
    {
        if (base.OnReceiveDamage(damageInfo))
        {
            damageInfo.m_detail.m_BuffAplly.Traversal((int buffIndex) => { m_CharacterInfo.AddBuff(damageInfo.m_detail.I_SourceID, buffIndex); });
            return true;
        }

        return false;
    }

    protected override void OnDead()
    {
        m_CharacterInfo.OnDeactivate();
        this.StartSingleCoroutine(1, TIEnumerators.ChangeValueTo((float value) => {
            TCommon.Traversal(m_SkinRenderers, (Renderer renderer) => {
                renderer.materials.Traversal((Material mat) => {
                    mat.SetFloat("_Amount1", value);
                });
            });
        }, 0, 1, GameConst.F_EntityDeadFadeTime, OnRecycle));
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterDead,this);
    }
    protected override void OnHealthChanged(enum_HealthChangeMessage type)
    {
        Color targetColor = Color.white;
        switch (type)
        {
            case enum_HealthChangeMessage.DamageArmor:
                targetColor = Color.yellow;
                break;
            case enum_HealthChangeMessage.DamageHealth:
                targetColor = Color.red;
                break;
            case enum_HealthChangeMessage.ReceiveArmor:
                targetColor = Color.blue;
                break;
            case enum_HealthChangeMessage.ReceiveHealth:
                targetColor = Color.green;
                break;
        }
        this.StartSingleCoroutine(0,TIEnumerators.ChangeValueTo((float value)=> {
            TCommon.Traversal(m_SkinRenderers, (Renderer renderer) => {
                renderer.material.SetColor("_Color",Color.Lerp(targetColor, Color.white, value)); });
          },0,1,1f));
    }
}
