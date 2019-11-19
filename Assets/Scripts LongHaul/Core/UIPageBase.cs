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
    protected float f_bgAlphaStart;
    public float m_BaseScale = 1f;
    bool b_useAnim;
    const float F_AnimDuration = .2f;
    public static T Show<T>(Transform parentTransform,bool useAnim) where T:UIPageBase
    {
        T page = TResources.Instantiate<T>("UI/Pages/" + typeof(T).ToString(), parentTransform);
        page.Init(useAnim);
        m_Pages.Add(page);
        return page;
    }
    protected Button btn_ContainerCancel,btn_WholeCancel;
    protected Transform tf_Container;
    protected void Init(bool useAnim)
    {
        Init();
        b_useAnim = useAnim;
        if (!useAnim)
        {
            tf_Container.localScale = Vector2.one * m_BaseScale;
            return;
        }

        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
            tf_Container.localScale = Vector2.one* m_BaseScale * value;
            img_Background.color = new Color(img_Background.color.r, img_Background.color.g, img_Background.color.b, value * f_bgAlphaStart);
        }
        , 0f, 1f, F_AnimDuration, null, false));
    }

    protected override void Init()
    {
        base.Init();
        tf_Container = transform.Find("Container");
        img_Background = transform.Find("Background").GetComponent<Image>();
        f_bgAlphaStart = img_Background.color.a;
        btn_WholeCancel = img_Background.GetComponent<Button>();
        Transform containerCancel = tf_Container.Find("BtnCancel");
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
        if (!b_useAnim)
        {
            OnHideFinished();
            return;
        }
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
            tf_Container.localScale = m_BaseScale* Vector2.one *value;
            img_Background.color = new Color(img_Background.color.r, img_Background.color.g, img_Background.color.b, value * f_bgAlphaStart);
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
        OnPageExit();
        this.StopAllSingleCoroutines();
    }
}
