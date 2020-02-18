using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGI_MapEntityLocation : UIT_GridItem {

    EntityBase m_Target;
    public void Play(EntityBase entity)
    {
        m_Target = entity;
        UpdatePos();
    }

    public void UpdatePos()
    {
        if(m_Target)
        rtf_RectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(m_Target.transform.position);
    }
}
