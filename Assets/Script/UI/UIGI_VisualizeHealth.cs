using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TSpecialClasses;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_VisualizeHealth : UIT_GridItem {
    EntityBase m_AttachEntity;
    Image m_Normal,m_Elite;
    Image m_EliteExpire;
    bool b_showItem = false;
    float f_hideCheck;
    Graphic[] m_Graphics;

    ValueLerpSeconds m_HealthLerp;

    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Normal = rtf_Container.Find("Normal").GetComponent<Image>();
        m_Elite = rtf_Container.Find("Elite").GetComponent<Image>();
        m_EliteExpire = rtf_Container.Find("EliteExpire").GetComponent<Image>();

        m_Graphics = GetComponentsInChildren<Graphic>();
        rtf_RectTransform.anchoredPosition = UIConst.V2_UINumericVisualizeOffset;
        m_HealthLerp = new ValueLerpSeconds(0f, 1f, .5f, (float value) => { m_Normal.fillAmount = value; m_Elite.fillAmount = value; });
    }

    public void Play(EntityBase _attachTo)
    {
        m_AttachEntity = _attachTo;
        OnHide();

        ExpireEnermyPerkBase elitePerk = _attachTo.m_ControllType == enum_EntityType.AIWeaponHelper ? (_attachTo as EntityCharacterAI).m_EnermyPerk : null;
        bool isElite = elitePerk.IsElitePerk();
        m_EliteExpire.SetActivate(isElite);
        m_Elite.SetActivate(isElite);
        m_Normal.SetActivate(!isElite);
        if (isElite)
            m_EliteExpire.sprite = UIManager.Instance.m_ExpireSprites[elitePerk.GetExpireSprite()];

        m_HealthLerp.SetFinalValue(m_AttachEntity.m_Health.F_HealthMaxScale);
    }

    public void OnHide()
    {
        transform.SetActivate(false);
        b_showItem = false;
    }
    
    public void OnShow()
    {
        m_Graphics.Traversal((Graphic graphic) => { graphic.color = TCommon.ColorAlpha(graphic.color, 1f); });

        f_hideCheck = 2f;
        if (b_showItem)
            return;

        rtf_RectTransform.SetWorldViewPortAnchor(m_AttachEntity.transform.position,CameraController.MainCamera);
        transform.SetActivate(true);
        b_showItem = true;
    }

    private void Update()
    {
        if (!b_showItem)
            return;

        m_HealthLerp.SetLerpValue(m_AttachEntity.m_Health.F_HealthMaxScale);
        m_HealthLerp.TickDelta(Time.deltaTime);
        rtf_RectTransform.SetWorldViewPortAnchor(m_AttachEntity.transform.position, CameraController.MainCamera, Time.deltaTime*20f);

        f_hideCheck -= Time.deltaTime;
        if (f_hideCheck < 1f)
            m_Graphics.Traversal((Graphic graphic)=> { graphic.color = TCommon.ColorAlpha(graphic.color,f_hideCheck); });

        if (f_hideCheck < 0)
            OnHide();
    }
    
}
