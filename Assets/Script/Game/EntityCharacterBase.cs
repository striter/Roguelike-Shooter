using GameSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using TSpecialClasses;
using UnityEngine;

public class EntityCharacterBase : EntityBase
{
    public int I_MaxHealth;
    public int I_DefaultArmor;
    public float F_MovementSpeed;
    public float F_AttackSpread;
    public Transform tf_Model { get; private set; }
    public Transform tf_Head { get; private set; }
    public virtual Transform tf_Weapon=>null;
    public CharacterInfoManager m_CharacterInfo { get; private set; }
    public virtual MeshRenderer m_WeaponSkin { get; private set; }
    public EntityCharacterSkinEffectManager m_CharacterSkinEffect { get; private set; }
    public virtual Vector3 m_PrecalculatedTargetPos(float time)=> tf_Head.position;
    public int m_SpawnerEntityID { get; private set; }
    public bool b_isSubEntity => m_SpawnerEntityID != -1;
    protected virtual CharacterInfoManager GetEntityInfo() => new CharacterInfoManager(this, m_HitCheck.TryHit, OnExpireChange);
    
    public virtual float m_baseMovementSpeed => F_MovementSpeed;
    protected override float DamageReceiveMultiply => m_CharacterInfo.F_DamageReceiveMultiply;
    protected override float HealReceiveMultiply => m_CharacterInfo.F_HealReceiveMultiply;

