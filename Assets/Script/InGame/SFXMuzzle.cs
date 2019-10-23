
using UnityEngine;

public class SFXMuzzle : SFXParticles
{
    protected override bool m_Loop => false;
    protected override bool m_AutoStop => false;
    SFXRelativeLight[] m_lights;
    float lightCheck;
    bool b_lightPlaying;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_lights = GetComponentsInChildren<SFXRelativeLight>();

    }
    public override void Play(int sourceID, float duration = 0,float delayDuration=0)
    {
        base.Play(sourceID, duration,delayDuration);
        lightCheck = .2f;
        b_lightPlaying = true;
    }

    protected override void Update()
    {
        base.Update();
        lightCheck -= Time.deltaTime;
        if (b_lightPlaying&&lightCheck < 0)
        {
            b_lightPlaying = false;
            m_lights.Traversal((SFXRelativeLight light) => { light.OnStop(); });
        }
    }
}
