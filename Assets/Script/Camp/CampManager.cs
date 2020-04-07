using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampManager : GameManagerBase
{
    public static CampManager nInstance;
    public static new CampManager Instance => nInstance;
    Transform tf_PlayerStart;
    public Transform tf_PlayerCameraAttach { get; private set; }
    protected override void Awake()
    {
        nInstance = this;
        base.Awake();
        tf_PlayerStart = transform.Find("PlayerStart");
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        nInstance = null;
    }
    protected override void Start()
    {
        base.Start();
        InitGameEffects( enum_GameStyle.Invalid,GameRenderData.Default());
        EntityCharacterPlayer player = GameObjectManager.SpawnEntityPlayer(new PlayerSaveData( enum_PlayerCharacter.Beth, enum_PlayerWeapon.P92),tf_PlayerStart.position, tf_PlayerStart.rotation);
        tf_PlayerCameraAttach = player.transform;
        AttachPlayerCamera(tf_PlayerCameraAttach);
        CampAudioManager.Instance.PlayBGM(enum_CampMusic.Relax);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCampStart);
    }
    
    public void OnSceneItemInteract()
    {
        OnPortalEnter(1f,tf_PlayerCameraAttach, () => {
            LoadingManager.Instance.ShowLoading(enum_Stage.Rookie);
            SwitchScene( enum_Scene.Game);
        });
    }

    public void OnCreditStatus(float creditChange)
    {
        GameDataManager.OnCreditStatus(creditChange);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampDataStatus);
    }

    public void OnTechPointStatus(float techPoint)
    {
        GameDataManager.OnTechPointStatus(techPoint);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampDataStatus);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            OnCreditStatus(100);
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            OnCreditStatus(-50);
        if (Input.GetKeyDown(KeyCode.Equals))
            OnTechPointStatus(20);
        if (Input.GetKeyDown(KeyCode.Minus))
            OnTechPointStatus(-15);
        if (Input.GetKeyDown(KeyCode.Equals))
            OnSceneItemInteract();
    }
#endif
}
