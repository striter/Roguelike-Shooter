
using UnityEngine;

public class SFXMuzzle : SFXParticles
{
    SFXRelativeLight[] m_lights;
    float lightCheck;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_lights = GetComponentsInChildren<SFXRelativeLight>();

    }
    public override void Play(int sourceID, float duration = 0)
    {
        base.Play(sourceID, duration);
        lightCheck = .3f;

        m_lights.Traversal((SFXRelativeLight light) => { light.Play(); });
    }

    protected override void Update()
    {
        base.Update();
        lightCheck -= Time.deltaTime;
        if (lightCheck < 0)
            m_lights.Traversal((SFXRelativeLight light) => { light.Stop(); });
    }
}
