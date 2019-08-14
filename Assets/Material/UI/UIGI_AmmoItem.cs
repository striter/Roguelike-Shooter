using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_AmmoItem : UIT_GridItem {
    Image image;
    protected override void Init()
    {
        base.Init();
        image = tf_Container.Find("Image").GetComponent<Image>();
    }
    public void Set(Color color)
    {
        image.color = color;
    }
}
