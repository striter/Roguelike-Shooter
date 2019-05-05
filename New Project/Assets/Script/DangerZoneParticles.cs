
public class DangerZoneParticles : DangerZoneBase
{
    EnviormentParticleBase m_particleBase;
    protected override void Awake()
    {
        m_particleBase = gameObject.AddComponent<EnviormentParticleBase>();
        base.Awake();
    }
    protected override void OnSetActivate(bool activate)
    {
        base.OnSetActivate(activate);
        m_particleBase.SetPlay(activate);
    }
}