    public new EntityHealth m_Health=>base.m_Health as EntityHealth;
    protected override HealthBase GetHealthManager()=> new EntityHealth(this, OnHealthChanged);
    TimeCounter m_DeadCounter = new TimeCounter(GameConst.F_EntityDeadFadeTime,true);
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
        m_CharacterSkinEffect = new EntityCharacterSkinEffectManager(tf_Model,renderers);
        m_CharacterInfo = GetEntityInfo();
    }

    protected override void OnPoolItemEnable()
    {
        base.OnPoolItemEnable();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        m_CharacterInfo.OnActivate();
    }

    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        m_CharacterSkinEffect.OnDisable();
    }
    public bool m_TargetAvailable =>  !m_IsDead;
    protected override void EntityActivate(enum_EntityFlag flag, float startHealth = 0)
    {
        base.EntityActivate(flag, startHealth);
        m_CharacterSkinEffect.OnReset();
    }

    protected void OnMainCharacterActivate(enum_EntityFlag _flag)
    {
        EntityActivate(_flag,I_MaxHealth);
        m_SpawnerEntityID = -1;
    }
    public virtual void OnSubCharacterActivate(enum_EntityFlag _flag, int _spawnerID , float startHealth )
    {
        EntityActivate(_flag, startHealth);
        m_SpawnerEntityID = _spawnerID;
    }

    protected virtual void OnExpireChange(){ }

    private void Update()
    {
        if (!m_Activating)
            return;

        m_CharacterInfo.Tick(Time.deltaTime);

        if (!m_IsDead)
            OnAliveTick(Time.deltaTime);
        else
            OnDeadTick(Time.deltaTime);
    }
    protected virtual void OnAliveTick(float deltaTime) { }
    protected virtual void OnDeadTick(float deltaTime)
    {
        if (m_DeadCounter.m_Timing)
        {
            m_DeadCounter.Tick(deltaTime);
            m_CharacterSkinEffect.SetDeathEffect(1f-m_DeadCounter.m_TimeLeftScale);
            return;
        }
        if (!m_DeadCounter.m_Timing)
            DoRecycle();
    }

    public virtual void ReviveCharacter()
    {
        if (!m_IsDead)
            return;
        OnRevive();
        m_CharacterSkinEffect.OnReset();
        m_CharacterInfo.OnRevive();
        EntityHealth health = (m_Health as EntityHealth);
        health.OnSetStatus( health.m_MaxHealth,health.m_MaxArmor);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterRevive, this);
    }

    protected override bool OnReceiveDamage(DamageInfo damageInfo, Vector3 damageDirection)
    {
        if (!base.OnReceiveDamage(damageInfo, damageDirection))
            return false;

        damageInfo.m_BaseBuffApply.Traversal((SBuff buffInfo) => { m_CharacterInfo.AddBuff(damageInfo.m_SourceID, buffInfo); });

        return true;
    }

    protected virtual void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
    {
        m_CharacterInfo.OnCharacterHealthChange(damageInfo,damageEntity,amountApply);
    }
    protected override void OnDead()
    {
        base.OnDead();
        m_CharacterInfo.OnDead();
        m_CharacterSkinEffect.SetDeath();
        m_DeadCounter.Replay();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterDead, this);
    }
    public override void DoRecycle()
    {
        base.DoRecycle();
        m_CharacterInfo.OnRecycle();
    }

    protected override void OnHealthChanged(enum_HealthChangeMessage type)
    {
        base.OnHealthChanged(type);
        m_CharacterSkinEffect.OnHit(type);
        switch (type)
        {
            case enum_HealthChangeMessage.DamageArmor:
            case enum_HealthChangeMessage.DamageHealth:
                AudioManager.Instance.Play3DClip(m_EntityID, AudioManager.Instance.GetGameSFXClip(m_DamageClip), false, transform);
                break;
        }
    }
    
    protected virtual void OnBattleFinish()
    {
        if (b_isSubEntity)
            OnDead();
    }

    public class EntityCharacterSkinEffectManager:ICoroutineHelperClass
    {
        Material m_NormalMaterial,m_EffectMaterial;
        List<Renderer> m_skins;
        public Renderer m_MainSkin =>m_skins[0];
        enum_Effects m_Effect = enum_Effects.Invalid;
        TSpecialClasses.ParticleControlBase m_Particles;
        MaterialPropertyBlock m_NormalProperty = new MaterialPropertyBlock();
        public EntityCharacterSkinEffectManager(Transform particleTrans, List<Renderer> _skin)
        {
            m_Particles = new ParticleControlBase(particleTrans);
            m_skins = _skin;
            m_NormalMaterial = m_skins[0].sharedMaterial;
            m_EffectMaterial = m_skins[0].material;
            OnReset();
        }

        public void OnReset()
        {
            this.StopAllSingleCoroutines();
            m_Particles.Play();
            CheckMaterials(enum_Effects.Invalid);
        }
        void CheckMaterials(enum_Effects effect)
        {
            if (m_Effect == effect)
                return;
            m_Effect = effect;
            bool extraEffect = m_Effect != enum_Effects.Invalid;

            m_skins.Traversal((Renderer renderer) => {
                if (extraEffect)
                    m_EffectMaterial.shader = TEffects.SD_ExtraEffects[m_Effect];
                renderer.material = extraEffect ? m_EffectMaterial : m_NormalMaterial;
            });
        }

        public void SetDeath()
        {
            m_Particles.Stop();
            CheckMaterials(enum_Effects.Death);
            m_EffectMaterial.SetTexture(TEffects.ID_NoiseTex, TEffects.TX_Noise);
            m_EffectMaterial.SetFloat(TEffects.ID_Dissolve, 0);
            m_EffectMaterial.SetFloat(TEffects.ID_DissolveScale, .4f);
        }
        public void SetDeathEffect(float value)
        {
            m_EffectMaterial.SetFloat(TEffects.ID_Dissolve, value);
        }

        public void SetScaned(bool _scaned)=>CheckMaterials(_scaned? enum_Effects.Scan: enum_Effects.Invalid);
        public void SetFreezed(bool _freezed)
        {
            CheckMaterials(_freezed ? enum_Effects.Freeze : enum_Effects.Invalid);

            if (!_freezed)
                return;
            m_EffectMaterial.SetTexture(TEffects.ID_NoiseTex, TEffects.TX_Noise);
            m_EffectMaterial.SetColor("_IceColor", TCommon.GetHexColor("3DAEC5FF"));
            m_EffectMaterial.SetFloat("_Opacity", .5f);
        }
        public void SetCloak(bool _cloacked,float clockOpacity=.3f ,float lerpDuration=.5f)
        {
            CheckMaterials(_cloacked ? enum_Effects.Cloak : enum_Effects.Invalid);
            if (_cloacked)
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
                {
                    m_EffectMaterial.SetFloat(TEffects.ID_Opacity, value);
                }, 1, clockOpacity, lerpDuration));
            }
            else
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => { m_EffectMaterial.SetFloat(TEffects.ID_Opacity, value);}, clockOpacity, 1f, lerpDuration, OnReset ));
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
                m_NormalProperty.SetColor(TEffects.ID_Color, Color.Lerp(targetColor, Color.white, value));
                m_skins.Traversal((Renderer renderer) => { renderer.SetPropertyBlock(m_NormalProperty); });
            }, 0, 1, 1f));
        }

        public void OnDisable()
        {
            this.StopAllSingleCoroutines();
        }
    }

}
