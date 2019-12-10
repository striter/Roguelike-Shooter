using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;
using System.Collections.Generic;
using System.Linq;

public class EntityCharacterBase : EntityBase, ISingleCoroutine
{
    public int I_DefaultArmor;
    public float F_MovementSpeed;
    public float F_AttackSpread;
    public Transform tf_Model { get; private set; }
    public Transform tf_Head { get; private set; }
    public virtual Transform tf_Weapon=>null;
    public virtual Transform tf_WeaponModel => null;
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

    protected virtual enum_GameVFX m_DamageClip => enum_GameVFX.EntityDamage;
    protected virtual enum_GameVFX m_ReviveClip => enum_GameVFX.PlayerRevive;

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        List<Renderer> renderers = tf_Model.Find("Skin").GetComponentsInChildren<Renderer>().ToList();
        if(ExtraRendererOnlyAvailableFor107WhatAwkwardRequirementWhyNotPutThemIntoOneModelCauseOutModelArtistCantDoItHaveToCreateThisIntoMyCodeLikeABunchOfShitForFuckingRealSucks)
        renderers.Add(ExtraRendererOnlyAvailableFor107WhatAwkwardRequirementWhyNotPutThemIntoOneModelCauseOutModelArtistCantDoItHaveToCreateThisIntoMyCodeLikeABunchOfShitForFuckingRealSucks);
        m_Effect = new EntityCharacterEffectManager(renderers);
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
        if (!m_Health.b_IsDead)
            OnAliveTick(Time.deltaTime);
        else
            OnDeadTick(Time.deltaTime);
    }
    protected virtual void OnAliveTick(float deltaTime) { }
    protected virtual void OnDeadTick(float deltaTime) { }

    public virtual void ReviveCharacter(float reviveHealth = -1, float reviveArmor = -1)
    {
        if (!m_Health.b_IsDead)
            return;
        OnRevive();
        m_Effect.OnReset();
        m_CharacterInfo.OnRevive();
        EntityHealth health = (m_Health as EntityHealth);
        health.OnSetHealth(reviveHealth == -1 ? health.m_BaseHealth : reviveHealth, reviveArmor == -1 ? health.m_StartArmor : reviveArmor);
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
        if (m_Health.b_IsDead)
            TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterDead, this);
    }
    
    protected override void OnHealthStatus(enum_HealthChangeMessage type)
    {
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
        Shader SD_Base;
        static readonly Shader SD_DeathOutline=Shader.Find("Game/Effect/BloomSpecific/Bloom_DissolveEdge");
        static readonly Shader SD_Scan = Shader.Find("Game/Extra/ScanEffect");
        static readonly Shader SD_Ice = Shader.Find("Game/Effect/Ice");
        static readonly Shader SD_Cloak = Shader.Find("Game/Effect/Cloak");
        static readonly int ID_Color = Shader.PropertyToID("_Color");
        static readonly int ID_Amount1=Shader.PropertyToID("_Amount1");
        static readonly int ID_Opacity = Shader.PropertyToID("_Opacity");
        static readonly Texture TX_Distort = TResources.GetNoiseTex();
        List<Renderer> m_skins;
        Material m_MatBase,m_MatExtra;
        Material[] m_Materials;
        bool m_mainCloack;
        bool m_mainFreeze;
        bool m_extraScan;
        bool m_extraDeath;
        public EntityCharacterEffectManager(List<Renderer> _skin)
        {
            m_MatBase = _skin[0].material;
            SD_Base = m_MatBase.shader;
            m_MatExtra = new Material(m_MatBase);
            m_Materials = new Material[1] { m_MatBase };
            m_skins = _skin;
            OnReset();
        }

        public void OnReset()
        {
            m_extraScan = false;
            m_mainCloack = false;
            m_extraDeath = false;
            m_mainFreeze = false;
            CheckMaterials();
            m_Materials.Traversal((Material mat) => { mat.SetFloat(ID_Amount1, 0); });
        }
        void CheckMaterials()
        {
            Shader mainShader = SD_Base;
            Shader extraShader = null;
            if (m_mainCloack)
                mainShader = SD_Cloak;
            if (m_mainFreeze)
                mainShader = SD_Ice;
            if (m_extraDeath)
                extraShader = SD_DeathOutline;
            if (m_extraScan)
                extraShader = SD_Scan;

            m_MatBase.shader = mainShader;
            if (extraShader == null)
            {
                m_Materials = new Material[1] { m_MatBase };
            }
            else
            {
                m_MatExtra.shader = extraShader;
                m_Materials = new Material[2] { m_MatBase, m_MatExtra };
            }
            m_skins.Traversal((Renderer renderer) => { renderer.materials = m_Materials; });
        }

        public void SetDeath()
        {
            m_extraDeath = true;
            CheckMaterials();
        }
        public void OnDeathEffect(float value) => m_Materials.Traversal((Material mat) => { mat.SetFloat(ID_Amount1, value); });

        public void SetScaned(bool _scaned)
        {
            if (m_extraScan == _scaned)
                return;

            m_extraScan = _scaned;
            CheckMaterials();
        }
        public void SetFreezed(bool _freezed)
        {
            if (m_mainFreeze == _freezed)
                return;
            m_mainFreeze = _freezed;
            CheckMaterials();

            if (!m_mainFreeze)
                return;
            m_MatBase.SetColor("_IceColor", TCommon.GetHexColor("3DAEC5FF"));
            m_MatBase.SetTexture("_DistortTex", TX_Distort);
            m_MatBase.SetFloat("_Opacity", .5f);
        }
        public void SetCloak(bool _cloacked)
        {
            if (m_mainCloack == _cloacked)
                return;

            m_mainCloack = _cloacked;
            CheckMaterials();
            if (_cloacked)
            {
                m_MatBase.SetTexture("_DistortTex", TX_Distort);
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
                {
                    m_Materials[0].SetFloat(ID_Opacity, value);
                }, 1, .3f, .5f));
            }
            else
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>{ m_MatBase.SetFloat(ID_Opacity, value); }, .3f, 1f, .3f, () => { m_MatBase.shader = SD_Base; }));
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
                m_MatBase.SetColor(ID_Color, Color.Lerp(targetColor, Color.white, value));
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


    #region ?
    public Renderer ExtraRendererOnlyAvailableFor107WhatAwkwardRequirementWhyNotPutThemIntoOneModelCauseOutModelArtistCantDoItHaveToCreateThisIntoMyCodeLikeABunchOfShitForFuckingRealSucks;
    #endregion
}
