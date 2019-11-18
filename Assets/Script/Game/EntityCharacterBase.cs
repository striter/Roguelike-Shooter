using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;

public class EntityCharacterBase : EntityBase, ISingleCoroutine
{
    public int I_DefaultArmor;
    public float F_MovementSpeed;
    public float F_AttackSpread;
    public Transform tf_Model { get; private set; }
    public Transform tf_Head { get; private set; }
    public virtual Transform tf_Weapon=>null;
    public CharacterInfoManager m_CharacterInfo { get; private set; }
    EntityCharacterEffectManager m_Effect;
    public virtual Vector3 m_PrecalculatedTargetPos(float time) { Debug.LogError("Override This Please");return Vector2.zero; }
    protected virtual CharacterInfoManager GetEntityInfo() => new CharacterInfoManager(this, m_HitCheck.TryHit, OnExpireChange);
    public virtual float m_baseMovementSpeed => F_MovementSpeed;
    public override bool B_IsCharacter => true;
    protected override float DamageReceiveMultiply => m_CharacterInfo.F_DamageReceiveMultiply;
    protected override float HealReceiveMultiply => m_CharacterInfo.F_HealReceiveMultiply;
    public new EntityHealth m_Health=>base.m_Health as EntityHealth;
    protected override HealthBase GetHealthManager()=> new EntityHealth(this, OnHealthStatus, OnDead);

    protected virtual enum_GameAudioSFX m_DamageClip => enum_GameAudioSFX.EntityDamage;
    protected virtual enum_GameAudioSFX m_ReviveClip => enum_GameAudioSFX.PlayerRevive;

    public override void OnPoolItemInit(int _poolIndex)
    {
        base.OnPoolItemInit(_poolIndex);
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        m_Effect = new EntityCharacterEffectManager(tf_Model.Find("Skin").GetComponentsInChildren<Renderer>());
        m_CharacterInfo = GetEntityInfo();
    }

