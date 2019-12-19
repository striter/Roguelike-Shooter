using GameSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using TSpecialClasses;
using UnityEngine;

public class EntityCharacterBase : EntityBase, ISingleCoroutine
{
    public Renderer ExtraRendererForForest107Only;
    public enum_EnermyType E_SpawnType = enum_EnermyType.Invalid;
    public int I_DefaultArmor;
    public float F_MovementSpeed;
    public float F_AttackSpread;
    public Transform tf_Model { get; private set; }
    public Transform tf_Head { get; private set; }
    public virtual Transform tf_Weapon=>null;
    public virtual Transform tf_WeaponModel => null;
    public CharacterInfoManager m_CharacterInfo { get; private set; }
    EntityCharacterEffectManager m_Effect;
    public virtual Vector3 m_PrecalculatedTargetPos(float time)=> tf_Head.position;
    protected virtual CharacterInfoManager GetEntityInfo() => new CharacterInfoManager(this, m_HitCheck.TryHit, OnExpireChange);
    public virtual float m_baseMovementSpeed => F_MovementSpeed;
    public override bool B_IsCharacter => true;
    protected override float DamageReceiveMultiply => m_CharacterInfo.F_DamageReceiveMultiply;
    protected override float HealReceiveMultiply => m_CharacterInfo.F_HealReceiveMultiply;
    public new EntityHealth m_Health=>base.m_Health as EntityHealth;
    protected override HealthBase GetHealthManager()=> new EntityHealth(this, OnHealthChanged);

