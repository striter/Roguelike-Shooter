using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPageBase : MonoBehaviour,ISingleCoroutine
{
    public static bool m_PageOpening => t_curPage != null;
    public static Type t_curPage;
    public static Action OnPageExit;
    protected Image img_Background;
    protected Action<bool> OnInteractFinished;
    protected float f_bgAlphaStart;
    bool b_useAnim;
    const float F_AnimDuration = .2f;
    public static T Show<T>(Transform parentTransform,bool useAnim) where T:UIPageBase
    {
        if (t_curPage == typeof(T))
            return null;

        t_curPage = typeof(T);
        T tempBase = TResources.Instantiate<T>("UI/Pages/" + typeof(T).ToString(), parentTransform);
        tempBase.Init(useAnim);
        return tempBase;
    }
    protected Button btn_ContainerCancel,btn_WholeCancel;
    protected Transform tf_Container;
    protected virtual void Init(bool useAnim)
    {
        b_useAnim = useAnim;
        tf_Container = transform.Find("Container");
        img_Background = transform.Find("Background").GetComponent<Image>();
        f_bgAlphaStart = img_Background.color.a;
        btn_WholeCancel = img_Background.GetComponent<Button>();
        Transform containerCancel = tf_Container.Find("BtnCancel");
        if(containerCancel) btn_ContainerCancel = containerCancel.GetComponent<Button>();

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

        if (!useAnim)
        {
            tf_Container.localScale = UIManagerBase.m_FitScale;
            return;
        }

        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
            tf_Container.localScale = UIManagerBase.m_FitScale * value;
            img_Background.color = new Color(img_Background.color.r, img_Background.color.g, img_Background.color.b, value * f_bgAlphaStart);
        }
        , 0f, 1f, F_AnimDuration, null, false));
    }
    protected virtual void OnCancelBtnClick()
    {
        OnPageExit?.Invoke();
        if (btn_ContainerCancel) btn_ContainerCancel.enabled = false;
        if (btn_WholeCancel) btn_WholeCancel.enabled = false;
        if (!b_useAnim)
        {
            OnHideFinished();
            return;
        }
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
            tf_Container.localScale = UIManagerBase.m_FitScale * value;
            img_Background.color = new Color(img_Background.color.r, img_Background.color.g, img_Background.color.b, value * f_bgAlphaStart);
        }, 1f, 0f, F_AnimDuration, OnHideFinished, false));
    }

    void OnHideFinished()
    {
        t_curPage = null;
        OnInteractFinished?.Invoke(false);
        Destroy(this.gameObject);
    }

    protected virtual void OnDestroy()
    {
        t_curPage = null;
        this.StopAllSingleCoroutines();
    }
}
