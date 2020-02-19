
using UnityEngine;
using UnityEngine.UI;

public class UITest : UIT_EventTriggerListener {
    RawImage image;
    private void Awake()
    {
        image = GetComponent<RawImage>();
        base.OnLocalDown = this.OnDownLocal;
    }

    void OnDownLocal(bool down,Vector2 localPos)
    {
        Debug.Log(localPos+" "+ (image.mainTexture as Texture2D).GetPixel((int)localPos.x, (int)localPos.y));
    }
}
