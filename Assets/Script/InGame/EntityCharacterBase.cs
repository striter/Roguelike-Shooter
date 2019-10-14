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
    EntityCharacterEffectManager m_Effect;
    public virtual Vector3 m_PrecalculatedTargetPos(float time) { Debug.LogError("Override This Please");return Vector2.zero; }
    protected virtual CharacterInfoManager GetEntityInfo() => new CharacterInfoManager(this, m_HitCheck.TryHit, OnExpireChange);
    public virtual float m_baseMovementSpeed => F_MovementSpeed;
    public override bool B_IsCharacter => true;
    protected override float DamageReceiveMultiply => m_CharacterInfo.F_DamageReceiveMultiply;
    protected override float HealReceiveMultiply => m_CharacterInfo.F_HealReceiveMultiply;
    public int m_SpawnerEntityID { get; private set; }
    public void SetSpawnerID(int _spawnerEntityID) => m_SpawnerEntityID = _spawnerEntityID;
    public bool b_isSubEntity => m_SpawnerEntityID != -1;
    public new EntityHealth m_Health=>base.m_Health as EntityHealth;
    protected override HealthBase GetHealthManager()=> new EntityHealth(this, OnHealthStatus, OnDead);

    public override void Init(int _poolIndex)
    {
        base.Init(_poolIndex);
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        m_Effect = new EntityCharacterEffectManager(tf_Model.Find("Skin").GetComponentsInChildren<Renderer>());
        m_CharacterInfo = GetEntityInfo();
    }

    public override void OnActivate(enum_EntityFlag _flag, float startHealth =0)
    {
       base.OnActivate(_flag,startHealth);
        m_SpawnerEntityID = -1;
        m_Effect.OnReset();
        m_CharacterInfo.OnActivate();
        this.StopSingleCoroutine(0);
    }
    protected virtual void OnExpireChange(){ }

    protected virtual void OnEnable()
    {
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinished);
    }
    protected virtual void OnDisable()
    {
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinished);
        m_CharacterInfo.OnDeactivate();
        this.StopSingleCoroutines(0);
    }
    protected virtual void Update()
    {
        if (m_Health.b_IsDead)
            return;

        m_CharacterInfo.Tick(Time.deltaTime);
        m_Health.SetMaxHealth(m_CharacterInfo.F_MaxHealthAdditive);

        m_Effect.SetCloak(m_CharacterInfo.B_Effecting( enum_CharacterEffect.Cloak));
        m_Effect.SetScaned(m_CharacterInfo.B_Effecting(enum_CharacterEffect.Scan));
    }

    protected override bool OnReceiveDamage(DamageInfo damageInfo, Vector3 damageDirection)
    {
        if (base.OnReceiveDamage(damageInfo, damageDirection))
        {
            damageInfo.m_detail.m_BaseBuffApply.Traversal((SBuff buffInfo) => { m_CharacterInfo.AddBuff(damageInfo.m_detail.I_SourceID, buffInfo); });
            if (damageInfo.m_detail.m_DamageEffect != enum_CharacterEffect.Invalid) m_CharacterInfo.OnSetEffect(damageInfo.m_detail.m_DamageEffect, damageInfo.m_detail.m_EffectDuration);
            return true;
        }
        return false;
    }

    protected override void OnDead()
    {
        base.OnDead();
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo(m_Effect.OnRecycleEffect, 0, 1, GameConst.F_EntityDeadFadeTime, OnRecycle));
        m_CharacterInfo.OnDead();

        if(m_Health.b_IsDead)
            TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterDead, this);
    }

    public virtual void OnRevive(float reviveHealth=-1, float reviveArmor=-1)
    {
        if (!m_Health.b_IsDead)
            return;
        OnActivate(m_Flag);
        EntityHealth health = (m_Health as EntityHealth);
        health.OnRevive(reviveHealth==-1? health.m_MaxHealth:reviveHealth,reviveArmor==-1? health.m_DefaultArmor:reviveArmor);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterRevive, this);
    }

    protected override void OnHealthStatus(enum_HealthChangeMessage type)
    {
        m_Effect.OnHit(type);
    }
    protected override void OnRecycle()
    {
        base.OnRecycle();
        m_Effect.OnRecycle();
    }
    protected virtual void OnBattleFinished()
    {
        if (b_isSubEntity)
            OnDead();
    }

    class EntityCharacterEffectManager:ISingleCoroutine
    {
        Shader SD_Opaque,SD_Outline;
        static readonly Shader SD_Transparent = Shader.Find("Game/Common/Diffuse_Texture_Transparent");
        static readonly Shader SD_Scan = Shader.Find("Game/Extra/ScanEffect");
        static readonly int ID_Color = Shader.PropertyToID("_Color");
        static readonly int ID_Amount1=Shader.PropertyToID("_Amount1");
        static readonly int ID_Alpha = Shader.PropertyToID("_Alpha");

        Material[] m_Materials;

        bool m_cloaked;
        bool m_scaned;
        public EntityCharacterEffectManager(Renderer[] _skin)
        {
            Material materialBase= _skin[0].materials[0];
            Material materialEffect = _skin[0].materials[1];
            m_Materials = new Material[2] { materialBase, materialEffect };
            _skin.Traversal((Renderer renderer) => { renderer.materials = m_Materials;  });
            SD_Opaque = materialBase.shader;
            SD_Outline = materialEffect.shader;
        }

        public void OnReset()
        {
            m_scaned = false;
            m_cloaked = false;
            m_Materials[0].shader = SD_Opaque;
            m_Materials[1].shader = SD_Outline;
            m_Materials.Traversal((Material mat) => { mat.SetFloat(ID_Amount1, 0); });
        }

        public void OnDead()
        {
            OnReset();
        }

        public void OnRecycleEffect(float value)
        {
            m_Materials[0].SetFloat(ID_Amount1, value);
            m_Materials[1].SetFloat(ID_Amount1, value);
        }

        public void SetCloak(bool _cloacked)
        {
            if (m_cloaked == _cloacked)
                return;

            m_cloaked = _cloacked;

            m_Materials[0].shader = SD_Transparent;
            if (_cloacked)
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
                {
                    m_Materials[0].SetFloat(ID_Alpha, value);
                }, 1, .3f, .5f));
            }
            else
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>{ m_Materials[0].SetFloat(ID_Alpha, value); }, .3f, 1f, .3f, () => {   m_Materials[0].shader = SD_Opaque; }));
            }
        }

        public void SetScaned(bool _scaned)
        {
            if (m_scaned == _scaned)
                return;

            m_scaned = _scaned;
            m_Materials[1].shader = m_scaned ? SD_Scan : SD_Outline;
        }

        public void OnHit(enum_HealthChangeMessage type)
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
            this.StartSingleCoroutine(1, TIEnumerators.ChangeValueTo((float value) => {
                    m_Materials[0].SetColor(ID_Color, Color.Lerp(targetColor, Color.white, value));
            }, 0, 1, 1f));
        }

        public void OnRecycle()
        {
            this.StopAllSingleCoroutines();
        }
    }
}
