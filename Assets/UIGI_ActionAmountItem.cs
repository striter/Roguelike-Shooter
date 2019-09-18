using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_ActionAmountItem : UIT_GridItem {
    Image img_NotFull, img_Full;
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        img_Full = tf_Container.Find("Full").GetComponent<Image>();
        img_NotFull = tf_Container.Find("NotFull").GetComponent<Image>();
    }

    public void SetValue(float value)
    {
        bool showFull = value == 1;
        img_Full.SetActivate(showFull);
        img_NotFull.SetActivate(!showFull);
        if (!showFull)
            img_NotFull.fillAmount = value;
    }
}
