using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using TGameSave;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 商品类型
/// </summary>
public enum enum_CommodityType
{
    Invalid = -1,
    /// <summary>
    /// vip
    /// </summary>
    VIP,
    /// <summary>
    /// 新手礼包
    /// </summary>
    Novice,
    /// <summary>
    /// 宝箱
    /// </summary>
    LuckDraw,
    /// <summary>
    /// 金币
    /// </summary>
    GoldCoin,
    /// <summary>
    /// 钻石
    /// </summary>
    Diamonds,
    /// <summary>
    /// 角色
    /// </summary>
    Role,
    /// <summary>
    /// 图纸
    /// </summary>
    Drawing,
    /// <summary>
    /// 武器
    /// </summary>
    Arms,
    Max,
}
/// <summary>
/// 商品
/// </summary>
public class Commodity
{
    public enum_CommodityType m_type= enum_CommodityType.Invalid;
    public string m_name;
    /// <summary>
    /// 剩余时间
    /// </summary>
    public string m_timeRemaining;
    /// <summary>
    /// 简介
    /// </summary>
    public string m_introduction;
    /// <summary>
    /// 货币数量
    /// </summary>
    public int m_num;
    /// <summary>
    /// 价格
    /// </summary>
    public int m_price;
    /// <summary>
    /// 图片
    /// </summary>
    public Sprite m_sprite;
}
public class Prize
{
    public enum_CommodityType m_type = enum_CommodityType.Invalid;
    public int m_id;
    public string m_Name;
    /// <summary>
    /// 图片
    /// </summary>
    public Sprite m_sprite;
    /// <summary>
    /// 货币数量
    /// </summary>
    public int m_num;
    public Vector3 m_pos;
}
public class UI_ShoppingMall : UIPage
{
    /// <summary>
    /// 商店货币数量
    /// </summary>
    public int[] NumList = new[] { 1,1,1,1000, 5000, 10000, 100, 500, 1000 };
    public enum_CommodityType[] CommodityTypeList = new enum_CommodityType[] { enum_CommodityType.VIP, enum_CommodityType.Novice, enum_CommodityType.LuckDraw, enum_CommodityType.GoldCoin, enum_CommodityType.GoldCoin, enum_CommodityType.GoldCoin, enum_CommodityType.Diamonds, enum_CommodityType.Diamonds, enum_CommodityType.Diamonds };
    /// <summary>
    /// 宝箱奖品货币数量
    /// </summary>
    public int[] NumNewList = new[] { 500, 500, 1000, 1000, 2000, 50, 100,1, 1, 1};
    public enum_CommodityType[] TreasureChestList = new enum_CommodityType[] { enum_CommodityType.GoldCoin, enum_CommodityType.GoldCoin, enum_CommodityType.GoldCoin, enum_CommodityType.GoldCoin, enum_CommodityType.GoldCoin, enum_CommodityType.Diamonds, enum_CommodityType.Diamonds, enum_CommodityType.Role, enum_CommodityType.Drawing , enum_CommodityType.Arms };
    /// <summary>
    /// 最大商品数量
    /// </summary>
    static int m_maiCont = 9;
    Commodity[] m_commodityList = new Commodity[m_maiCont];


    UIT_GridControlledSingleSelect<UIGI_Commodity> m_commodityGrid;
    UIT_GridControlledSingleSelect<UIGI_TreasureChest> m_treasureChestGrid;
    UIGI_Commodity[] m_gi_commodityList = new UIGI_Commodity[m_maiCont];

    /// <summary>
    /// 最大宝箱奖励数量
    /// </summary>
    static int m_treasureMaiCont = 10;
    List<Prize> m_prizeList = new List<Prize> ();
    UIGI_TreasureChest[] m_treasureChestList = new UIGI_TreasureChest[m_treasureMaiCont];

    public static CPlayerCharactersCultivateData m_CharacterData => TGameData<CPlayerCharactersCultivateData>.Data;
    /// <summary>
    /// 礼包角色
    /// </summary>
    enum_PlayerCharacter m_giftPackRole;
    /// <summary>
    /// 礼包武器
    /// </summary>
    enum_PlayerWeaponIdentity m_weapon;
    /// <summary>
    /// 礼包武器图纸
    /// </summary>
    enum_PlayerWeaponIdentity m_weaponDrawing;

    [SerializeField] GameObject m_purchaseGoldCoinsObj;
    [SerializeField] Text m_introduce;
    [SerializeField] Image m_picture;
    [SerializeField] Text m_num;
    [SerializeField] Text m_price;
    [SerializeField] Button m_button;
    [SerializeField] Button m_buttonCloce;
    [SerializeField] Button m_buttonCloceNew;
    [SerializeField] Button m_buttonTreasureChestCloceNew;
    [SerializeField] Button m_buttonNew;

