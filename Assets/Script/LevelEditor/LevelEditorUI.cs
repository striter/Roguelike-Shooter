using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649
public class LevelEditorUI : SimpleSingletonMono<LevelEditorUI>,TReflection.UI.IUIPropertyFill {

    Transform TReflection.UI.IUIPropertyFill.GetFillParent() => transform;
    Button m_File_New, m_File_Read, m_File_Save;
    InputField m_File_New_X, m_File_New_Y,  m_File_Read_Name, m_File_Save_Name;
    Button m_Edit_Resize, m_Edit_Desize;
    InputField m_Edit_Resize_X, m_Edit_Resize_Y;
    protected override void Awake()
    {
        base.Awake();
        TReflection.UI.UIPropertyFill(this);
        m_File_New.onClick.AddListener(OnNewClick);
        m_File_Read.onClick.AddListener(OnReadClick);
        m_File_Save.onClick.AddListener(OnSaveClick);
        m_Edit_Resize.onClick.AddListener(OnResizeButtonClick);
        m_Edit_Desize.onClick.AddListener(OnDesizeButtonClick);
    }

    void OnNewClick()
    {
        LevelEditorManager.Instance.New(int.Parse( m_File_New_X.text),int.Parse(m_File_New_Y.text));
    }

    void OnReadClick()
    {
        LevelEditorManager.Instance.Read(m_File_Read_Name.text);
    }

    void OnSaveClick()
    {
        LevelEditorManager.Instance.Save(m_File_Save_Name.text);
    }

    void OnDesizeButtonClick()
    {
        LevelChunkEditor.Instance.Desize();
    }

    void OnResizeButtonClick()
    {
        LevelChunkEditor.Instance.Resize(int.Parse(m_Edit_Resize_X.text),int.Parse(m_Edit_Resize_Y.text));
    }

}
