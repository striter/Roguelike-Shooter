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
    protected virtual CharacterInfoManager GetEntityInfo() => new CharacterInfoManager(this, OnReceiveDamage, OnExpireChange);
    public virtual float m_baseMovementSpeed => F_MovementSpeed;
    public override bool B_IsCharacter => true;
    public override void Init(int _poolIndex)
    {
        base.Init(_poolIndex);
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        m_Effect = new EntityCharacterEffectManager(tf_Model.Find("Skin").GetComponentsInChildren<Renderer>());
        m_CharacterInfo = GetEntityInfo();
    }

    public override void OnActivate(enum_EntityFlag _flag)
    {
       base.OnActivate(_flag);
       m_CharacterInfo.OnActivate();
        m_Effect.OnReset();
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

        m_Effect.SetCloak(m_CharacterInfo.B_Effecting( enum_CharacterEffect.Cloak));
    }

    protected override bool OnReceiveDamage(DamageInfo damageInfo)
    {
        if (base.OnReceiveDamage(damageInfo))
        {
            damageInfo.m_detail.m_BaseBuffApply.Traversal((int buffIndex) => { m_CharacterInfo.AddBuff(damageInfo.m_detail.I_SourceID, buffIndex); });
            if (damageInfo.m_detail.m_DamageEffect != enum_CharacterEffect.Invalid) m_CharacterInfo.OnSetEffect(damageInfo.m_detail.m_DamageEffect, damageInfo.m_detail.m_EffectDuration);

            return true;
        }

        return false;
    }

    protected override void OnDead()
    {
        base.OnDead();
        m_CharacterInfo.OnDeactivate();
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo(m_Effect.OnRecycleEffect, 0, 1, GameConst.F_EntityDeadFadeTime, OnRecycle));
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterDead,this);
    }
    protected override void OnHealthChanged(enum_HealthChangeMessage type)
    {
        m_Effect.OnHit(type);
    }
    protected override void OnRecycle()
    {
        base.OnRecycle();
        m_Effect.OnRecycle();
    }

    class EntityCharacterEffectManager:ISingleCoroutine
    {
        Shader SD_Opaque;
        static readonly Shader SD_Transparent = Shader.Find("Game/Common/Diffuse_Texture_Transparent");
        static readonly int ID_Color = Shader.PropertyToID("_Color");
        static readonly int ID_Amount1=Shader.PropertyToID("_Amount1");
        static readonly int ID_Alpha = Shader.PropertyToID("_Alpha");

        Material[] m_Materials;

        public EntityCharacterEffectManager(Renderer[] _skin)
        {
            Material materialBase= _skin[0].materials[0];
            Material materialEffect = _skin[0].materials[1];
            m_Materials = new Material[2] { materialBase, materialEffect };
            _skin.Traversal((Renderer renderer) => { renderer.materials = m_Materials;  });
            SD_Opaque = materialBase.shader;
        }

        public void OnReset()
        {
            m_Materials[0].shader = SD_Opaque;
            m_Materials.Traversal((Material mat) => { mat.SetFloat(ID_Amount1, 0); });
        }

        public void OnRecycleEffect(float value)
        {
            m_Materials[0].SetFloat(ID_Amount1, value);
            m_Materials[1].SetFloat(ID_Amount1, value);
        }

        bool m_cloaked;
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
                }, 1, .3f, .5f, null));
            }
            else
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
                {
                    m_Materials[0].SetFloat(ID_Alpha, value);
                }, .3f, 1f, .3f, () =>
                {
                    m_Materials[0].shader = SD_Opaque;
                }));
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
}
