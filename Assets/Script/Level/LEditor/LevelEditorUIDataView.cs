using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUIDataView : UIT_GridItem {
    Button m_Edit;
    Text m_Name;
    RawImage m_Image;
    Action<int> OnDataEditClick;
    public override void Init()
    {
        base.Init();
        m_Edit= tf_Container.Find("Button").GetComponent<Button>();
        m_Name = tf_Container.Find("Name").GetComponent<Text>();
        m_Image = tf_Container.Find("Image").GetComponent<RawImage>();
        m_Edit.onClick.AddListener(OnClick);
    }


    public void SetData(Action<int> OnDataClick, LevelChunkData data)
    {
        OnDataEditClick = OnDataClick;
        m_Name.text = data.name;
        m_Image.texture = data.CalculateEditorChunkTexture();
        m_Image.SetNativeSize();
    }

    void OnClick() => OnDataEditClick(m_Index);
}
