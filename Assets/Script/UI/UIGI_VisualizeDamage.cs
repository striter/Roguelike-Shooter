using GameSetting;
using System;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_VisualizeDamage : UIT_GridItem {
    Text m_Amount,m_Projection;
    Action<int> OnAnimFinished;
    RectTransform rtf_SubContainer;
    EntityCharacterBase m_Entity;
    TSpecialClasses.AnimationControlBase m_Animation;
    public override void Init()
    {
        base.Init();
        m_Animation = new TSpecialClasses.AnimationControlBase(transform.GetComponent<Animation>(), true);
        rtf_SubContainer = tf_Container.Find("SubContainer").GetComponent<RectTransform>();
        m_Amount = rtf_SubContainer.Find("Amount").GetComponent<Text>();
        m_Projection = rtf_SubContainer.Find("Projection").GetComponent<Text>();
    }

    public void Play(EntityCharacterBase damageEntity,float amount,Action<int> _OnAnimFinished)
    {
        m_Animation.Play(true);
        rtf_SubContainer.localScale = Vector3.one * UIExpression.GetUIDamageScale(amount);
        m_Entity = damageEntity;
        rtf_RectTransform.SetWorldViewPortAnchor(damageEntity.tf_Head.position, CameraController.MainCamera);
        rtf_SubContainer.anchoredPosition = UnityEngine.Random.insideUnitCircle*UIConst.F_UIDamageStartOffset;
        string integer = Mathf.CeilToInt(amount).ToString();
        m_Amount.text = integer;
        m_Projection.text = integer;
        OnAnimFinished = _OnAnimFinished;
    }
    void OnAnimFinish()
    {
        OnAnimFinished(I_Index);
    }

    private void Update()
    {
        rtf_RectTransform.SetWorldViewPortAnchor(m_Entity.tf_Head.position, CameraController.MainCamera, .1f);
    }
}
