using GameSetting;
using TPhysics;
using UnityEngine;

public class SFXProjectileDestroyableSpreadDelay : SFXProjectileDestroyableSpread,ISingleCoroutine {
    public float F_DelayDuration;
    protected override PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) => null;
    protected ParticleSystem[] m_Particles;
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
    }
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, DamageBuffInfo buffInfo)
    {
        targetPosition = EnviormentManager.NavMeshPosition(targetPosition) + Vector3.up * .5f;
        base.Play(sourceID, direction, targetPosition, buffInfo);
        m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
        transform.localScale = Vector3.zero;
        B_SimulatePhysics = false;
        this.StartSingleCoroutine(1, TIEnumerators.PauseDel(F_DelayDuration, () => {
            transform.localScale = Vector3.one;
            transform.position = targetPosition;
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
            B_SimulatePhysics = true;
            m_Particles.Traversal((ParticleSystem particle) => { particle.Play(); });
        }));
    }
    protected override void SpawnIndicator(Vector3 position, Vector3 direction, float duration)
    {
        duration = F_DelayDuration;
        base.SpawnIndicator(position, direction, duration);
    }
    public void OnDisable()
    {
        this.StopSingleCoroutine(1);
    }
}