    protected virtual enum_GameVFX m_DamageClip => enum_GameVFX.EntityDamage;
    protected virtual enum_GameVFX m_ReviveClip => enum_GameVFX.PlayerRevive;

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        Transform tf_Skin = tf_Model.Find("Skin");
        List<Renderer> renderers = new List<Renderer>();
        if(tf_Skin) renderers.AddRange(tf_Skin.GetComponentsInChildren<Renderer>().ToList());
        if (ExtraRendererForForest107Only) renderers.Add(ExtraRendererForForest107Only);
        m_Effect = new EntityCharacterEffectManager(tf_Model,renderers);
        m_CharacterInfo = GetEntityInfo();
    }

    protected override void OnPoolItemEnable()
    {
        base.OnPoolItemEnable();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        m_CharacterInfo.OnActivate();
    }

    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        this.StopSingleCoroutines(0);
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
        m_Effect.SetCloak(m_CharacterInfo.B_Effecting(enum_CharacterEffect.Cloak));
        m_Effect.SetFreezed(m_CharacterInfo.B_Effecting(enum_CharacterEffect.Freeze));
        m_Effect.SetScaned(m_CharacterInfo.B_Effecting(enum_CharacterEffect.Scan));
        if (!m_IsDead)
            OnAliveTick(Time.deltaTime);
        else
            OnDeadTick(Time.deltaTime);
    }
    protected virtual void OnAliveTick(float deltaTime) { }
    protected virtual void OnDeadTick(float deltaTime) { }

    public virtual void ReviveCharacter(float reviveHealth = -1, float reviveArmor = -1)
    {
        if (!m_IsDead)
            return;
        OnRevive();
        m_Effect.OnReset();
        m_CharacterInfo.OnRevive();
        EntityHealth health = (m_Health as EntityHealth);
        health.OnSetStatus(reviveHealth == -1 ? health.m_BaseHealth : reviveHealth, reviveArmor == -1 ? health.m_StartArmor : reviveArmor);
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

    protected virtual void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
    {
        m_CharacterInfo.OnCharacterHealthChange(damageInfo,damageEntity,amountApply);
    }
    protected override void OnDead()
    {
        base.OnDead();
        m_CharacterInfo.OnDead();
        m_Effect.SetDeath();
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo(m_Effect.OnDeathEffect, 0, 1, GameConst.F_EntityDeadFadeTime, OnRecycle));
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterDead, this);
    }
    
    protected override void OnHealthChanged(enum_HealthChangeMessage type)
    {
        base.OnHealthChanged(type);
        m_Effect.OnHit(type);
        switch (type)
        {
            case enum_HealthChangeMessage.DamageArmor:
            case enum_HealthChangeMessage.DamageHealth:
                AudioManager.Instance.Play3DClip(m_EntityID, AudioManager.Instance.GetGameSFXClip(m_DamageClip), false, transform);
                break;
        }
    }

    protected override void OnRecycle()
    {
        base.OnRecycle();
        m_Effect.OnRecycle();
    }
    protected virtual void OnBattleStart()
    {

    }
    protected virtual void OnBattleFinish()
    {
        if (b_isSubEntity)
            OnDead();
    }

    class EntityCharacterEffectManager:ISingleCoroutine
    {
        Material m_NormalMaterial,m_EffectMaterial;
        List<Renderer> m_skins;
        bool m_cloaked;
        bool m_freezed;
        bool m_scanned;
        bool m_death;
        TSpecialClasses.ParticleControlBase m_Particles;
        public EntityCharacterEffectManager(Transform transform, List<Renderer> _skin)
        {
            m_Particles = new ParticleControlBase(transform);
            m_skins = _skin;
            m_NormalMaterial = m_skins[0].sharedMaterial;
            m_EffectMaterial = m_skins[0].material;
            OnReset();
        }

        public void OnReset()
        {
            m_Particles.Play();
            m_scanned = false;
            m_cloaked = false;
            m_death = false;
            m_freezed = false;
            CheckMaterials();
        }
        void CheckMaterials()
        {
            Shader mainShader = null;
            if (m_cloaked)
                mainShader = TEffects.SD_Cloak;
            if (m_freezed)
                mainShader = TEffects.SD_Ice;
            if (m_death)
                mainShader = TEffects.SD_Dissolve;
            if (m_scanned)
                mainShader = TEffects.SD_Scan;
            
            if (mainShader)
                m_EffectMaterial.shader = mainShader;

            m_skins.Traversal((Renderer renderer) => {
                renderer.material =  mainShader?m_EffectMaterial:m_NormalMaterial;
            });
        }

        public void SetDeath()
        {
            m_Particles.Stop();
            m_death = true;
            CheckMaterials();
            m_EffectMaterial.SetTexture(TEffects.ID_NoiseTex, TEffects.TX_Noise);
            m_EffectMaterial.SetFloat(TEffects.ID_Dissolve, 0);
            m_EffectMaterial.SetFloat(TEffects.ID_DissolveScale, .2f);
        }
        public void OnDeathEffect(float value) => m_EffectMaterial.SetFloat(TEffects.ID_Dissolve, value);

        public void SetScaned(bool _scaned)
        {
            if (m_scanned == _scaned)
                return;

            m_scanned = _scaned;
            CheckMaterials();
        }
        public void SetFreezed(bool _freezed)
        {
            if (m_freezed == _freezed)
                return;
            m_freezed = _freezed;
            CheckMaterials();

            if (!m_freezed)
                return;
            m_EffectMaterial.SetTexture(TEffects.ID_NoiseTex, TEffects.TX_Noise);
            m_EffectMaterial.SetColor("_IceColor", TCommon.GetHexColor("3DAEC5FF"));
            m_EffectMaterial.SetFloat("_Opacity", .5f);
        }
        public void SetCloak(bool _cloacked)
        {
            if (m_cloaked == _cloacked)
                return;

            m_cloaked = _cloacked;
            CheckMaterials();
            if (_cloacked)
            {
                
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
                {
                    m_EffectMaterial.SetFloat(TEffects.ID_Opacity, value);
                }, 1, .3f, .5f));
            }
            else
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => { m_EffectMaterial.SetFloat(TEffects.ID_Opacity, value);}, .3f, 1f, .3f, CheckMaterials));
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
                m_EffectMaterial.SetColor(TEffects.ID_Color, Color.Lerp(targetColor, Color.white, value)) ;
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
