using GameSetting;
using System;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_Damage : UIT_GridItem {
    Text m_Amount,m_Projection;
    float f_expireCheck;
    Action<int> OnAnimFinished;
    RectTransform rtf_Container,rtf_SubContainer;
    EntityCharacterBase m_Entity;
    public override void Init()
    {
        base.Init();
        rtf_Container = tf_Container.GetComponent<RectTransform>();
        rtf_SubContainer = tf_Container.Find("SubContainer").GetComponent<RectTransform>();
        m_Amount = rtf_SubContainer.Find("Amount").GetComponent<Text>();
        m_Projection = rtf_SubContainer.Find("Projection").GetComponent<Text>();
    }

    public void Play(EntityCharacterBase damageEntity,float amount,Action<int> _OnAnimFinished)
    {
        rtf_SubContainer.localScale = Vector3.one * UIExpression.GetUIDamageScale(amount);
        m_Entity = damageEntity;
        rtf_RectTransform.SetWorldViewPortAnchor(damageEntity.tf_Head.position, CameraController.MainCamera);
        rtf_SubContainer.anchoredPosition = new Vector2(0,80f)+ UnityEngine.Random.insideUnitCircle*UIConst.F_UIDamageStartOffset;
        string integer = Mathf.CeilToInt(amount).ToString();
        m_Amount.text = integer;
        m_Projection.text = integer;
        f_expireCheck = 1f;
        OnAnimFinished = _OnAnimFinished;
    }

    private void Update()
    {
        rtf_RectTransform.SetWorldViewPortAnchor(m_Entity.tf_Head.position, CameraController.MainCamera, .1f);

        f_expireCheck -= Time.deltaTime;
        if (f_expireCheck < 0)
            OnAnimFinished(I_Index);
        rtf_Container.anchoredPosition = Vector2.Lerp(new Vector2(0,100),Vector2.zero,f_expireCheck);
        m_Amount.color = Color.Lerp(TCommon.ColorAlpha(Color.red,0f),Color.red,f_expireCheck);
    }
}
