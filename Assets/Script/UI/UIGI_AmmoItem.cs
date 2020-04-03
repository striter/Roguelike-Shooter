using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_AmmoItem : UIT_GridItem {
    Image image,background;
    public override void Init()
    {
        base.Init();
        image = rtf_Container.Find("Image").GetComponent<Image>();
        background = rtf_Container.Find("Background").GetComponent<Image>();
    }

    public void SetValid(bool valid)
    {
        image.SetActivate(valid);
        background.SetActivate(!valid);
    }
}
