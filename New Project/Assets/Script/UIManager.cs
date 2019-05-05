using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public enum enum_BC_PlayerMessage
{
    PlayerWeaponStatusChanged,
    PlayerStatusChanged,
    PlayerHealthChanegd,
    PlayerHitEnermy,
    PlayerInteractInfo,
}
public class UIManager : MonoBehaviour
{
    UI_HUD ui_hud;
    public void Awake()
    {
        ui_hud = new UI_HUD(transform.Find("HUD"));
        TBroadCaster<enum_BC_PlayerMessage>.Init();
        TBroadCaster<enum_BC_PlayerMessage>.Add<PlayerBase>(enum_BC_PlayerMessage.PlayerWeaponStatusChanged,ui_hud.OnPlayerWeaponStatusChanged);
        TBroadCaster<enum_BC_PlayerMessage>.Add<PlayerBase>(enum_BC_PlayerMessage.PlayerHealthChanegd, ui_hud.OnPlayerHealthStatusChanged);
        TBroadCaster<enum_BC_PlayerMessage>.Add<bool>(enum_BC_PlayerMessage.PlayerHitEnermy, ui_hud.OnPlayerHitEnermy);
        TBroadCaster<enum_BC_PlayerMessage>.Add<string>(enum_BC_PlayerMessage.PlayerInteractInfo, ui_hud.OnPlayerInteractInfo);
    }


    class UI_HUD:ISingleCoroutine
    {
        Transform transform;
        Transform tf_Weapon;
        SpriteAtlas atlas_hud;
        Text txt_ClipAmmo, txt_PackAmmo;
        Text txt_WeaponName, txt_AmmoType;
        Image img_CrossHair;
        Image img_CrossHairHit;

        Transform tf_PlayerStatus;
        Text txt_HealthAmount;
        Image img_HealthBar;

        UIT_GridController gc_InteractInfo;
        public UI_HUD(Transform parentTrans)
        {
            transform = parentTrans;
            atlas_hud = TResources.Load<SpriteAtlas>("Atlas/HUD");
            tf_Weapon=transform.Find("WeaponStatus");
            txt_ClipAmmo = tf_Weapon.Find("ClipAmmo").GetComponent<Text>();
            txt_PackAmmo = tf_Weapon.Find("PackAmmo").GetComponent<Text>();
            txt_WeaponName = tf_Weapon.Find("WeaponName").GetComponent<Text>();
            txt_AmmoType = tf_Weapon.Find("AmmoType").GetComponent<Text>();
            img_CrossHair = tf_Weapon.Find("CrossHair").GetComponent<Image>();
            img_CrossHairHit = tf_Weapon.Find("CrossHairHit").GetComponent<Image>();

            tf_PlayerStatus = transform.Find("PlayerStatus");
            txt_HealthAmount = tf_PlayerStatus.Find("HealthAmount").GetComponent<Text>();
            img_HealthBar = tf_PlayerStatus.Find("HealthBar").GetComponent<Image>();

            gc_InteractInfo =new UIT_GridController(transform.Find("InteractInfo"));
            for (int i = 0; i < GameSettings.CI_UI_InteractMaxShow; i++)
            {
                Transform temp= gc_InteractInfo.AddItem(i);
                temp.SetActivate(false);
            }
        }

        enum_WeaponType E_WeaponTypeBefore;
        internal void OnPlayerWeaponStatusChanged(PlayerBase player)
        {

            bool showWeaponUI = player.wb_current != null;
            tf_Weapon.SetActivate(showWeaponUI);

            if (showWeaponUI)
            {
                txt_ClipAmmo.text = player.wb_current.I_AmmoLeft.ToString();
                txt_PackAmmo.text = player.I_AmmoLeft.ToString();

                if (E_WeaponTypeBefore != player.wb_current.E_WeaponType)
                {
                    img_CrossHair.sprite = atlas_hud.GetSprite("CrossHair_" + player.wb_current.E_WeaponType.ToString());
                    img_CrossHair.SetNativeSize();
                    img_CrossHairHit.sprite = atlas_hud.GetSprite("CrossHair_Hit_" + player.wb_current.E_WeaponType.ToString());
                    img_CrossHairHit.SetNativeSize();
                    img_CrossHairHit.color = new Color(img_CrossHairHit.color.r, img_CrossHairHit.color.g, img_CrossHairHit.color.b, 0);
                    this.StartSingleCoroutine(0, TIEnumerators.UI.StartTypeWriter(txt_AmmoType, player.wb_current.E_AmmoType.ToString(), 1f));
                    this.StartSingleCoroutine(1, TIEnumerators.UI.StartTypeWriter(txt_WeaponName, player.wb_current.E_WeaponType.ToWeaponName(), 1f));
                }
            }
            E_WeaponTypeBefore = player.wb_current==null? enum_WeaponType.Invalid:player.wb_current.E_WeaponType;
        }

        internal void OnPlayerHitEnermy(bool targetDead)
        {
            img_CrossHairHit.color = targetDead ? Color.red : Color.white;
            this.StartSingleCoroutine(2,TIEnumerators.ChangeValueTo((float value)=> {
                img_CrossHairHit.color = new Color(img_CrossHairHit.color.r, img_CrossHairHit.color.g, img_CrossHairHit.color.b,value);
            },1f,0,targetDead?1f:.5f));
        }

        internal void OnPlayerHealthStatusChanged(PlayerBase pb)
        {
            txt_HealthAmount.text = ((int)pb.f_curHealth).ToString();
            float percentage= pb.f_curHealth / pb.I_MaxHealth;
            img_HealthBar.fillAmount = percentage;

            if (percentage < .5f)
            {
                Color c= Color.Lerp(Color.red, Color.white, percentage * 2);
                img_HealthBar.color = c;
                txt_HealthAmount.color = c;
            }
        }

        int i_identity = 0;
        internal void OnPlayerInteractInfo(string pickupInfo)
        {
            Text temp = gc_InteractInfo.GetItem(i_identity).GetComponent<Text>();
            temp.transform.SetAsLastSibling();
            temp.SetActivate(true);
            this.StartSingleCoroutine(10+i_identity, TIEnumerators.UI.StartTypeWriter(temp,pickupInfo,1f));
            this.StartSingleCoroutine(20 + i_identity, TIEnumerators.PauseDel(2f,i_identity, (int identity) => {  gc_InteractInfo.GetItem(identity).SetActivate(false);}));

            i_identity++;
            if (i_identity >= GameSettings.CI_UI_InteractMaxShow)
                i_identity = 0;
        }
    }
}
