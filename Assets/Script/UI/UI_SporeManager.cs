using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TExcel;
using System;
public class UI_SporeManager : UIPageBase,ISingleCoroutine {
    Text txt_Coin, txt_CoinsPerSecond, txt_Blue;
    Text txt_MaxLevel;
    Button btn_BuyCoin, btn_BuyBlue;
    Text txt_BuyCoin, txt_BuyBlue;
    UIT_GridControllerMono<UIGI_SporeContainer> gc_SporeContainers;     //0 Spare //-1 Locked
    MessageBox_OffsetProfit m_profitMessageBox;
    CSporeManagerSave m_ManagerInfo;
    SSporeLevelRate m_CurrentSporeRate;
    int i_TimeScale = 1,i_draggingSlot=-1;
    float f_totalCoinsPerMinute;
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
        
        btn_BuyCoin = tf_Container.Find("BuyCoin").GetComponent<Button>();
        btn_BuyCoin.onClick.AddListener(OnCoinAcquireChest);
        btn_BuyBlue = tf_Container.Find("BuyBlue").GetComponent<Button>();
        btn_BuyBlue.onClick.AddListener(OnBlueAcquireChest);
        txt_BuyCoin = btn_BuyCoin.GetComponentInChildren<Text>();
        txt_BuyBlue = btn_BuyBlue.GetComponentInChildren<Text>();

        tf_Container.Find("TrashBox").GetComponent<UIT_EventTriggerListener>().D_OnRaycast = OnTrashBox;

        m_profitMessageBox = new MessageBox_OffsetProfit(tf_Container.Find("OfflineProfit"));

        gc_SporeContainers = new UIT_GridControllerMono<UIGI_SporeContainer>(tf_Container.Find("SporeContainers"));
        for (int i = 1; i <= UIConst.I_SporeManagerContainersMaxAmount; i++)
            gc_SporeContainers.AddItem(i).Init(OnDragSporeStatus,OnDragSpore,OnDropSpore, OnSlotTickProfit);

        f_autoSaveCheck = Time.time + UIConst.I_SporeManagerAutoSave;

        RefreshMaxLevel(false);
        RefreshManagerInfo();
        RefreshContainerInfo();

        CalculateOfflineProfit();
    }
    void CalculateOfflineProfit()
    {
        int offsetSeconds = TTime.TTime.GetTimeStamp(DateTime.Now) - m_ManagerInfo.i_previousTimeStamp;
        m_ManagerInfo.d_timePassed += offsetSeconds;
        if (offsetSeconds > UIConst.I_SporeManagerUnitTime)
        {
            float profit = OnProfit(f_totalCoinsPerMinute * (offsetSeconds / UIConst.I_SporeManagerUnitTime));
            m_profitMessageBox.Show("Offline Profit:" + profit);
        }
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

        if (Time.time > f_autoSaveCheck)      //Auto Save Case Game Crush Or Force Quit
            OnSavePlayerData();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnSavePlayerData();
    }

    void RefreshManagerInfo()
    {
        txt_Coin.text = "Coins:" + m_ManagerInfo.f_coin;
        txt_Blue.text = "Blues:" + DataManager.m_PlayerInfo.f_blue;
    }

    void RefreshContainerInfo()
    {
        for (int i = 1; i <= gc_SporeContainers.I_Count; i++)
        {
            UIGI_SporeContainer item = gc_SporeContainers.GetItem(i);
            item.SetContainerInfo(m_ManagerInfo[i]);
        }

        f_totalCoinsPerMinute = 0;
        for (int i = 1; i <= m_ManagerInfo.I_SlotCount; i++)
        {
            if (m_ManagerInfo[i] > 0)
                f_totalCoinsPerMinute += UIExpression.F_SporeManagerProfitPerMinute(m_ManagerInfo[i]);
        }
        txt_CoinsPerSecond.text = "CPM:" + f_totalCoinsPerMinute;
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

    void OnSlotTickProfit(int slotIndex)
    {
        int level = m_ManagerInfo[slotIndex];
        if (level == -1 || level == 0)
            return;

        OnProfit(UIExpression.F_SporeManagerProfitPerMinute(level) * ((float)UIConst.I_SporeManagerContainerTickTime / UIConst.I_SporeManagerUnitTime));
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

        if (m_ManagerInfo[dropSlot] == m_ManagerInfo[i_draggingSlot]&&m_ManagerInfo[dropSlot]!=UIConst.I_SporeManagerHybridMaxLevel)     //Hybrid
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
        if (DataManager.m_PlayerInfo.f_blue < m_CurrentSporeRate.F_BlueChestPrice)
            return;
        int spareSlot = m_ManagerInfo.GetSpareSlot();
        if (spareSlot == -1)
            return;
        DataManager.m_PlayerInfo.OnBlueUsed( m_CurrentSporeRate.F_BlueChestPrice);
        m_ManagerInfo[spareSlot] = m_CurrentSporeRate.AcquireNewSpore();

        RefreshContainerInfo();
    }

    float OnProfit(float coinsAmount)
    {
        float profit = 0f;
        if (m_ManagerInfo.f_coin >= m_CurrentSporeRate.F_MaxCoinsAmount)
            return profit;

        if (m_ManagerInfo.f_coin + coinsAmount > m_CurrentSporeRate.F_MaxCoinsAmount)
            profit = m_CurrentSporeRate.F_MaxCoinsAmount - m_ManagerInfo.f_coin;
        else
            profit = coinsAmount;

        m_ManagerInfo.f_coin += profit;
        return profit;
    }
    
    void OnSavePlayerData()
    {
        m_ManagerInfo.i_previousTimeStamp = TTime.TTime.GetTimeStamp(DateTime.Now);
        TGameData<CSporeManagerSave>.Save(m_ManagerInfo);
    }

    class MessageBox_OffsetProfit : UIT_SimpleBehaviours.UIT_MessageBox
    {
        Button btn_Confirm;
        public MessageBox_OffsetProfit(Transform _transform) : base(_transform)
        {
            btn_Confirm = tf_Container.Find("BtnConfirm").GetComponent<Button>();
            btn_Confirm.onClick.AddListener(()=> { SetShow(false); });
            SetShow(false);
        }
        public void Show(string text)
        {
            txt_Title.text = text;
            SetShow(true);
        }
    }
}
