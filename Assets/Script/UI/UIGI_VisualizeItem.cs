using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_VisualizeItem : UIT_GridItem
{
    [SerializeField] Text m_itemName;

    Transform m_gameObject;
    InteractBattleBase m_interctBase;
    public void Play(string name, InteractBattleBase interctBase)
    {
        m_itemName.text = name;
        m_interctBase = interctBase;
        m_gameObject = m_interctBase.transform;
        m_interctBase.m_visualizeItem = this;
    }
    private void Update()
    {
        if (m_interctBase && m_interctBase.m_InteractEnable)
        {
            if (m_gameObject.gameObject.activeInHierarchy)
            {
                if (!rtf_Container.gameObject.activeInHierarchy)
                    rtf_Container.SetActivate(true);
                float deltaTime = Time.deltaTime;
                rtf_RectTransform.SetWorldViewPortAnchor(m_gameObject.position, CameraController.MainCamera);
            }
            else
            {
                if (rtf_Container.gameObject.activeInHierarchy)
                    rtf_Container.SetActivate(false);
            }
        }
        else
        {
            if (rtf_Container.gameObject.activeInHierarchy)
                rtf_Container.SetActivate(false);
        }
    }
}
