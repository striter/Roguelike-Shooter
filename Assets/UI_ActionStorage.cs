using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionStorage : UIPageBase {
    UIT_GridControllerMono<UIGI_ActionSelectItem> m_Grid;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Grid = new UIT_GridControllerMono<UIGI_ActionSelectItem>(tf_Container.Find("ActionGrid"));
    }
    public void Show(List<ActionBase> showList)
    {
        m_Grid.ClearGrid();
        for (int i = 0; i < showList.Count; i++)
            m_Grid.AddItem(i).SetItemInfo(showList[i].m_Index+"/"+showList[i].m_Level);
    }
}
