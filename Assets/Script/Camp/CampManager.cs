using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampManager : GameManagerBase{
    public static CampManager nInstance;
    public static new CampManager Instance => nInstance;
    
    Transform tf_PlayerStart;
    public Transform tf_PlayerHead { get; private set; }
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
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BloomSpecific>();
        GameObjectManager.PresetRegistCommonObject();
        EntityCharacterPlayer player = GameObjectManager.SpawnEntityPlayer(new CPlayerGameSave());
        player.transform.SetPositionAndRotation(tf_PlayerStart.position, tf_PlayerStart.rotation);
        tf_PlayerHead = player.tf_Head;
        CameraController.Instance.RotateCamera(new Vector2(tf_PlayerStart.eulerAngles.y,0));
    }
    
    public void OnSceneItemInteract(enum_Scene scene)
    {
        SwitchScene(scene);
    }
}
