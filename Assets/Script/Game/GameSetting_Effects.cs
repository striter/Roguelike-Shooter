using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;

namespace GameSetting
{
    public enum enum_Effects
    {
        Invalid = -1,
        Cloak,
        Freeze,
        Scan,
        Death,
    }

    public static class TEffects
    {
        public static readonly Dictionary<enum_Effects, Shader> SD_ExtraEffects = new Dictionary<enum_Effects, Shader>()
    {
        { enum_Effects.Cloak,Shader.Find("Game/Effect/Cloak")},
        { enum_Effects.Death,Shader.Find("Game/Effect/BloomSpecific/Bloom_Dissolve")},
        { enum_Effects.Scan,Shader.Find("Game/Extra/ScanEffect")},
        { enum_Effects.Freeze, Shader.Find("Game/Effect/Ice")}
    };
        public static readonly Texture TX_Noise = TResources.GetNoiseTex();
        public static readonly int ID_Color = Shader.PropertyToID("_Color");
        public static readonly int ID_Dissolve = Shader.PropertyToID("_DissolveAmount");
        public static readonly int ID_DissolveScale = Shader.PropertyToID("_DissolveScale");
        public static readonly int ID_Opacity = Shader.PropertyToID("_Opacity");
        public static readonly int ID_NoiseTex = Shader.PropertyToID("_NoiseTex");
    }
    public class EntityCharacterSkinEffectManager : ICoroutineHelperClass
    {
        Material m_NormalMaterial, m_EffectMaterial;
        List<Renderer> m_skins;
        public Renderer m_MainSkin => m_skins[0];
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

        public void SetScaned(bool _scaned) => CheckMaterials(_scaned ? enum_Effects.Scan : enum_Effects.Invalid);
        public void SetFreezed(bool _freezed)
        {
            CheckMaterials(_freezed ? enum_Effects.Freeze : enum_Effects.Invalid);

            if (!_freezed)
                return;
            m_EffectMaterial.SetTexture(TEffects.ID_NoiseTex, TEffects.TX_Noise);
            m_EffectMaterial.SetColor("_IceColor", TCommon.GetHexColor("3DAEC5FF"));
            m_EffectMaterial.SetFloat("_Opacity", .5f);
        }
        public void SetCloak(bool _cloacked, float clockOpacity = .3f, float lerpDuration = .5f)
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
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => { m_EffectMaterial.SetFloat(TEffects.ID_Opacity, value); }, clockOpacity, 1f, lerpDuration, ()=>SetCloak(false)));
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