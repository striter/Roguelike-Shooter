using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIT_TextLocalization)),CanEditMultipleObjects]
public class ELocalizationText : UnityEditor.UI.TextEditor
{
    UIT_TextLocalization m_target = null;
    public override void OnInspectorGUI()
    {
        m_target = target as UIT_TextLocalization;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Auto Localize:", GUILayout.Width(Screen.width / 3 - 20));
        m_target.B_AutoLocalize = EditorGUILayout.Toggle(m_target.B_AutoLocalize, GUILayout.Width(Screen.width * 2 / 3 - 20));
        EditorGUILayout.EndHorizontal();

        if (m_target.B_AutoLocalize)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Localize Key:", GUILayout.Width(Screen.width / 3 - 20));
            m_target.LocalizeKey = GUILayout.TextArea(m_target.LocalizeKey, GUILayout.Width(Screen.width * 2 / 3 - 20));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            TLocalization.SetRegion(enum_LanguageRegion.CN);
            GUILayout.Label("Localized Text:", GUILayout.Width(Screen.width / 3 - 20));
            GUILayout.Label( TLocalization.CanLocalize(m_target.LocalizeKey) ? TLocalization.GetKeyLocalized(m_target.LocalizeKey) : "Unable To Localize", GUILayout.Width(Screen.width * 2 / 3 - 20));
            GUILayout.EndHorizontal();
        }
        base.OnInspectorGUI();
    }
}
