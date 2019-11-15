using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class TestGameManager : GameManagerBase
{
    public enum_Style m_EnermiesType;
    protected ParticleSystem[] m_Particles { get; private set; }
    protected SFXRelativeBase[] m_relativeSFXs;
    protected override void Start()
    {
        base.Start();
        m_relativeSFXs = GetComponentsInChildren<SFXRelativeBase>();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Init(); });
        m_Particles = transform.GetComponentsInChildren<ParticleSystem>();

        if(m_EnermiesType!= enum_Style.Invalid)
            GameObjectManager.RegistStyledIngameEnermies(m_EnermiesType, enum_StageLevel.Veteran);

        AttachPlayerCamera( GameObjectManager.SpawnEntityPlayer(new CBattleSave()).tf_Head);
        InitPostEffects( enum_Style.Iceland);
    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            Time.timeScale = Time.timeScale == 1 ? .1f : 1f;
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.OnPlay(); });
            m_Particles.Traversal((ParticleSystem particle) => { particle.Play(true); });
        }
        else if (Input.GetKeyDown(KeyCode.Equals))
        {
            m_relativeSFXs.Traversal((SFXRelativeBase sfxRelative) => { sfxRelative.OnStop(); });
            m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(true); });
        }
    }
}
