using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampManager : SimpleSingletonMono<CampManager> {
    Dictionary<enum_Scene, CampInteract> m_CampItemDic = new Dictionary<enum_Scene, CampInteract>();
    private void Start()
    {
        CameraEffectManager.AddCameraEffect<PE_BloomSpecific>();
        TCommon.TraversalEnum((enum_Scene scene) => {
            switch (scene)
            {
                case enum_Scene.Game:
                    {
                        CampInteractScene item = CampEnvironment.Instance.tf_Interactions.Find("Scene/"+scene.ToString()).GetComponent<CampInteractScene>();
                        item.Play(scene,OnItemClick);
                        m_CampItemDic.Add(scene, item);
                    }
                    break;
            }
        });
    }
    
    void OnItemClick(enum_Scene scene)
    {
        TSceneLoader.Instance.LoadScene(scene);
    }
}
