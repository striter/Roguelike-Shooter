using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour
{
    protected ParticleSystem[] m_Particles { get; private set; }
    protected SFXRelativeBase[] m_relativeSFXs;
    protected void Start()
    {
        m_relativeSFXs = GetComponentsInChildren<SFXRelativeBase>();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Init(); });
        m_Particles = transform.GetComponentsInChildren<ParticleSystem>();
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
