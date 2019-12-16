using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIT_TextExtend)),CanEditMultipleObjects]
public class EUITextExtend : UnityEditor.UI.TextEditor
{
    UIT_TextExtend m_target = null;
    public override void OnInspectorGUI()
    {
        m_target = target as UIT_TextExtend;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Auto Localize:", GUILayout.Width(Screen.width / 3 - 20));
        m_target.B_AutoLocalize = EditorGUILayout.Toggle(m_target.B_AutoLocalize, GUILayout.Width(Screen.width * 2 / 3 - 20));
        EditorGUILayout.EndHorizontal();

        if (m_target.B_AutoLocalize)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Localize Key:", GUILayout.Width(Screen.width / 3 - 20));
            m_target.S_AutoLocalizeKey= GUILayout.TextArea(m_target.S_AutoLocalizeKey, GUILayout.Width(Screen.width * 2 / 3 - 20));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            TLocalization.SetRegion(enum_Option_LanguageRegion.CN);
            GUILayout.Label("Localized Text:", GUILayout.Width(Screen.width / 3 - 20));
            GUILayout.Label( TLocalization.CanLocalize(m_target.S_AutoLocalizeKey) ? TLocalization.GetKeyLocalized(m_target.S_AutoLocalizeKey) : "Unable To Localize", GUILayout.Width(Screen.width * 2 / 3 - 20));
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        int spacing = EditorGUILayout.IntField("Character Spacing:", m_target.m_characterSpacing);
        if (spacing != m_target.m_characterSpacing)
        {
            m_target.m_characterSpacing = spacing;
            m_target.SetAllDirty();
        }
        EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
