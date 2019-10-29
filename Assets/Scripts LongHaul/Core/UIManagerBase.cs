using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBase : SimpleSingletonMono<UIManagerBase> {

    public static Vector2 m_PageScale { get; private set; }
    protected void Init(Canvas scaleCanvas)
    {
        CanvasScaler m_Scaler = scaleCanvas.GetComponent<CanvasScaler>();
        m_PageScale = Vector2.one * (Screen.width / (float)Screen.height) / (m_Scaler.referenceResolution.x / m_Scaler.referenceResolution.y);
    }
}
