using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TTiles;
#pragma warning disable 0649
public class LevelEditorUI : SingletonMono<LevelEditorUI>,TReflection.UI.IUIPropertyFill {
    class TypeSelection:TReflection.UI.CPropertyFillElement
    {
        Text m_Text;
        ObjectPoolListComponent<int, Button> m_ChunkButton;
        public TypeSelection(Transform transform):base(transform)
        {
            TReflection.UI.UIPropertyFill(this, transform);
            m_ChunkButton = new ObjectPoolListComponent<int, Button>(transform.Find("Grid"), "GridItem");
            transform.GetComponent<Button>().onClick.AddListener(() => {
                m_ChunkButton.transform.SetActivate(!m_ChunkButton.transform.gameObject.activeSelf);
            });
            m_ChunkButton.transform.SetActivate(false);
        }

        public void Init<T>(T defaultValue, Action<int> OnClick)
        {
            m_Text.text = defaultValue.ToString();
            m_ChunkButton.ClearPool();
            TCommon.TraversalEnum((T temp) =>
            {
                int index =(int)((object)temp);
                Button btn= m_ChunkButton.AddItem(index);
                btn.onClick.RemoveAllListeners();
                btn.GetComponentInChildren<Text>().text = temp.ToString();
                btn.onClick.AddListener(() => {
                    m_Text.text = temp.ToString();
                    OnClick(index);
                    m_ChunkButton.transform.SetActivate(false);
                });
            });
        }
    }

    Transform m_File, m_Edit;
    Button m_File_New, m_File_Read, m_File_Save;
    InputField m_File_New_X, m_File_New_Y,  m_File_Read_Name, m_File_Save_Name;
    Button m_Edit_AddSize;
    Text m_Edit_AddSize_Text;
    InputField m_Edit_AddSize_Count;
    TypeSelection m_Edit_AddSize_Direction;
    TypeSelection m_File_Type;
    Transform m_Data;
    TypeSelection m_Data_Type;
    UIT_GridControllerGridItem<LevelEditorUIDataView> m_EditorView;

    enum_ChunkType m_FileChunkType= enum_ChunkType.Battle;
    enum_ChunkType m_ViewChunkType = enum_ChunkType.Invalid;
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
        m_File_Type.Init(m_FileChunkType,OnFileTypeSelect);
        m_Data_Type.Init( enum_ChunkType.Battle, OnDataViewTypeSelect);
        m_EditorView = new UIT_GridControllerGridItem<LevelEditorUIDataView>(m_Data.Find("DataView/DataGrid"));
    }

    void OnFileTypeSelect(int typeID)
    {
        m_FileChunkType = (enum_ChunkType)typeID;
    }

    void OnDataViewTypeSelect(int typeID)
    {
        m_ViewChunkType = (enum_ChunkType)typeID;
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
        LevelEditorManager.Instance.New(int.Parse( m_File_New_X.text),int.Parse(m_File_New_Y.text), m_FileChunkType);
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
        if (m_ViewChunkType == enum_ChunkType.Invalid)
            return;
        int index = 0;
        TResources.GetChunkDatas().Traversal((enum_ChunkType type, List<LevelChunkData> datas) => {
            if (type != m_ViewChunkType)
                return;
            datas.Traversal((LevelChunkData data) =>
            {
                m_EditorView.AddItem(index++).SetData(OnDataEditClick, OnDataDeleteClick, data);
                m_DataViewing.Add(data.name);
            });
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
