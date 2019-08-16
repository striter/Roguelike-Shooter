using System;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_Damage : UIT_GridItem {
    Text m_Amount;
    float f_expireCheck;
    Action<int> OnExpire;
    EntityBase m_attachEntity;
    RectTransform rtf_Container;
    protected override void Init()
    {
        base.Init();
        rtf_Container = tf_Container.GetComponent<RectTransform>();
        m_Amount = tf_Container.Find("Amount").GetComponent<Text>();
    }
    public void Play(EntityBase damageEntity,float amount,Action<int> _OnExpire)
    {
        m_attachEntity = damageEntity;
        m_Amount.text = amount.ToString();
        f_expireCheck = 1f;
        OnExpire = _OnExpire;
    }
    private void Update()
    {
        f_expireCheck -= Time.deltaTime;
        rtf_RectTransform.anchoredPosition = CameraController.MainCamera.WorldToScreenPoint(m_attachEntity.tf_Head.position);
        rtf_Container.anchoredPosition = Vector2.Lerp(new Vector2(0,200),Vector2.zero,f_expireCheck);
        m_Amount.color = Color.Lerp(TCommon.ColorAlpha(Color.red,0f),Color.red,f_expireCheck);
        if (f_expireCheck < 0)
            OnExpire(I_Index);
    }
}
