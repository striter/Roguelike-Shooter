using UnityEngine;
using UnityEngine.UI;
public class UIT_TextLocalization : Text
{
    public bool B_AutoLocalize = false;
    public string S_AutoLocalizeKey;
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
        text = TLocalization.GetKeyLocalized(S_AutoLocalizeKey);
    }

    public string formatText(string formatKey, params object[] subItems) => base.text = string.Format(TLocalization.GetKeyLocalized(formatKey), subItems);
    public string localizeText
    {
        set
        {
            text = TLocalization.GetKeyLocalized(S_AutoLocalizeKey);
        }
    }
}