
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIT_GridDefaultItem : UIT_GridItem {

    protected Text txt_Default;
    protected Button btn_Default;
    protected Image img_Default;
    protected Image img_HighLight;
    public bool B_HighLight { get; protected set; }
    Action<int> OnItemClick;
    protected override void Init()
    {
        base.Init();
        if (txt_Default)
            return;

        txt_Default = tf_Container.Find("DefaultText").GetComponentNullable<Text>();
        img_Default = tf_Container.Find("DefaultImage").GetComponentNullable<Image>();
        img_HighLight = tf_Container.Find("DefaultHighLight").GetComponentNullable<Image>();
        btn_Default = tf_Container.Find("DefaultBtn").GetComponentNullable<Button>();
        if (btn_Default != null)
            btn_Default.onClick.AddListener(OnItemTrigger);
    }
    public void SetDefaultOnClick(Action<int> _OnItemClick)
    {
        OnItemClick = _OnItemClick;
    }
        public void SetItemInfo(string defaultText = "", bool highLight = false, Sprite defaultSprite = null, bool setNativeSize = false)
    {
        if (defaultText != "")
            txt_Default.text = defaultText;
        if (defaultSprite != null)
        {
            img_Default.sprite = defaultSprite;
            if (setNativeSize)
                img_Default.SetNativeSize();
        }
        SetHighLight(highLight);
    }
    public void SetHighLight(bool highLight)
    {
        B_HighLight = highLight;
        if (img_HighLight == null)
        {
            return;
        }
        img_HighLight.SetActivate(highLight);
    }
    protected void OnItemTrigger()
    {
        OnItemClick?.Invoke(I_Index);
    }
}
