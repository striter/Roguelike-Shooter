using System;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_Damage : UIT_GridItem {
    Text m_Amount;
    float f_expireCheck;
    Action<int> OnAnimFinished;
    EntityCharacterBase m_attachEntity;
    RectTransform rtf_Container;
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        rtf_Container = tf_Container.GetComponent<RectTransform>();
        m_Amount = tf_Container.Find("Amount").GetComponent<Text>();
    }

    public void Play(EntityCharacterBase damageEntity,float amount,Action<int> _OnAnimFinished)
    {
        m_attachEntity = damageEntity;
        m_Amount.text = Mathf.CeilToInt(amount).ToString()+"/"+amount;
        f_expireCheck = 1f;
        OnAnimFinished = _OnAnimFinished;
    }

    private void Update()
    {
        rtf_RectTransform.SetWorldViewPortAnchor(m_attachEntity.tf_Head.position, CameraController.MainCamera, Time.deltaTime * 10f);

        f_expireCheck -= Time.deltaTime;
        rtf_Container.anchoredPosition = Vector2.Lerp(new Vector2(0,200),Vector2.zero,f_expireCheck);
        m_Amount.color = Color.Lerp(TCommon.ColorAlpha(Color.red,0f),Color.red,f_expireCheck);
        if (f_expireCheck < 0)
            OnAnimFinished(I_Index);
    }
}
