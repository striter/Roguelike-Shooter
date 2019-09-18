using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TExcel;
public class ActionTester : MonoBehaviour {
    public TextAsset m_ActionTestAsset;
    List<ActionTest> m_AllActions = new List<ActionTest>();
    List<ActionTest> m_OwnedActions = new List<ActionTest>();
    Dropdown m_level;
    Button btn_reset, btn_6to2, btn_2to1,btn_confirm;
    UIT_GridDefaultSingle<UIT_GridDefaultItem> m_GridSingle;
    UIT_GridDefaultMulti<UIT_GridDefaultItem> m_GridMulti;
    Text m_Count;
    ScrollRect m_OwnedParent;
    UIT_GridControllerMonoItem<UIT_GridDefaultItem> m_GridOwned;
    
    private void Awake()
    {
        btn_reset = transform.Find("BtnReset").GetComponent<Button>();
        btn_reset.onClick.AddListener(OnResetClick);
        btn_6to2 = transform.Find("Btn6to2").GetComponent<Button>();
        btn_6to2.onClick.AddListener(On6To2Click);
        btn_2to1 = transform.Find("Btn2to1").GetComponent<Button>();
        btn_2to1.onClick.AddListener(On2to1Click);
        btn_confirm = transform.Find("BtnConfirm").GetComponent<Button>();
        btn_confirm.onClick.AddListener(OnConfirmed);
        m_level = transform.Find("Level").GetComponent<Dropdown>();
        m_Count = transform.Find("Count").GetComponent<Text>();
        List<SActionTest> datas = Tools.GetFieldData<SActionTest>(Tools.ReadExcelData(m_ActionTestAsset));
        m_AllActions = new List<ActionTest>();
        for (int i = 0; i < datas.Count; i++)
            m_AllActions.Add(new ActionTest(i, datas[i]));

        m_GridMulti = new UIT_GridDefaultMulti<UIT_GridDefaultItem>(transform.Find("ActionGridMulti"),2, OnItemClick);
        m_GridSingle = new UIT_GridDefaultSingle<UIT_GridDefaultItem>(transform.Find("ActionGridSingle"), OnItemClick, true);
        m_OwnedParent = transform.Find("OwnedParent").GetComponent<ScrollRect>();
        m_GridOwned = new UIT_GridControllerMonoItem<UIT_GridDefaultItem>(m_OwnedParent.transform.Find("Viewport/ActionOwned"));

        Debug.Log("'8':Random 1 Actions ,'9':Random 2 Actions,'0': Reset , '-':Minus Level, '=':Add Level ,'Page Up':ScrollView Up,'Page Down': ScrollView Down");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            OnResetClick();
        if (Input.GetKeyDown(KeyCode.Minus))
            m_level.value--;
        if (Input.GetKeyDown(KeyCode.Equals))
            m_level.value++;
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            On2to1Click();
            m_GridSingle.OnItemClick(1);
            OnConfirmed();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            On6To2Click();
            m_GridMulti.OnItemClick(1);
            m_GridMulti.OnItemClick(2);
            OnConfirmed();
        }
        if (Input.GetKey(KeyCode.PageUp))
            m_OwnedParent.verticalNormalizedPosition += 1f * Time.deltaTime;
        else if(Input.GetKey(KeyCode.PageDown))
            m_OwnedParent.verticalNormalizedPosition -= 1f * Time.deltaTime;

    }

    void OnResetClick()
    {
        m_OwnedActions.Clear();
        m_GridMulti.transform.SetActivate(isMultiSelect);
        m_GridSingle.transform.SetActivate(!isMultiSelect);
        btn_confirm.interactable = false;
        OnActionOwnedChanged();
    }
    void OnActionOwnedChanged()
    {
        m_Count.text = m_OwnedActions.Count.ToString() ;
        m_GridOwned.ClearGrid();
        for (int i = 0; i < m_OwnedActions.Count; i++)
            m_GridOwned.AddItem(i).SetItemInfo(GetBaseText( m_OwnedActions[i]));
        m_OwnedParent.verticalNormalizedPosition = 1;
    }
    string GetBaseText(ActionTest action) => action.level + "," + action.info.m_Name + "," + action.info.m_Intro;
    void On6To2Click()
    {
        OnSelectBegin(RandomActions(6), 2);
    }
    void On2to1Click()
    {
        OnSelectBegin(RandomActions(2),1);
    }

    List<ActionTest> RandomActions(int count)
    {
        List<ActionTest> result = new List<ActionTest>();
        for (int i = 0; i < count; i++)
        {
            ActionTest random = null;
            TCommon.TraversalRandom(m_AllActions, (ActionTest action) =>{
                if (result.Find(p => p.index == action.index) == null)
                {
                    random = action;
                    return true;
                }
                return false;
             });
            if(random!=null)
                result.Add(new ActionTest(m_level.value+1, random));
        }
        return result;
    }

    List<ActionTest> m_selectList;
    bool isMultiSelect;
    void OnSelectBegin(List<ActionTest> _selectList, int count)
    {
        m_selectList = _selectList;
        isMultiSelect = count != 1;
        m_GridMulti.transform.SetActivate(isMultiSelect);
        m_GridSingle.transform.SetActivate(!isMultiSelect);
        btn_confirm.interactable = false;

        m_GridSingle.ClearGrid();
        m_GridMulti.ClearGrid();

        for (int i = 0; i < m_selectList.Count; i++)
        {
            m_GridMulti.AddItem(i).SetItemInfo(GetBaseText( m_selectList[i]), false);
            m_GridSingle.AddItem(i).SetItemInfo(GetBaseText(m_selectList[i]), false);
        }
    }
    void OnItemClick(int id)
    {
        btn_confirm.interactable = isMultiSelect ? m_GridMulti.m_AllSelected : true;
    }
    void OnConfirmed()
    {
        if (isMultiSelect)
            m_GridMulti.m_Selecting.Traversal((int index) =>{m_OwnedActions.Add(m_selectList[index]);});
        else
            m_OwnedActions.Add(m_selectList[m_GridSingle.I_CurrentSelecting]);
        btn_confirm.interactable = false;
        m_GridSingle.ClearGrid();
        m_GridMulti.ClearGrid();
        OnActionOwnedChanged();
    }


    class ActionTest
    {
        public int index { get; private set; }
        public int level { get; private set; }
        public SActionTest info { get; private set; }
        public ActionTest(int _index, SActionTest _info)
        {
            index = _index;
            info = _info;
        }
        public ActionTest(int _level, ActionTest action)
        {
            level = _level;
            index = action.index;
            info = action.info;
        }
    }
#pragma warning disable 0649
    struct SActionTest :ISExcel
    {
        string name;
        string intro;
        public string m_Name => name;
        public string m_Intro=>intro;
        public void InitOnValueSet()
        {

        }
    }
}
