using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIManager : SingletonMono<UIManager>
{
    public static Action OnReload;
    public static Action<bool> OnMainDown;
    Transform tf_Top,tf_Pages,tf_LowerTools;
    Image img_fire,img_pickup,img_chat;
    Image m_main;
    protected override void Awake()
    {
        instance = this;
        tf_Top = transform.Find("Top");
        tf_LowerTools = transform.Find("Lower");
        tf_Pages = transform.Find("Pages");

        tf_Top.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        tf_Top.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnMainDown?.Invoke(down); };
        img_fire = tf_Top.Find("Main/Fire").GetComponent<Image>();
        img_pickup = tf_Top.Find("Main/Pickup").GetComponent<Image>();
        img_chat= tf_Top.Find("Main/Chat").GetComponent<Image>();
        
        transform.Find("Test/SporeBtn").GetComponent<Button>().onClick.AddListener(() => { ShowPage<UI_SporeManager>(true); });
    }
    private void Start()
    {
        ShowTools<UI_EntityHealth>();
        ShowTools<UI_PlayerStatus>();

        transform.Find("Test/SeedTest").GetComponent<Text>().text = GameManager.Instance.m_GameLevel.m_Seed;
    }

    public T ShowPage<T>(bool animate) where T : UIPageBase => UIPageBase.ShowPage<T>(tf_Pages, animate);
    protected T ShowTools<T>() where T : UIToolsBase => UIToolsBase.Show<T>(tf_LowerTools);

    void OnPlayerStatusChanged(EntityCharacterPlayer player)
    {
        Image mainImage=img_fire;
        if (player.m_Interact != null)
        {
            switch (player.m_Interact.m_InteractType)
            {
                case enum_Interaction.Invalid: Debug.LogError("No Convertions Here");break;
                case enum_Interaction.ActionAdjustment:mainImage = img_chat;break;
                default:mainImage = img_pickup;break;
            }
        }
        if (m_main == mainImage)
            return;
        if(m_main)
            m_main.SetActivate(false);
        m_main = mainImage;
        m_main.SetActivate(true);
    }
}
