using System;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649        //Warnning Closed Cause  Use Private Field To Protected Value Set By Reflection 
public enum enum_LanguageRegion
{
    CN=1,
    EN=2,
}
public static class TLocalization 
{
    public static bool IsInit = false;
    public static event Action OnLocaleChanged;
    public static enum_LanguageRegion e_CurLocation { get; private set; }
    static Dictionary<string, string> CurLocalization = new Dictionary<string, string>();
    public static void SetRegion(enum_LanguageRegion location)
    {
        if (e_CurLocation == location)
            return;

        e_CurLocation = location;

        List<string[]> data = TExcel.Tools.GetExcelData("SLocalization");

        for (int i = 1; i < data[0].Length; i++)
        {
            if (data[0][i] != ((enum_LanguageRegion)i).ToString())
                Debug.LogError("SLocalizataion Not Init Propertly:" + i.ToString());    
        }

        CurLocalization.Clear();
        int localizeIndex = (int)e_CurLocation;
        for (int i = 0; i < data.Count; i++)
            CurLocalization.Add(data[i][0], data[i][localizeIndex]);
        OnLocaleChanged?.Invoke();
        IsInit = true;
    }
    public static bool CanLocalize(this string key) => CurLocalization.ContainsKey(key);
    public static string Localize(this string key)
    {
        if (CanLocalize(key))
            return CurLocalization[key.Replace("\\n", "\n")];

        Debug.LogWarning("Localization Key:(" + key + ") Not Found In Localization " + e_CurLocation.ToString());
        return key;
    }
}
