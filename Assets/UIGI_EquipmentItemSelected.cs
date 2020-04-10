using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIGI_EquipmentItemSelected : UIGI_EquipmentItemBase {

    UIT_GridControllerMono<Text> m_EntryGrid;

    public override void Init()
    {
        base.Init();
        m_EntryGrid =new UIT_GridControllerMono<Text>( rtf_Container.Find("EntryGrid"));
    }

    public new void Play(EquipmentSaveData data)
    {
        base.Play(data);
        m_EntryGrid.ClearGrid();
        data.m_Entries.Traversal((int index,EquipmentEntrySaveData entryData) =>
        {
            m_EntryGrid.AddItem(index).text=entryData.m_Type+":"+entryData.m_Value;
        });
        m_EntryGrid.AddItem(m_EntryGrid.I_Count).text = "Passive" + data.GetPassiveLocalizeKey();
    }
}
