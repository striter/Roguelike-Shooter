using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIT_Localization))]
public class ELocalizationText : UnityEditor.UI.TextEditor
{
    UIT_Localization m_target = null;
    public override void OnInspectorGUI()
    {
        m_target = target as UIT_Localization;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Localize Key:", GUILayout.Width(Screen.width / 3-20));
        m_target.LocalizeKey = GUILayout.TextArea(m_target.LocalizeKey, GUILayout.Width(Screen.width*2/3-20) );
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        TLocalization.SetRegion(enum_LanguageRegion.CN);
        GUILayout.Label("Localized Text:", GUILayout.Width(Screen.width / 3 - 20));
        GUILayout.Label(m_target.LocalizeKey.CanLocalize()?m_target.LocalizeKey.Localize():"Unable To Localize", GUILayout.Width(Screen.width * 2 / 3 - 20));
        GUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}
