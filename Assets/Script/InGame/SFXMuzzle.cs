
using UnityEngine;

public class SFXMuzzle : SFXParticles
{
    SFXRelativeLight[] m_lights;
    float lightCheck;
    bool b_lightPlaying;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_lights = GetComponentsInChildren<SFXRelativeLight>();

    }
    public override void Play(int sourceID, float duration = 0)
    {
        base.Play(sourceID, duration);
        lightCheck = .2f;
        b_lightPlaying = true;
        m_lights.Traversal((SFXRelativeLight light) => { light.Play(); });
    }

    protected override void Update()
    {
        base.Update();
        lightCheck -= Time.deltaTime;
        if (b_lightPlaying&&lightCheck < 0)
        {
            m_lights.Traversal((SFXRelativeLight light) => { light.Stop(); });
            b_lightPlaying = false;
        }
    }
}
