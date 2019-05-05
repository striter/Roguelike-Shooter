using UnityEngine;

public class EnviormentParticleBase : MonoBehaviour
{
    public bool B_PlayOnAwake = false;
    ParticleSystem[] m_particles;
    protected virtual void Awake()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();

        SetPlay(B_PlayOnAwake);
    }
    public virtual void SetPlay(bool play)
    {
        if (play)
            TCommon.TraversalArray(m_particles, (ParticleSystem particle) => { particle.Play(); });
        else
            TCommon.TraversalArray(m_particles, (ParticleSystem particle) => { particle.Stop(); });
    }
}
