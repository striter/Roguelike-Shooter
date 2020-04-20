using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TTiles;
#pragma warning disable 0649
public class LevelEditorUI : SingletonMono<LevelEditorUI>,TReflection.UI.IUIPropertyFill {

    Transform m_File, m_Edit;
    Button m_File_New, m_File_Read, m_File_Save;
    InputField m_File_New_X, m_File_New_Y,  m_File_Read_Name, m_File_Save_Name;
    Button m_Edit_AddSize;
    Text m_Edit_AddSize_Text;
    InputField m_Edit_AddSize_Count;
    EnumSelection m_Edit_AddSize_Direction;
    Transform m_Data;
    UIT_GridControllerGridItem<LevelEditorUIDataView> m_EditorView;

    int m_DelectConfirmIndex = -1;
    enum_TileDirection m_Direction = enum_TileDirection.Top;
    List<string> m_DataViewing = new List<string>();
    protected override void Awake()
    {
        base.Awake();
        TReflection.UI.UIPropertyFill(this,transform);
        m_File_New.onClick.AddListener(OnNewClick);
        m_File_Read.onClick.AddListener(OnReadClick);
        m_File_Save.onClick.AddListener(OnSaveClick);
        m_Edit_AddSize.onClick.AddListener(OnResizeButtonClick);
        m_Edit_AddSize_Direction.Init(enum_TileDirection.Top,(int index)=> { m_Direction = (enum_TileDirection)index; });
        m_EditorView = new UIT_GridControllerGridItem<LevelEditorUIDataView>(m_Data.Find("DataView/DataGrid"));
    }


    void Start()
    {
        RebuildViewGrid();
    }

    void OnDataEditClick(int index)
    {
        ReadData(LevelEditorManager.Instance.Read(m_DataViewing[index]));
        RebuildViewGrid();
    }

    void OnDataDeleteClick(int deleteIndex)
    {
        if (m_DelectConfirmIndex == deleteIndex)
        {
            LevelEditorManager.Instance.Delete(m_DataViewing[deleteIndex]);
            RebuildViewGrid();
            return;
        }
        m_DelectConfirmIndex = deleteIndex;
    }

    void OnNewClick()
    {
        m_Edit_AddSize_Text.text = m_File_New_X.text+"|"+ m_File_New_Y.text;
        LevelEditorManager.Instance.New(int.Parse( m_File_New_X.text),int.Parse(m_File_New_Y.text));
    }

    void OnReadClick() => ReadData(LevelEditorManager.Instance.Read(m_File_Read_Name.text));

    void ReadData(LevelChunkData data)
    {
        if (!data)
            return;

        m_Edit_AddSize_Text.text = data.Width + "|" + data.Height;
        m_File_Read_Name.text = data.name;
        m_File_Save_Name.text = data.name;
    }

    void OnSaveClick()=>LevelEditorManager.Instance.Save(m_File_Save_Name.text);

    void OnResizeButtonClick()
    {
        LevelChunkEditor.Instance.Resize(int.Parse(m_Edit_AddSize_Count.text),m_Direction);
    }

    void RebuildViewGrid()
    {

        m_DelectConfirmIndex = -1;
        m_DataViewing.Clear();
        m_EditorView.ClearGrid();
        int index = 0;
        TResources.GetChunkDatas().Traversal((LevelChunkData data) => {
            m_EditorView.AddItem(index++).SetData(OnDataEditClick, OnDataDeleteClick, data);
            m_DataViewing.Add(data.name);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            m_File.SetActivate(!m_File.gameObject.activeSelf);
        if (Input.GetKeyDown(KeyCode.F2))
            m_Edit.SetActivate(!m_Edit.gameObject.activeSelf);
        if (Input.GetKeyDown(KeyCode.F4))
            m_Data.SetActivate(!m_Data.gameObject.activeSelf);
    }
}