    [SerializeField] GameObject m_ItemAcquisitionObj;
    [SerializeField] Image m_pictureNew;
    [SerializeField] Text m_numNew;
    [SerializeField] public Animator m_animator;
    [SerializeField] GameObject m_treasureChestObj;
    [SerializeField] GameObject m_unableToOperateObj;

    Prize m_prize;
    Commodity m_commodity;
    public static UI_ShoppingMall Instance;
    public static int m_coordinate = 3735;
    public static bool m_awardWinning = false;
    int m_randomNumber = 0;
    /// <summary>
    /// 奖品图标
    /// </summary>
    int[] m_iconList = new int[] { 3, 3, 4, 4, 5, 6, 7, 2, 9, 10 };
    protected override void Init()
    {
        Instance = this;
        base.Init();
        m_commodityGrid = new UIT_GridControlledSingleSelect<UIGI_Commodity>(rtf_Container.Find("Commodity/Viewport/Content"), OnCharacterSelect);
        m_treasureChestGrid = new UIT_GridControlledSingleSelect<UIGI_TreasureChest>(transform.Find("TreasureChest/Viewport/TreasureChest/Grid"), OnCharacterSelect);
        m_button.onClick.AddListener(OnClick);
        m_buttonCloce.onClick.AddListener(OnClickCloce);
        m_buttonCloceNew.onClick.AddListener(OnClickCloceNew);
        m_buttonTreasureChestCloceNew.onClick.AddListener(OnClickTreasureChestCloceNew);
        m_buttonNew.onClick.AddListener(OnClickNew);
        for (int i = 0; i < m_commodityList.Length; i++)
        {    
            m_gi_commodityList[i] = m_commodityGrid.AddItem(i);
            m_commodityList[i] = new Commodity();
            m_commodityList[i].m_type = CommodityTypeList[i];
            m_commodityList[i].m_name = string.Format("UI_Commodity_Name{0}", i);
            m_commodityList[i].m_sprite = LoadSourceSprite("UI/Texter/icon_"+i);
            m_commodityList[i].m_introduction = string.Format("UI_Commodity_Introduce{0}", i);
            m_commodityList[i].m_num = NumList[i];
        }

        for (int i = 0; i < m_treasureChestList.Length; i++)
        {
            m_treasureChestList[i] = m_treasureChestGrid.AddItem(i);
            m_treasureChestList[i].SetLocation(new Vector3(415 * i, 0), i);
            Prize prize = new Prize();
            prize.m_Name = string.Format("UI_Prize_Name{0}", i);
            prize.m_sprite = LoadSourceSprite("UI/Texter/icon_"+ m_iconList[i]);
            prize.m_type = TreasureChestList[i];
            prize.m_num = NumNewList[i];
            m_prizeList.Add(prize);
        }
    }

