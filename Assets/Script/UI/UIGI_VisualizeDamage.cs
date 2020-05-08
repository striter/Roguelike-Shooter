using GameSetting;
using System;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_VisualizeDamage : UIT_GridItem {
    Text m_Amount,m_Projection,m_Critical;
    Action<int> OnAnimFinished;
    RectTransform rtf_SubContainer;
    EntityCharacterBase m_Entity;
    Animator m_Animator;
    static int m_AnimatorIDPlay = Animator.StringToHash("t_play");
    static int m_AnimatorIDCritical = Animator.StringToHash("b_critical");
    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Animator = GetComponent<Animator>();
        rtf_SubContainer = rtf_Container.Find("SubContainer").GetComponent<RectTransform>();
        m_Amount = rtf_SubContainer.Find("Amount").GetComponent<Text>();
        m_Projection = rtf_SubContainer.Find("Projection").GetComponent<Text>();
        m_Critical = m_Amount.transform.Find("Critical").GetComponent<Text>();
        rtf_RectTransform.anchoredPosition = UIConst.V2_UINumericVisualizeOffset;
    }

    public void Play(EntityCharacterBase _damageEntity,bool _criticalHit,float _applyAmount,Action<int> _OnAnimFinished)
    {
        rtf_SubContainer.anchoredPosition = UIExpression.GetUIDamagePositionOffset();
        rtf_RectTransform.SetWorldViewPortAnchor(_damageEntity.tf_Head.position, CameraController.MainCamera);
        rtf_SubContainer.localScale = Vector3.one * UIExpression.GetUIDamageScale(_applyAmount);
        m_Animator.SetBool(m_AnimatorIDCritical, _criticalHit);
        m_Critical.SetActivate(_criticalHit);
        m_Animator.SetTrigger(m_AnimatorIDPlay);
        m_Entity = _damageEntity;
        string integer = Mathf.CeilToInt(_applyAmount).ToString();
        m_Amount.text = integer;
        m_Amount.color = UIExpression.GetUIDamageColor(_criticalHit);
        m_Projection.text = integer;
        OnAnimFinished = _OnAnimFinished;
    }
    void OnAnimFinish()
    {
        OnAnimFinished(m_Identity);
    }

    private void Update()
    {
        rtf_RectTransform.SetWorldViewPortAnchor(m_Entity.transform.position, CameraController.MainCamera, .1f);
    }
}
