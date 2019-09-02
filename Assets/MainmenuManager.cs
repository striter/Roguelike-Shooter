using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class MainmenuManager : SimpleSingletonMono<MainmenuManager> {
    Dictionary<enum_Scene, MainmenuItem> m_ItemDic = new Dictionary<enum_Scene, MainmenuItem>();
    MainmenuItem m_currentSelectItem;
    protected override void Awake()
    {
        base.Awake();
        m_currentSelectItem = null;
        TouchInputManager.Instance.OnSingleTouch += OnTouch;
        Transform interactions = transform.Find("Interactions");
        TCommon.TraversalEnum((enum_Scene scene) => {
            switch (scene)
            {
                case enum_Scene.STest:
                case enum_Scene.Game:
                    {
                        MainmenuItem item = interactions.Find(scene.ToString()).GetComponent<MainmenuItem>();
                        item.Init(scene);
                        m_ItemDic.Add(scene, item);
                    }
                    break;
            }
        });
    }
    RaycastHit hit;
    void OnTouch(bool touch)
    {
        if (touch && Physics.Raycast(Camera.main.ScreenPointToRay(TouchInputManager.v3_SingleTouchPos), out hit, 100, GameLayer.Mask.I_Interact))
            OnItemClick(hit.collider.GetComponent<MainmenuItem>());
    }

    
    void OnItemClick(MainmenuItem item)
    {
        if (item == m_currentSelectItem)
        {
            TSceneLoader.Instance.LoadScene(item.m_scene);
            return;
        }
        if(m_currentSelectItem)
            m_currentSelectItem.Highlight(false);
        m_currentSelectItem = item;
        m_currentSelectItem.Highlight(true);
    }
}
