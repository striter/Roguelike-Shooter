using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_AmmoItem : UIT_GridItem {
    Image image,background;
    public override void Init()
    {
        base.Init();
        image = tf_Container.Find("Image").GetComponent<Image>();
        background = tf_Container.Find("Background").GetComponent<Image>();
    }

    public void Set(bool valid)
    {
        image.SetActivate(valid);
        background.SetActivate(!valid);
    }
}
