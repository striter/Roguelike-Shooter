using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TExcel;

public class UI_SporeManager : UIPageBase,ISingleCoroutine {
    Text txt_Coin, txt_CoinsPerSecond, txt_Blue;
    Text txt_MaxLevel, txt_TimePassed, txt_TimeScale;
    Button btn_BuyCoin, btn_BuyBlue;
    Text txt_BuyCoin, txt_BuyBlue;
    UIT_GridControllerMono<UIGI_SporeContainer> gc_SporeContainers;     //0 Spare //-1 Locked
    CSporeManagerSave m_ManagerInfo;
    SSporeLevelRate m_CurrentSporeRate;
    int i_TimeScale = 5,i_draggingSlot=-1;
    float f_autoSaveCheck;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
       
        if (!Properties<SSporeLevel>.B_Inited) Properties<SSporeLevel>.Init();
        m_ManagerInfo=TGameData<CSporeManagerSave>.Read();

        txt_Coin =tf_Container.Find("Coin").GetComponentInChildren<Text>();
        txt_CoinsPerSecond = tf_Container.Find("CoinsPerSecond").GetComponentInChildren<Text>();
        txt_Blue = tf_Container.Find("Blue").GetComponentInChildren<Text>();
        txt_MaxLevel = tf_Container.Find("MaxLevel").GetComponentInChildren<Text>();
        txt_TimePassed = tf_Container.Find("TimePassed").GetComponentInChildren<Text>();
        txt_TimeScale = tf_Container.Find("TimeScale").GetComponentInChildren<Text>();

        InputField if_TimeScaleChanged = tf_Container.Find("TimeScaleChanger").GetComponent<InputField>();
        if_TimeScaleChanged.text = "Change Time Scale";
        if_TimeScaleChanged.onValueChanged.AddListener((string s)=> { if (s != "") i_TimeScale = int.Parse(s);RefreshManagerInfo(); });
        InputField if_CoinChanger = tf_Container.Find("CoinChanger").GetComponent<InputField>();
        if_CoinChanger.text = "Change Coin";
        if_CoinChanger.onValueChanged.AddListener((string s) => {if(s!="") m_ManagerInfo.f_coin = int.Parse(s); RefreshManagerInfo();});
        InputField if_BlueChanger = tf_Container.Find("BlueChanger").GetComponent<InputField>();
        if_BlueChanger.text = "Change Blue?";
        if_BlueChanger.onValueChanged.AddListener((string s) => { if (s != "") GameManager.m_PlayerInfo.f_blue = int.Parse(s); RefreshManagerInfo(); });

        btn_BuyCoin = tf_Container.Find("BuyCoin").GetComponent<Button>();
        btn_BuyCoin.onClick.AddListener(OnCoinAcquireChest);
        btn_BuyBlue = tf_Container.Find("BuyBlue").GetComponent<Button>();
        btn_BuyBlue.onClick.AddListener(OnBlueAcquireChest);
        txt_BuyCoin = btn_BuyCoin.GetComponentInChildren<Text>();
        txt_BuyBlue = btn_BuyBlue.GetComponentInChildren<Text>();

        tf_Container.Find("TrashBox").GetComponent<UIT_EventTriggerListener>().D_OnRaycast = OnTrashBox;

        gc_SporeContainers = new UIT_GridControllerMono<UIGI_SporeContainer>(tf_Container.Find("SporeContainers"));
        for (int i = 1; i <= UIConst.I_SporeManagerContainersMaxAmount; i++)
            gc_SporeContainers.AddItem(i).Init(OnDragSporeStatus,OnDragSpore,OnDropSpore, TickProfit);

        f_autoSaveCheck = Time.time + UIConst.I_SporeManagerAutoSave;

