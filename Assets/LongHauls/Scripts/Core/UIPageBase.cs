using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPageBase : UIComponentBase,ISingleCoroutine
{
    public static List<UIPageBase> m_Pages = new List<UIPageBase>();
    public static int I_PageCount => m_Pages.Count;
    public static bool m_PageOpening => I_PageCount>0;
    public static bool Opening<T>() where T : UIPageBase => m_Pages.Count > 0 && m_Pages.Find(p => p.GetType() ==typeof(T));
    public static Action OnPageExit;
    protected Image img_Background;
    protected Action<bool> OnInteractFinished;
    bool m_Animating;
    const float F_AnimDuration = .15f;
    public static T Show<T>(Transform parentTransform,bool useAnim) where T:UIPageBase
    {
        if (Opening<T>())
            return null;
        T page = TResources.Instantiate<T>("UI/Pages/" + typeof(T).ToString(), parentTransform);
       
        page.Init();
        page.Play(useAnim);
        m_Pages.Add(page);
        return page;
    }
    protected Button btn_ContainerCancel,btn_WholeCancel;
    protected RectTransform rtf_Container;
    Vector2 m_AnimateStartPos, m_AnimateEndPos;
    protected void Play(bool useAnim)
    {
        m_Animating = useAnim;
        if (!useAnim)
            return;

        m_AnimateStartPos = rtf_Container.anchoredPosition + Vector2.up * Screen.height;
        m_AnimateEndPos = rtf_Container.anchoredPosition;
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
            rtf_Container.anchoredPosition = Vector2.Lerp(m_AnimateStartPos, m_AnimateEndPos, value); 
        }
        , 0f, 1f, F_AnimDuration, null, false));
    }

    protected override void Init()
    {
        base.Init();
        rtf_Container = transform.Find("Container") as RectTransform;
        rtf_Container.localScale = Vector2.one * UIManagerBase.Instance.m_fittedScale;
        img_Background = transform.Find("Background").GetComponent<Image>();
        btn_WholeCancel = img_Background.GetComponent<Button>();
        Transform containerCancel = rtf_Container.Find("BtnCancel");
        if (containerCancel) btn_ContainerCancel = containerCancel.GetComponent<Button>();

        if (btn_WholeCancel)
        {
            btn_WholeCancel.onClick.AddListener(OnCancelBtnClick);
            btn_WholeCancel.enabled = true;
        }
        if (btn_ContainerCancel)
        {
            btn_ContainerCancel.onClick.AddListener(OnCancelBtnClick);
            btn_ContainerCancel.enabled = true;
        }

    }

    protected virtual void OnCancelBtnClick()
    {
        if (btn_ContainerCancel) btn_ContainerCancel.enabled = false;
        if (btn_WholeCancel) btn_WholeCancel.enabled = false;
        if (!m_Animating)
        {
            OnHideFinished();
            return;
        }
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
            rtf_Container.anchoredPosition = Vector2.Lerp(m_AnimateStartPos, m_AnimateEndPos, value);
        }, 1f, 0f, F_AnimDuration, OnHideFinished, false));
    }

    void OnHideFinished()
    {
        OnInteractFinished?.Invoke(false);
        Destroy(this.gameObject);
    }

    protected virtual void OnDestroy()
    {
        m_Pages.Remove(this);
        this.StopAllSingleCoroutines();
        OnPageExit?.Invoke();
    }
}
