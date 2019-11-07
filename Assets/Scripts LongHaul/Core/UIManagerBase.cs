using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBase : SimpleSingletonMono<UIManagerBase> {

    protected Canvas cvs_Overlay, cvs_Camera;
    protected RectTransform tf_Page, tf_Control,tf_MessageBox;
    public static Vector2 m_FitScale { get; private set; }
    protected virtual void Init()
    {
        cvs_Overlay = transform.Find("Overlay").GetComponent<Canvas>();
        cvs_Camera = transform.Find("Camera").GetComponent<Canvas>();
        tf_Control = cvs_Camera.transform.Find("Control").GetComponent<RectTransform>();
        tf_Page = cvs_Overlay.transform.Find("Page").GetComponent<RectTransform>();
        tf_MessageBox = cvs_Overlay.transform.Find("MessageBox").GetComponent<RectTransform>();
        CanvasScaler m_Scaler = cvs_Overlay.GetComponent<CanvasScaler>();
        m_FitScale = Vector2.one * (Screen.width / (float)Screen.height) / (m_Scaler.referenceResolution.x / m_Scaler.referenceResolution.y);
    }

    protected T ShowPage<T>(bool useAnim) where T : UIPageBase => UIPageBase.Show<T>(tf_Page,useAnim);
    protected T ShowControls<T>() where T : UIControlBase => UIControlBase.Show<T>(tf_Control);
    protected T ShowMessageBox<T>() where T : UIMessageBoxBase => UIMessageBoxBase.Show<T>(tf_MessageBox);
}

