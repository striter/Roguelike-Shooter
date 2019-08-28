using UnityEngine;
using UnityEngine.UI;
public class UIT_Localization : Text
{
    public bool B_AutoLocalize = false;
    public string LocalizeKey;
    protected override void Awake()
    {
        base.Awake();
        if(B_AutoLocalize)
            TLocalization.OnLocaleChanged += OnKeyLocalize;
    }
    protected override void Start()
    {
        base.Start();
        if (B_AutoLocalize)
            OnKeyLocalize();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(B_AutoLocalize)
            TLocalization.OnLocaleChanged -= OnKeyLocalize;
    }

    void OnKeyLocalize()
    {
        base.text = TLocalization.GetKeyLocalized(LocalizeKey);
    }

    public override string text
    {
        get
        {
            return base.text;
        }

        set
        {
            LocalizeKey = value;
            OnKeyLocalize();
        }
    }
}