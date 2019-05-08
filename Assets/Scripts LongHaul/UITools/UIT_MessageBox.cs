using System;
using UnityEngine;
using UnityEngine.UI;
public class UIT_MessageBox: SimpleSingletonMono<UIT_MessageBox>
{
    public static void ShowMessageBox(string titleKey,string tipsKey,string leftBtnKey,string rightBtnKey,
        Action<bool> _OnBtnClick,bool useLoading=false)
    {
        Instance.StartMessage(titleKey, tipsKey,leftBtnKey,rightBtnKey,_OnBtnClick,useLoading);
    }
    public static void ShowMessageBox<T>(Action OnFinished=null) where T : UIT_MessageBoxItem
    {
        Instance.StartMessage<T>(OnFinished);
    }

    protected Transform tf_Container, tf_Normal, tf_Special;

    Button  btn_Cancel,btn_Left,btn_Right;
    Text txt_Tips,txt_Title,txt_Left,txt_Right;
    Action<bool> OnBtnClick;
    bool b_UseLoading;
    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    public virtual void Init()
    {
        tf_Container = transform.Find("Container");
        txt_Title = tf_Container.Find("TxtTitle").GetComponent<Text>();
        btn_Cancel = tf_Container.transform.Find("BtnCancel").GetComponent<Button>();
        btn_Cancel.onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
            SetShow(false);
        }));

        tf_Normal = tf_Container.Find("Normal");
        txt_Tips = tf_Normal.Find("TxtTips").GetComponent<Text>();
        btn_Left = tf_Normal.Find("BtnLeft").GetComponent<Button>();
        btn_Right = tf_Normal.Find("BtnRight").GetComponent<Button>();
        txt_Left = btn_Left.transform.GetComponentInChildren<Text>();
        txt_Right = btn_Right.transform.GetComponentInChildren<Text>();
        btn_Left.onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
            OnLowerBtnClick(true);
        }));
        btn_Right.onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
            OnLowerBtnClick(false);
        }));

        tf_Special = tf_Container.Find("Special");

        TCommon.SetTransformShow(this.transform, false);
    }
    protected void StartMessage(string titleKey, string tipsKey, string leftBtnKey, string rightBtnKey,
        Action<bool> _OnBtnClick, bool useLoading)
    {
        tf_Normal.SetActivate(true);
        tf_Special.SetActivate(false);
        b_UseLoading = useLoading;
        txt_Tips.text = tipsKey.Localize();
        SetTitle(titleKey);
        txt_Left.text = leftBtnKey.Localize();
        txt_Right.text = rightBtnKey.Localize();
        OnBtnClick = _OnBtnClick;
        SetShow(true);
    }
    void OnLowerBtnClick(bool isLeft)
    {
        if (b_UseLoading)
        {
            UIT_Loading.StartLoading(()=> {
                if(OnBtnClick!=null)
                OnBtnClick(isLeft);
            });
        }
        else
        {
            if (OnBtnClick != null)
                OnBtnClick(isLeft);
        }
        SetShow(false );
    }

    UIT_MessageBoxItem mbi_Special;
    Action OnSuccessFul;
    protected T StartMessage<T>(Action _OnSuccessful) where T:UIT_MessageBoxItem
    {
        tf_Normal.SetActivate(false);
        tf_Special.SetActivate(true);

        OnSuccessFul = _OnSuccessful;
        
        SetShow(true);
        
        mbi_Special = Instantiate(Resources.Load<GameObject>("Prefabs/UI/MessageBox/" + typeof(T).ToString()), tf_Special).GetComponent<T>();
        mbi_Special.Init(this);
        SetTitle(mbi_Special.s_TitleKey);
        return mbi_Special.GetComponent<T>();
    }
    public void SpecialMessageFinished()
    {
        SetShow(false);
        Destroy(mbi_Special.gameObject);
        if (OnSuccessFul != null)
            OnSuccessFul();
    }
     void SetShow(bool show )
    {
        TCommon.SetTransformShow(this.transform, show);
        if(!show&&mbi_Special!=null)
                Destroy(mbi_Special.gameObject);
    }
    public void SetTitle(string titleKey)
    {
        if (titleKey != "")
            txt_Title.text = titleKey.Localize();
    }

    public class UIT_MessageBoxItem : MonoBehaviour
    {
        internal virtual string s_TitleKey
        {
            get
            {
                return "Test Must Change";
            }
        }
        protected UIT_MessageBox parent;
        public RectTransform rtf_Transform;
        internal virtual void Init(UIT_MessageBox _parent)
        {
            parent = _parent;
            rtf_Transform = transform.GetComponent<RectTransform>();
        }
        protected virtual void OnFinished()
        {
            parent.SpecialMessageFinished();
        }
    }
}