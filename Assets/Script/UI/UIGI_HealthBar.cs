using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIGI_HealthBar : UIT_GridItem {
    EntityBase m_AttachEntity;
    Text m_Name;
    Slider m_HealthBar1,m_HealthBar2,m_HealthBar3;
    bool b_showItem = false;
    float f_hideCheck;
    Graphic[] m_Graphics;
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        m_Name = tf_Container.Find("Name").GetComponent<Text>();
        m_HealthBar1 = tf_Container.Find("HealthBar1").GetComponent<Slider>();
        m_HealthBar2 = tf_Container.Find("HealthBar2").GetComponent<Slider>();
        m_HealthBar3 = tf_Container.Find("HealthBar3").GetComponent<Slider>();
        m_Graphics = GetComponentsInChildren<Graphic>();
    }

    public void AttachItem(EntityBase _attachTo)
    {
        m_AttachEntity = _attachTo;
        OnHide();
    }

    public void OnHide()
    {
        transform.SetActivate(false);
        b_showItem = false;
    }

    float m_currnetHealthValue;
    public void OnShow()
    {
        m_Graphics.Traversal((Graphic graphic) => { graphic.color = TCommon.ColorAlpha(graphic.color, 1f); });

        f_hideCheck = 2f;
        if (b_showItem)
            return;
        SetHealthValue(m_AttachEntity.m_Health.F_BaseHealthScale);
        rtf_RectTransform.position = CameraController.MainCamera.WorldToScreenPoint(m_AttachEntity.transform.position);
        rtf_RectTransform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(m_AttachEntity.transform.position, CameraController.MainCamera.transform.position) / 30, 1, 3);
        transform.SetActivate(true);
        b_showItem = true;
    }
    private void Update()
    {
        if (!b_showItem)
            return;

        SetHealthValue(Mathf.Lerp(m_currnetHealthValue,  m_AttachEntity.m_Health.F_BaseHealthScale,Time.deltaTime*5));
        m_Name.text = m_AttachEntity.I_PoolIndex.ToString()+"|"+m_AttachEntity.m_Health.F_TotalEHP.ToString();

        rtf_RectTransform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(m_AttachEntity.transform.position, CameraController.MainCamera.transform.position) / 20, 1, 3);
        rtf_RectTransform.position = Vector3.Lerp(rtf_RectTransform.position, CameraController.MainCamera.WorldToScreenPoint(m_AttachEntity.transform.position), Time.deltaTime * 20);

        f_hideCheck -= Time.deltaTime;
        if (f_hideCheck < 1f)
            m_Graphics.Traversal((Graphic graphic)=> { graphic.color = TCommon.ColorAlpha(graphic.color,f_hideCheck); });

        if (f_hideCheck < 0)
            OnHide();
    }

    void SetHealthValue(float value)
    {
        m_HealthBar1.value = value<1?value:1;
        m_HealthBar2.value = value<2?value-1:1;
        m_HealthBar3.value = value<3?value-2:1;
        m_currnetHealthValue = value;
    }
}
