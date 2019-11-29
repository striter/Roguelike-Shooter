using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampAudioManager : AudioManager {

    protected static CampAudioManager ninstance;
    public static new CampAudioManager Instance => ninstance;
    protected override void Awake()
    {
        base.Awake();
        ninstance = this;
    }
    Dictionary<enum_CampMusic, AudioClip> m_CampMusic = new Dictionary<enum_CampMusic, AudioClip>();
    public override void Init()
    {
        base.Init();
        TCommon.TraversalEnum((enum_CampMusic music) => {
            m_CampMusic.Add(music, TResources.GetCampBGM(music));
        });
    }

    public void PlayBGM(enum_CampMusic music) => SwitchBackground(m_CampMusic[music],true);
}
