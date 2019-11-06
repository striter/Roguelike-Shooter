using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBase : SimpleSingletonMono<UIManagerBase> {

    protected Canvas cvs_Overlay, cvs_Camera;
    Transform tf_Page, tf_Control,tf_MessageBox;
    public static Vector2 m_FitScale { get; private set; }
    protected virtual void Init()
    {
        cvs_Overlay = transform.Find("Overlay").GetComponent<Canvas>();
        cvs_Camera = transform.Find("Camera").GetComponent<Canvas>();
        tf_Control = cvs_Camera.transform.Find("Control");
        tf_Page = cvs_Overlay.transform.Find("Page");
        tf_MessageBox = cvs_Overlay.transform.Find("MessageBox");
        CanvasScaler m_Scaler = cvs_Overlay.GetComponent<CanvasScaler>();
        m_FitScale = Vector2.one * (Screen.width / (float)Screen.height) / (m_Scaler.referenceResolution.x / m_Scaler.referenceResolution.y);
    }

    public T ShowPage<T>(bool useAnim) where T : UIPageBase => UIPageBase.Show<T>(tf_Page,useAnim);
    public T ShowControls<T>() where T : UIControlBase => UIControlBase.Show<T>(tf_Control);
    public T ShowMessageBox<T>() where T : UIMessageBoxBase => UIMessageBoxBase.Show<T>(tf_MessageBox);
}

