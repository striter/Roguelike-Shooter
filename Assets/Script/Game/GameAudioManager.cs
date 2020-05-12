using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(AudioSource))]
public class GameAudioManager : AudioManager 
{
    public static new GameAudioManager Instance { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    Dictionary<enum_GameMusic, AudioClip> m_GameMusic = new Dictionary<enum_GameMusic, AudioClip>();
    public override void Init()
    {
        base.Init();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnGameLoadBegin, OnStageBeginLoad);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Add<bool>(enum_BC_GameStatus.OnGameFinish, OnGameFinish);
    }
    public override void Destroy()
    {
        base.Destroy();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnGameLoadBegin, OnStageBeginLoad);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Remove<bool>(enum_BC_GameStatus.OnGameFinish, OnGameFinish);
    }

    void OnStageBeginLoad()
    {
        TCommon.TraversalEnum((enum_GameMusic music) =>
        {
            if (!(music > enum_GameMusic.StyledStart && music < enum_GameMusic.StyledEnd))
                return;
            if (m_GameMusic.ContainsKey(music))
                m_GameMusic.Remove(music);

            AudioClip clip = TResources.GetGameBGM_Styled(music,GameManager.Instance.m_GameLevel.m_GameStyle);
            if (!clip)
                return;
            m_GameMusic.Add(music, clip);
        });
    }
    void OnStageStart()
    {
        PlayBGM(enum_GameMusic.Relax, true);
    }
    void OnBattleStart()
    {
        PlayBGM( enum_GameMusic.Fight, true);
    }
    void OnBattleFinish()
    {
        Stop();
    }
    void OnGameFinish(bool win) => SetBGPitch(.8f);

    void PlayBGM(enum_GameMusic music, bool loop)
    {
        if (m_GameMusic.ContainsKey(music))
            SwitchBackground(m_GameMusic[music], loop);
        else
            Debug.LogWarning("None Music Found Of:" + music);
    }
    public void Stop() => StopBackground();
}
