using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampManager : GameManagerBase<CampManager>{
    Transform tf_PlayerStart;
    protected override void Awake()
    {
        base.Awake();
        tf_PlayerStart = transform.Find("PlayerStart");
    }
    protected override void Start()
    {
        base.Start();
        UIManager.Activate(false);
        CameraController.Instance.m_Effect.AddCameraEffect<PE_BloomSpecific>();
        GameObjectManager.PresetRegistCommonObject();
        GameObjectManager.SpawnEntityPlayer(new CPlayerLevelSave()).transform.SetPositionAndRotation(tf_PlayerStart.position,tf_PlayerStart.rotation);
        CameraController.Instance.RotateCamera(new Vector2(tf_PlayerStart.eulerAngles.y,0));
    }
    
    public void OnSceneItemInteract(enum_Scene scene)
    {
        GameObjectManager.RecycleAllObject();
        TSceneLoader.Instance.LoadScene(scene);
    }
}
