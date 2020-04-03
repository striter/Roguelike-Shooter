using GameSetting;
using System;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_VisualizeDamage : UIT_GridItem {
    Text m_Amount,m_Projection;
    Action<int> OnAnimFinished;
    RectTransform rtf_SubContainer;
    EntityCharacterBase m_Entity;
    Animator m_Animator;
    static int m_AnimatorIDPlay = Animator.StringToHash("t_play");
    static int m_AnimatorIDCritical = Animator.StringToHash("b_critical");
    public override void Init()
    {
        base.Init();
        m_Animator = GetComponent<Animator>();
        rtf_SubContainer = rtf_Container.Find("SubContainer").GetComponent<RectTransform>();
        m_Amount = rtf_SubContainer.Find("Amount").GetComponent<Text>();
        m_Projection = rtf_SubContainer.Find("Projection").GetComponent<Text>();
        rtf_RectTransform.anchoredPosition = UIConst.V2_UINumericVisualizeOffset;
    }

    public void Play(EntityCharacterBase damageEntity,bool criticalHit,float applyAmount,Action<int> _OnAnimFinished)
    {
        rtf_SubContainer.anchoredPosition = new Vector2( TCommon.RandomUnitValue()* UIConst.F_UIDamageStartOffset,0);
        rtf_RectTransform.SetWorldViewPortAnchor(damageEntity.tf_Head.position, CameraController.MainCamera);
        rtf_SubContainer.localScale = Vector3.one * UIExpression.GetUIDamageScale(applyAmount);
        m_Animator.SetBool(m_AnimatorIDCritical, criticalHit);
        m_Animator.SetTrigger(m_AnimatorIDPlay);
        m_Entity = damageEntity;
        string integer = Mathf.CeilToInt(applyAmount).ToString();
        m_Amount.text = integer;
        m_Amount.color = UIExpression.GetUIDamageColor(criticalHit);
        m_Projection.text = integer;
        OnAnimFinished = _OnAnimFinished;
    }
    void OnAnimFinish()
    {
        OnAnimFinished(m_Index);
    }

    private void Update()
    {
        rtf_RectTransform.SetWorldViewPortAnchor(m_Entity.transform.position, CameraController.MainCamera, .1f);
    }
}