        RefreshMaxLevel(false);
        RefreshManagerInfo();
        RefreshContainerInfo();
    }
    void OnChanged(string field)
    {
        i_TimeScale = int.Parse(field);
    }

    void Update()
    {
        RefreshManagerInfo();
        float deltaTime = Time.deltaTime * i_TimeScale;
        m_ManagerInfo.d_timePassed += deltaTime;
        for (int i = 1; i <= gc_SporeContainers.I_Count; i++)       //Tick Every Container
        {
            gc_SporeContainers.GetItem(i).Tick(deltaTime);
        }

        if (Time.time> f_autoSaveCheck)      //Auto Save Case Game Crush Or Force Quit
            TGameData<CSporeManagerSave>.Save(m_ManagerInfo);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TGameData<CSporeManagerSave>.Save(m_ManagerInfo);
    }

    void RefreshManagerInfo()
    {
        txt_Coin.text = "Coins:" + m_ManagerInfo.f_coin;
        txt_Blue.text = "Blues:" + GameManager.m_PlayerInfo.f_blue;

        txt_TimePassed.text =  "Time Passed:\n" + string.Format("{0:0.00}",m_ManagerInfo.d_timePassed);
        txt_TimeScale.text = "Time Scale:" + i_TimeScale;
    }

    void RefreshContainerInfo()
    {
        for (int i = 1; i <= gc_SporeContainers.I_Count; i++)
        {
            UIGI_SporeContainer item = gc_SporeContainers.GetItem(i);
            item.SetContainerInfo(m_ManagerInfo[i]);
        }

        float totalCoinsPerSecond = 0;
        for (int i = 1; i <= m_ManagerInfo.I_SlotCount; i++)
        {
            if (m_ManagerInfo[i] > 0)
                totalCoinsPerSecond += UIExpression.F_SporeManagerPorfitPerSecond(m_ManagerInfo[i]);
        }
        txt_CoinsPerSecond.text = "CPS:" + totalCoinsPerSecond;
    }

    void RefreshMaxLevel(bool reachHigherLevel)
    {
        m_CurrentSporeRate = new SSporeLevelRate(Properties<SSporeLevel>.PropertiesList.Find(p => p.MaxLevel == m_ManagerInfo.i_maxLevel));
        if (reachHigherLevel && m_CurrentSporeRate.B_AddSlot) 
            m_ManagerInfo.UnlockNewSlot();

        if (m_CurrentSporeRate.I_Level == 0)
            Debug.LogError("Invalid MaxLevel:" + m_ManagerInfo.i_maxLevel);

        txt_MaxLevel.text = "Max Level:" + m_ManagerInfo.i_maxLevel;

        txt_BuyCoin.text = "Acquire Chest:\n" + m_CurrentSporeRate.F_CoinChestPrice + "Coins";
        txt_BuyBlue.text = "Acquire Chest:\n" + m_CurrentSporeRate.F_BlueChestPrice + "Blues";
    }

    void OnDragSporeStatus(int slotIndex,bool begin,Vector2 position)
    {
        if (m_ManagerInfo[slotIndex] == -1)       //Wont Interact With Locked Slot
            return;

        if (begin)
        {
            if (i_draggingSlot!=-1)     //Already Dragged A Slot Reset Previous Slot
                gc_SporeContainers.GetItem(i_draggingSlot).SetDragStatus(false);

            i_draggingSlot = slotIndex;
            gc_SporeContainers.GetItem(i_draggingSlot).SetDragStatus(true);
        }
        else if(i_draggingSlot==slotIndex)      //Interact With The Same Slot
        {
            gc_SporeContainers.GetItem(i_draggingSlot).SetDragStatus(false);
            TCommonUI.RaycastAll(position);     
            i_draggingSlot = -1;
        }
    }

    void OnDragSpore(int slotIndex,Vector2 position)
    {
        if (i_draggingSlot != -1 && i_draggingSlot == slotIndex)
            gc_SporeContainers.GetItem(i_draggingSlot).SetPosition(position);
    }
    void OnDropSpore(int dropSlot)
    {
        if (dropSlot == i_draggingSlot || i_draggingSlot == -1 || dropSlot == -1)
            return;

        if (m_ManagerInfo[dropSlot] == m_ManagerInfo[i_draggingSlot])     //Hybrid
        {
            m_ManagerInfo.AddSlotValue(dropSlot);
            if (m_ManagerInfo[dropSlot] > m_ManagerInfo.i_maxLevel)
            {
                m_ManagerInfo.i_maxLevel = m_ManagerInfo[dropSlot];
                RefreshMaxLevel(true);
            }
            m_ManagerInfo[i_draggingSlot]= 0;
        }
        else if (m_ManagerInfo[dropSlot] != -1)     //Change Pos
        {
            int targetValue = m_ManagerInfo[dropSlot];
            m_ManagerInfo[dropSlot] =m_ManagerInfo[i_draggingSlot];
            m_ManagerInfo[i_draggingSlot] = targetValue;
        }
        RefreshContainerInfo();
    }

    void OnTrashBox()
    {
        if (i_draggingSlot == -1)
            return;

        m_ManagerInfo[i_draggingSlot] = 0;

        m_ManagerInfo.f_coin += m_CurrentSporeRate.F_CoinChestPrice / 2;

        RefreshContainerInfo();
    }

    void OnCoinAcquireChest()
    {
        if (m_ManagerInfo.f_coin < m_CurrentSporeRate.F_CoinChestPrice)
            return;

        int spareSlot =m_ManagerInfo.GetSpareSlot();
        if (spareSlot == -1)
            return;
        m_ManagerInfo.f_coin -= m_CurrentSporeRate.F_CoinChestPrice;
        m_ManagerInfo[spareSlot] = m_CurrentSporeRate.AcquireNewSpore();

        RefreshContainerInfo();
    }

    void OnBlueAcquireChest()
    {
        if (GameManager.m_PlayerInfo.f_blue < m_CurrentSporeRate.F_BlueChestPrice)
            return;
        int spareSlot = m_ManagerInfo.GetSpareSlot();
        if (spareSlot == -1)
            return;
        GameManager.m_PlayerInfo.f_blue -= m_CurrentSporeRate.F_BlueChestPrice;
        m_ManagerInfo[spareSlot] = m_CurrentSporeRate.AcquireNewSpore();

        RefreshContainerInfo();
    }

    void TickProfit(int slotIndex)
    {
        int level = m_ManagerInfo[slotIndex];
        if (level == -1 || level == 0)
            return;

        m_ManagerInfo.f_coin += UIExpression.F_SporeManagerPorfitPerSecond(level) *UIConst.I_SporeManagerTickOffsetEach;
    }
    
}
