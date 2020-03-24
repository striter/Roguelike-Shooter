using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUIDataView : UIT_GridItem {
    Text m_Name;
    RawImage m_Image;
    Action<int> OnDataEditClick,OnDataDeleteClick;
    public override void Init()
    {
        base.Init();
        m_Name = tf_Container.Find("Name").GetComponent<Text>();
        m_Image = tf_Container.Find("Image").GetComponent<RawImage>();
        tf_Container.Find("Edit").GetComponent<Button>().onClick.AddListener(()=>OnDataEditClick(m_Index));
        tf_Container.Find("Delete").GetComponent<Button>().onClick.AddListener(() => OnDataDeleteClick(m_Index));
    }


    public void SetData(Action<int> OnDataEditClick, Action<int> OnDataDeleteClick, LevelChunkData data)
    {
        this.OnDataEditClick = OnDataEditClick;
        this. OnDataDeleteClick = OnDataDeleteClick;
        m_Name.text = data.name;
        m_Image.texture = data.CalculateEditorChunkTexture();
        m_Image.SetNativeSize();
    }

    void OnEditClick() => OnDataEditClick(m_Index);
}
