using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TExcel;
public class ActionTester : MonoBehaviour {
    public TextAsset m_ActionTestAsset;
    List<SActionTest> m_ActionsTest = new List<SActionTest>();
    private void Awake()
    {
        m_ActionsTest = Tools.GetFieldData<SActionTest>(Tools.ReadExcelData(m_ActionTestAsset));
    }
    struct SActionTest:ISExcel
    {
        int index;
        string intro;

        public void InitOnValueSet()
        {
        }
    }
}
