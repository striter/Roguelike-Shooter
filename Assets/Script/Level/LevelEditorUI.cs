using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649
public class LevelEditorUI : SimpleSingletonMono<LevelEditorUI>,TReflection.UI.IUIPropertyFill {
    class ChunkTypeSelection:TReflection.UI.CPropertyFillElement
    {
        Text m_Text;
        ObjectPoolSimpleComponent<int, Button> m_ChunkButton;
        public override void OnElementFIll(Transform transform)
        {
            base.OnElementFIll(transform);
            m_Text = transform.Find("Text").GetComponent<Text>();
            m_ChunkButton = new ObjectPoolSimpleComponent<int, Button>(transform.Find("Grid"), "GridItem");
            transform.GetComponent<Button>().onClick.AddListener(() => {
                m_ChunkButton.transform.SetActivate(!m_ChunkButton .transform.gameObject.activeSelf);
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

    Transform TReflection.UI.IUIPropertyFill.GetFillParent() => transform;
    Transform m_File, m_Edit,m_Test;
    Button m_File_New, m_File_Read, m_File_Save;
    InputField m_File_New_X, m_File_New_Y,  m_File_Read_Name, m_File_Save_Name;
    Button m_Edit_Resize;
    InputField m_Edit_Resize_X, m_Edit_Resize_Y;
    ChunkTypeSelection m_File_Type;
    Button m_Test_Generate;
    Text m_Test_Generate_Text;
    InputField m_Test_Generate_Seed;
    RawImage m_View_Image;

    enum_ChunkType m_ChunkType= enum_ChunkType.Battle;
    protected override void Awake()
    {
        base.Awake();
        TReflection.UI.UIPropertyFill(this);
        m_File_New.onClick.AddListener(OnNewClick);
        m_File_Read.onClick.AddListener(OnReadClick);
        m_File_Save.onClick.AddListener(OnSaveClick);
        m_Edit_Resize.onClick.AddListener(OnResizeButtonClick);
        m_File_Type.Init(m_ChunkType,OnTypeSelect);
        m_Test_Generate.onClick.AddListener(OnTestGenerateClick);
    }

    void OnTypeSelect(int typeID)
    {
        m_ChunkType = (enum_ChunkType)typeID;
    }


    void OnNewClick()
    {
        m_Edit_Resize_X.text = m_File_New_X.text;
        m_Edit_Resize_Y.text = m_File_New_Y.text;
        LevelEditorManager.Instance.New(int.Parse( m_File_New_X.text),int.Parse(m_File_New_Y.text), m_ChunkType);
    }

    void OnReadClick()
    {
        LevelChunkData data= LevelEditorManager.Instance.Read(m_File_Read_Name.text);
        if (!data)
            return;

        m_Edit_Resize_X.text = data.Width.ToString();
        m_Edit_Resize_Y.text = data.Height.ToString();
        m_File_Save_Name.text = data.name;
        m_View_Image.texture = data.CalculateMapTexture();
        m_View_Image.SetNativeSize();
    }

    void OnSaveClick()
    {
        LevelChunkData data= LevelEditorManager.Instance.Save(m_File_Save_Name.text);
        if (!data)
            return;
        m_View_Image.texture = data.CalculateMapTexture();
        m_View_Image.SetNativeSize();
    }

    void OnResizeButtonClick()
    {
        LevelChunkEditor.Instance.Resize(int.Parse(m_Edit_Resize_X.text),int.Parse(m_Edit_Resize_Y.text));
    }

    void OnTestGenerateClick()
    {
        GameLevelManager.Instance.Generate(m_Test_Generate_Seed.text);
        m_Test_Generate_Text.text = GameLevelManager.Instance.m_Seed;
    } 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            m_File.SetActivate(!m_File.gameObject.activeSelf);
        if (Input.GetKeyDown(KeyCode.F2))
            m_Edit.SetActivate(!m_Edit.gameObject.activeSelf);
        if (Input.GetKeyDown(KeyCode.F3))
            m_Test.SetActivate(!m_Test.gameObject.activeSelf);
    }
}
