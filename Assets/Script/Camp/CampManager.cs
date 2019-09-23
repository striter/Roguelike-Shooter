using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampManager : GameManagerBase<CampManager>{
    Transform tf_PlayerStart;
    Dictionary<enum_Scene, CampInteract> m_CampItemDic = new Dictionary<enum_Scene, CampInteract>();

    protected override void Awake()
    {
        base.Awake();
        tf_PlayerStart = transform.Find("PlayerStart");
        TCommon.TraversalEnum((enum_Scene scene) => {
            switch (scene)
            {
                case enum_Scene.Game:
                    {
                        CampInteractScene item = CampEnvironment.Instance.tf_Interactions.Find("Scene/" + scene.ToString()).GetComponent<CampInteractScene>();
                        item.Play(scene, OnItemInteract);
                        m_CampItemDic.Add(scene, item);
                    }
                    break;
            }
        });
    }
    protected override void Start()
    {
        base.Start();
        UIManager.Activate(false);
        CameraEffectManager.AddCameraEffect<PE_BloomSpecific>();
        GameObjectManager.PresetRegistCommonObject();
        GameObjectManager.SpawnEntityPlayer(new CPlayerGameSave()).transform.SetPositionAndRotation(tf_PlayerStart.position,tf_PlayerStart.rotation);
        CameraController.Instance.RotateCamera(new Vector2(tf_PlayerStart.eulerAngles.y,0));
    }
    
    void OnItemInteract(enum_Scene scene)
    {
        GameObjectManager.RecycleAllObject();
        TSceneLoader.Instance.LoadScene(scene);
    }
}