    protected override void OnPoolItemEnable()
    {
        base.OnPoolItemEnable();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinished);
        m_CharacterInfo.OnActivate();
    }

    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinished);
        this.StopSingleCoroutines(0);
        m_CharacterInfo.OnDeactivate();
    }

    public override void OnActivate(enum_EntityFlag _flag,int _spawnerID=-1,float startHealth =0)
    {
       base.OnActivate(_flag,_spawnerID,startHealth);
        m_Effect.OnReset();
    }

    public void SetExtraDifficulty(float baseHealthMultiplier, float maxHealthMultiplier, SBuff difficultyBuff)
    {
        m_CharacterInfo.AddBuff(-1, difficultyBuff);
        m_Health.SetHealthMultiplier(maxHealthMultiplier);
        m_Health.OnSetHealth(I_MaxHealth * baseHealthMultiplier, true);
    }

    protected virtual void OnExpireChange(){ }

    private void Update()
    {
        m_CharacterInfo.Tick(Time.deltaTime);
        m_Health.OnMaxHealthAdditive(m_CharacterInfo.F_MaxHealthAdditive);
        m_Effect.SetCloak(m_CharacterInfo.B_Effecting(enum_CharacterEffect.Cloak));
        m_Effect.SetFreezed(m_CharacterInfo.B_Effecting(enum_CharacterEffect.Freeze));
        m_Effect.SetScaned(m_CharacterInfo.B_Effecting(enum_CharacterEffect.Scan));
        if (!m_Health.b_IsDead)
            OnCharacterUpdate(Time.deltaTime);
    }
    protected virtual void OnCharacterUpdate(float deltaTime)
    {

    }

    public virtual void ReviveCharacter(float reviveHealth = -1, float reviveArmor = -1)
    {
        if (!m_Health.b_IsDead)
            return;
        OnRevive();
        m_Effect.OnReset();
        m_CharacterInfo.OnRevive();
        EntityHealth health = (m_Health as EntityHealth);
        health.OnRevive(reviveHealth == -1 ? health.m_MaxHealth : reviveHealth, reviveArmor == -1 ? health.m_DefaultArmor : reviveArmor);
        this.StopSingleCoroutine(0);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterRevive, this);
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
    
    protected override void OnHealthStatus(enum_HealthChangeMessage type)
    {
        m_Effect.OnHit(type);
        switch (type)
        {
            case enum_HealthChangeMessage.DamageArmor:
            case enum_HealthChangeMessage.DamageHealth:
                GameAudioManager.Instance.PlayClip(m_EntityID, GameAudioManager.Instance.GetSFXClip(m_DamageClip), false, transform);
                break;
        }
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
        Shader SD_Base,SD_Extra;
        static readonly Shader SD_Scan = Shader.Find("Game/Extra/ScanEffect");
        static readonly Shader SD_Ice = Shader.Find("Game/Effect/Ice");
        static readonly Shader SD_Cloak = Shader.Find("Game/Effect/Cloak");
        static readonly int ID_Color = Shader.PropertyToID("_Color");
        static readonly int ID_Amount1=Shader.PropertyToID("_Amount1");
        static readonly int ID_Opacity = Shader.PropertyToID("_Opacity");
        static readonly Texture TX_Distort = TResources.GetNoiseTex();
        Material[] m_Materials;

        bool m_cloaked;
        bool m_scaned;
        bool m_freezed;
        public EntityCharacterEffectManager(Renderer[] _skin)
        {
            Material materialBase= _skin[0].materials[0];
            Material materialEffect = _skin[0].materials[1];
            m_Materials = new Material[2] { materialBase, materialEffect };
            _skin.Traversal((Renderer renderer) => { renderer.materials = m_Materials;  });
            SD_Base = materialBase.shader;
            SD_Extra = materialEffect.shader;

        }

        public void OnReset()
        {
            m_scaned = false;
            m_cloaked = false;
            m_Materials[0].shader = SD_Base;
            m_Materials[1].shader = SD_Extra;
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

        public void SetScaned(bool _scaned)
        {
            if (m_scaned == _scaned)
                return;

            m_scaned = _scaned;
            m_Materials[1].shader = m_scaned ? SD_Scan : SD_Extra;
        }
        public void SetFreezed(bool _freezed)
        {
            if (m_freezed == _freezed)
                return;
            m_freezed = _freezed;
            m_Materials[0].shader = m_freezed ? SD_Ice : SD_Base;
            if (!m_freezed)
                return;

            m_Materials[0].SetColor("_IceColor", TCommon.GetHexColor("3DAEC5FF"));
            m_Materials[0].SetTexture("_DistortTex", TX_Distort);
            m_Materials[0].SetFloat("_Opacity", .5f);
        }
        public void SetCloak(bool _cloacked)
        {
            if (m_cloaked == _cloacked)
                return;

            m_cloaked = _cloacked;

            m_Materials[0].shader = SD_Cloak;
            if (_cloacked)
            {
                m_Materials[0].SetTexture("_DistortTex", TX_Distort);
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
                {
                    m_Materials[0].SetFloat(ID_Opacity, value);
                }, 1, .3f, .5f));
            }
            else
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>{ m_Materials[0].SetFloat(ID_Opacity, value); }, .3f, 1f, .3f, () => {   m_Materials[0].shader = SD_Base; }));
            }
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

    protected class CharacterAnimator:AnimatorControlBase
    {
        protected static readonly int HS_T_Activate = Animator.StringToHash("t_activate");
        static readonly int HS_T_Dead = Animator.StringToHash("t_dead");
        static readonly int HS_F_Forward = Animator.StringToHash("f_forward");
        static readonly int HS_FM_Movement = Animator.StringToHash("fm_movement");
        static readonly int HS_I_WeaponType = Animator.StringToHash("i_weaponType");
        public CharacterAnimator(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator)
        {
            _animator.fireEvents = true;
            m_Animator.GetComponent<TAnimatorEvent>().Attach(_OnAnimEvent);
        }
        public void OnRevive()
        {
            m_Animator.SetTrigger(HS_T_Activate);
        }
        protected void OnActivate(int index)
        {
            m_Animator.SetInteger(HS_I_WeaponType, index);
            m_Animator.SetTrigger(HS_T_Activate);
        }
        public void SetPause(bool stun)
        {
            m_Animator.speed = stun ? 0 : 1;
        }
        public void SetForward(float forward)
        {
            m_Animator.SetFloat(HS_F_Forward, forward);
        }
        public void SetMovementSpeed(float movementSpeed)
        {
            m_Animator.SetFloat(HS_FM_Movement, movementSpeed);
        }
        public void OnDead()
        {
            m_Animator.SetTrigger(HS_T_Dead);
        }
    }
}
