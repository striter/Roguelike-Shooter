using UnityEngine;
using UnityEngine.UI;
public class UIT_Localization : Text
{
    public string LocalizeKey;
    protected override void Awake()
    {
        base.Awake();
        TLocalization.OnLocaleChanged += OnLocaleChanged;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TLocalization.OnLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged()
    {
        base.text = LocalizeKey.Localize();
    }
}