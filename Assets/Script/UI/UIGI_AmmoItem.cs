using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_AmmoItem : UIT_GridItem {
    Image image;
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        image = tf_Container.Find("Image").GetComponent<Image>();
    }

    public void Set(Color color)
    {
        image.color = color;
    }
}