    public override void OnPlay(bool doAnim, Action<UIPageBase> OnPageExit)
    {
        base.OnPlay(doAnim, OnPageExit);
        GameDataManager.m_GameTaskData.RandomTask();
        for (int i = 0; i < m_commodityList.Length; i++)
        {
            m_commodityList[i].m_price = int.Parse(TLocalization.GetKeyLocalized(string.Format("UI_Commodity_Price{0}", i)));
            m_gi_commodityList[i].OnPlay(m_commodityList[i]);
        }
        if (GameDataManager.m_CGameShopData.m_Vip == 1)
        {
            m_gi_commodityList[0].transform.SetAsLastSibling();
        }
        if (GameDataManager.m_CGameShopData.m_NoviceGiftBag == 1)
        {
            m_gi_commodityList[1].transform.SetAsLastSibling();
        }

    }
    void OnCharacterSelect(int charcterIndex)
    {
        Debug.Log("OnCharacterSelect");
    }
    public void Purchase(Commodity data)
    {
        m_commodity = data;
        if (data.m_type == enum_CommodityType.GoldCoin)
        {
            PurchaseGoldCoins(data);
        }
        else if (data.m_type == enum_CommodityType.Diamonds)
        {
            Debug.Log("消耗人民币购买钻石" + data.m_price.ToString());
            GameDataManager.OnDiamondsStatus(data.m_num);
            ItemAcquisition();
        }
        else if (data.m_type == enum_CommodityType.LuckDraw)
        {
            enum_PlayerCharacter playe = (enum_PlayerCharacter)GameDataManager.m_CGameShopData.m_roleId;
            if (playe == enum_PlayerCharacter.Invalid)
            {
                Prize prize = new Prize();
                prize.m_Name = "UI_Prize_NameNew";
                prize.m_sprite = LoadSourceSprite("UI/Texter/icon_5");
                prize.m_type = enum_CommodityType.GoldCoin;
                prize.m_num = 5000;
                m_prizeList[7] = prize;
            }
            m_prizeList = Shuffle(m_prizeList);
            m_treasureChestObj.SetActive(true);
            m_unableToOperateObj.SetActive(false);
            for (int i = 0; i < m_treasureChestList.Length; i++)
            {
                m_treasureChestList[i].OnPlay(m_prizeList[i]);
            }

        }
        else if (data.m_type == enum_CommodityType.VIP)
        {
            m_gi_commodityList[0].transform.SetAsLastSibling();
            GameDataManager.m_CGameShopData.m_Vip = 1;
            m_gi_commodityList[0].OnPlay(m_commodityList[0]);
            ItemAcquisition();
        }
        else if (data.m_type == enum_CommodityType.Novice)
        {
            m_giftPackRole = GameDataManager.RandomPlayerCharacter();
            if (m_giftPackRole == enum_PlayerCharacter.Invalid)
            {
                //当角色全满的时候新手礼包换成金币
                m_commodity = new Commodity();
                m_commodity.m_type = enum_CommodityType.GoldCoin;
                m_commodity.m_name = "UI_Commodity_NameNew";
                m_commodity.m_sprite = LoadSourceSprite("UI/Texter/icon_5");
                m_commodity.m_introduction = "UI_Commodity_IntroduceNew";
                m_commodity.m_num = 5000;
                m_commodity.m_price = 0;
            }
            else
            {
                m_commodity.m_name = string.Format("Character_Name_{0}", (int)m_giftPackRole);
                m_CharacterData.DoUnlock(m_giftPackRole);
                TGameData<CPlayerCharactersCultivateData>.Save();
            }
            m_gi_commodityList[1].transform.SetAsLastSibling();
            GameDataManager.m_CGameShopData.m_NoviceGiftBag = 1;
            m_gi_commodityList[1].OnPlay(m_commodityList[1]);
            ItemAcquisition();
        }
    }
    /// <summary>
    /// 二次确认
    /// </summary>
    /// <param name="data"></param>
    void PurchaseGoldCoins(Commodity data)
    {
        m_purchaseGoldCoinsObj.SetActive(true);
        m_introduce.text = TLocalization.GetKeyLocalized(data.m_introduction);
        m_picture.overrideSprite = data.m_sprite;
        m_price.text = data.m_price.ToString();
        m_num.text = data.m_num.ToString();
    }
    /// <summary>
    /// 物品获得
    /// </summary>
    void ItemAcquisition()
    {
        m_buttonNew.gameObject.SetActive(true);
        m_ItemAcquisitionObj.SetActive(true);
        if (m_prize!=null)
        {
            if (m_prize.m_type == enum_CommodityType.Arms|| m_prize.m_type == enum_CommodityType.Drawing)
            {
                m_numNew.text = m_prize.m_Name;
                m_pictureNew.sprite = m_prize.m_sprite;
            }
            else
            {
                m_numNew.text = TLocalization.GetKeyLocalized(m_prize.m_Name);
                m_pictureNew.overrideSprite = m_prize.m_sprite;
            }
        }
        else
        {
            m_pictureNew.overrideSprite = m_commodity.m_sprite;
            if (m_commodity.m_type == enum_CommodityType.GoldCoin || m_commodity.m_type == enum_CommodityType.Diamonds)
            {
                m_numNew.text = m_commodity.m_num.ToString();
            }
            else
            {
                m_numNew.text = TLocalization.GetKeyLocalized(m_commodity.m_name);
            }
        }
        m_pictureNew.SetNativeSize();
    }
    void OnClick()
    {
        if (GameDataManager.OnDiamondsStatus(-m_commodity.m_price))
        {
            GameDataManager.OnCreditStatus(m_commodity.m_num);
            GameDataManager.OnDiamondsStatus(-m_commodity.m_price);
            m_purchaseGoldCoinsObj.SetActive(false);
            ItemAcquisition();
        }
    }
    /// <summary>
    /// 抽奖开始
    /// </summary>
    void OnClickNew()
    {
        if (GameDataManager.OnDiamondsStatus(-200))
        {
            m_randomNumber = 0;
            m_coordinate = 3735;
            m_animator.SetBool("End", true);
            m_animator.SetBool("PlayLottery", false);
            Invoke("Delay", 0.5F);
            m_prizeList = Shuffle(m_prizeList);
            m_buttonNew.gameObject.SetActive(false);
            m_unableToOperateObj.SetActive(true);
        }
    }
    /// <summary>
    /// 活动宝箱奖品
    /// </summary>
    /// <param name="prize"></param>
    public void AwardWinning(Prize data)
    {
        m_unableToOperateObj.SetActive(false );
        m_prize = data;
        if (data.m_type == enum_CommodityType.GoldCoin)
        {
            GameDataManager.OnCreditStatus(data.m_num);
        }
        else if (data.m_type == enum_CommodityType.Diamonds)
        {
            GameDataManager.OnDiamondsStatus(data.m_num);
        }
        else if (data.m_type == enum_CommodityType.Role)
        {
            enum_PlayerCharacter m_roleId = (enum_PlayerCharacter)GameDataManager.m_CGameShopData.m_roleId;
            if (m_roleId == enum_PlayerCharacter.Invalid || GameDataManager.CheckCharacterUnlocked(m_roleId))
            {
                //当角色全满的时候抽到大奖成金币
                m_commodity = new Commodity();
                m_commodity.m_type = enum_CommodityType.GoldCoin;
                m_commodity.m_name = "UI_Commodity_NameNew";
                m_commodity.m_sprite = LoadSourceSprite("UI/Texter/icon_5");
                m_commodity.m_introduction = "UI_Commodity_IntroduceNew";
                m_commodity.m_num = 5000;
                m_commodity.m_price = 0;
                m_prize = null;
            }
            else
            {
                m_CharacterData.DoUnlock((enum_PlayerCharacter)GameDataManager.m_CGameShopData.m_roleId);
                TGameData<CPlayerCharactersCultivateData>.Save();
            }
        }
        else if (data.m_type == enum_CommodityType.Arms)
        {
            m_weapon = GameDataManager.RandomWeapon();
            SWeaponInfos weaponInfo = GameDataManager.GetWeaponProperties(m_weapon);
            Prize prize = new Prize();
            prize.m_Name = string.Format("<color=#{0}>{1}</color>",weaponInfo.m_Rarity.GetUIColor(), TLocalization.GetKeyLocalized(m_weapon.GetNameLocalizeKey()));
            prize.m_sprite = UIManager.Instance.m_WeaponSprites[weaponInfo.m_Weapon.GetSprite(true)];
            m_prize = prize;

            GameObjectManager.SpawnInteract<InteractPickupWeapon>(NavigationManager.NavMeshPosition(CampManager.Instance.m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity).Play(WeaponSaveData.New(m_weapon, 5));
        }
        else if (data.m_type == enum_CommodityType.Drawing)
        {
            m_weaponDrawing = GameDataManager.RandomWeaponDrawing();
            SWeaponInfos weaponInfo = GameDataManager.GetWeaponProperties(m_weaponDrawing);
            Prize prize = new Prize();
            prize.m_Name = string.Format("<color=#{0}>{1}</color>", weaponInfo.m_Rarity.GetUIColor(), TLocalization.GetKeyLocalized(m_weaponDrawing.GetNameLocalizeKey()));
            prize.m_sprite = UIManager.Instance.m_WeaponSprites[weaponInfo.m_Weapon.GetSprite(false)];
            m_prize = prize;

            GameDataManager.UnlockArmoryBlueprint(weaponInfo.m_Rarity);
            TGameData<CArmoryData>.Save();
        }
        Invoke("ItemAcquisition",0.5f);
    }
    void Delay()
    {
        m_animator.SetBool("End", false);
        m_animator.SetBool("PlayLottery", true);
    }
    void OnClickCloce()
    {
        m_purchaseGoldCoinsObj.SetActive(false);
    }
    void OnClickCloceNew()
    {
        m_ItemAcquisitionObj.SetActive(false);
    }
    void OnClickTreasureChestCloceNew()
    {
        m_coordinate = 3735;
        m_animator.SetBool("End", true);
        m_animator.SetBool("PlayLottery", false);
        m_treasureChestObj.SetActive(false);
        m_prize = null;
    }
    public Prize RandomNumber()
    {
        if (m_randomNumber < m_prizeList.Count)
        {
            m_awardWinning = false;
            m_randomNumber++;
            return m_prizeList[m_randomNumber-1];
        }
        return null;
    }
    /// <summary>
    /// 随机打乱list元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"></param>
    /// <returns></returns>
    public List<T> Shuffle<T>(List<T> original)
    {
        System.Random randomNum = new System.Random();
        int index = 0;
        T temp;
        for (int i = 0; i < original.Count; i++)
        {
            index = randomNum.Next(0, original.Count - 1);
            if (index != i)
            {
                temp = original[i];
                original[i] = original[index];
                original[index] = temp;
            }
        }
        return original;
    }
    public Sprite LoadSourceSprite(string relativePath)
    {
        //Debug.Log("relativePath=" + relativePath);
        //把资源加载到内存中
        UnityEngine.Object Preb = Resources.Load(relativePath, typeof(Sprite));
        Sprite tmpsprite = null;
        try
        {
            tmpsprite = Instantiate(Preb) as Sprite;
        }
        catch (System.Exception ex)
        {

        }

        //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
        return tmpsprite;
        //return Resources.Load(relativePath, typeof(Sprite)) as Sprite;
    }
}
