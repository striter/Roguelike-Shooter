using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    Button m_Edit_Resize;
    InputField m_Edit_Resize_X, m_Edit_Resize_Y;
    TypeSelection m_File_Type;
    RawImage m_View_Image;
    Text m_View_Name,m_View_Type;
    Transform m_Data;
    TypeSelection m_Data_Type;
    UIT_GridControllerGridItem<LevelEditorUIDataView> m_EditorView;

    enum_LevelType m_FileChunkType= enum_LevelType.Battle;
    List<string> m_DataViewing = new List<string>();
    protected override void Awake()
    {
        base.Awake();
        TReflection.UI.UIPropertyFill(this,transform);
        m_File_New.onClick.AddListener(OnNewClick);
        m_File_Read.onClick.AddListener(OnReadClick);
        m_File_Save.onClick.AddListener(OnSaveClick);
        m_Edit_Resize.onClick.AddListener(OnResizeButtonClick);
        m_File_Type.Init(m_FileChunkType,OnFileTypeSelect);
        m_Data_Type.Init( enum_LevelType.Battle, OnDataViewTypeSelect);
        m_EditorView = new UIT_GridControllerGridItem<LevelEditorUIDataView>(m_Data.Find("DataView/DataGrid"));
    }

    void OnFileTypeSelect(int typeID)
    {
        m_FileChunkType = (enum_LevelType)typeID;
    }

    void OnDataViewTypeSelect(int typeID)
    {
        enum_LevelType targetChunkType = (enum_LevelType)typeID;
        m_DataViewing.Clear();
        m_EditorView.ClearGrid();
        int index = 0;
        TResources.GetChunkDatas().Traversal((enum_LevelType type,List< LevelChunkData> datas)=> {
            if (type != targetChunkType)
                return;
            datas.Traversal((LevelChunkData data) =>
            {
                m_EditorView.AddItem(index++).SetData(OnDataEditClick, data);
                m_DataViewing.Add(data.name);
            });
        } );
    }

    void OnDataEditClick(int index)
    {
        ReadData(LevelEditorManager.Instance.Read(m_DataViewing[index]));
    }

    void OnNewClick()
    {
        m_Edit_Resize_X.text = m_File_New_X.text;
        m_Edit_Resize_Y.text = m_File_New_Y.text;
        LevelEditorManager.Instance.New(int.Parse( m_File_New_X.text),int.Parse(m_File_New_Y.text), m_FileChunkType);
    }

    void OnReadClick() => ReadData(LevelEditorManager.Instance.Read(m_File_Read_Name.text));

    void ReadData(LevelChunkData data)
    {
        if (!data)
            return;

        m_Edit_Resize_X.text = data.Width.ToString();
        m_Edit_Resize_Y.text = data.Height.ToString();
        m_File_Read_Name.text = data.name;
        m_File_Save_Name.text = data.name;
        m_View_Name.text = data.name;
        m_View_Type.text = data.Type.ToString();
        m_View_Image.texture = data.CalculateEditorChunkTexture();
        m_View_Image.SetNativeSize();
    }

    void OnSaveClick()
    {
        LevelChunkData data= LevelEditorManager.Instance.Save(m_File_Save_Name.text);
        if (!data)
            return;
        m_View_Image.texture = data.CalculateEditorChunkTexture();
        m_View_Image.SetNativeSize();
    }

    void OnResizeButtonClick()
    {
        LevelChunkEditor.Instance.Resize(int.Parse(m_Edit_Resize_X.text),int.Parse(m_Edit_Resize_Y.text));
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
