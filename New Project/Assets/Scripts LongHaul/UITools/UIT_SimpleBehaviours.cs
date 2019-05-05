using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace UIT_SimpleBehaviours
{
    class UIT_SelectAmountBase : SimpleBehaviour
    {
        Action<int> OnAmountSelect;
        int i_Amount,i_MaxAmount,i_MinAmount;
        Button btn_Add, btn_Minus;
        Text txt_Amount;
        public UIT_SelectAmountBase(Transform _transform, Action<int> _OnAmountSelect, int _Min = 0,int _Max=999) : base(_transform)
        {
            i_Amount = 1;
            i_MaxAmount = _Max;
            i_MinAmount = _Min;
            OnAmountSelect = _OnAmountSelect;
            txt_Amount = transform.Find("TxtAmount").GetComponent<Text>();
            btn_Add = transform.Find("BtnAdd").GetComponent<Button>();
            btn_Minus = transform.Find("BtnMinus").GetComponent<Button>();
            btn_Add.onClick.AddListener(OnAddBtnClick);
            btn_Minus.onClick.AddListener(OnMinusBtnClick);
        }
        void OnAddBtnClick()
        {
            if(i_Amount+1<i_MaxAmount)
            SetAmount(i_Amount+1);
        }
        void OnMinusBtnClick()
        {
            if(i_Amount-1>i_MinAmount)
            SetAmount(i_Amount-1);
        }
        void SetAmount(int _amount)
        {
            i_Amount = _amount;
            txt_Amount.text = i_Amount.ToString();
            OnAmountSelect(i_Amount);
        }
        public void ChangeAmount(int _amount)
        {
            SetAmount(_amount);
        }
    }
    class UIT_InfoBar<T> : SimpleBehaviour where T:UIT_SystemInfoItem     //Normal InfoBar 0 Anims
    {
        protected class SubTitleInfo:ISingleCoroutine
        {
            public int i_index;
            public SubTitleInfo(int _index)
            {
                i_index = _index;
            }
        }
        protected int i_SubTitleInfoIndex;
        protected  int i_maxSubTitleAmount;
        protected Transform tf_Container;
        protected UIT_GridControllerMono<T> ic_ChatItemsGrid;
        Queue<SubTitleInfo> q_subTitleQueue = new Queue<SubTitleInfo>();
        public int I_TitleCount
        {
            get
            {
                return q_subTitleQueue.Count;
            }
        }
        public UIT_InfoBar(Transform _transform, int maxSubTitleAmount) : base(_transform)
        {
            i_SubTitleInfoIndex = 0;
            i_maxSubTitleAmount = maxSubTitleAmount;
            tf_Container = transform.Find("Container");
            ic_ChatItemsGrid = new UIT_GridControllerMono<T>(tf_Container.Find("ChatItemsGrid"), null);
        }
        public virtual void AddInfoText(string SubTitleInfo)
        {
            i_SubTitleInfoIndex++;
            if (i_SubTitleInfoIndex > i_maxSubTitleAmount)
                i_SubTitleInfoIndex = 0;
            T item = ic_ChatItemsGrid.AddItem(i_SubTitleInfoIndex);
            item.SetActivate(false);
            item.transform.SetAsLastSibling();
            item.SetItem(i_SubTitleInfoIndex, SubTitleInfo);
            SubTitleInfo info = new SubTitleInfo(i_SubTitleInfoIndex);
            item.rectTransform.anchoredPosition = Vector3.zero - Vector3.up * 50 * (i_maxSubTitleAmount - 1);
            info.StartSingleCoroutine(0, TIEnumerators.PauseDel(4f, () => { RemoveSubTitle(); }));
            q_subTitleQueue.Enqueue(info);
            if (q_subTitleQueue.Count > i_maxSubTitleAmount)
                RemoveSubTitle();
            else
                AdjustPositions();
        }
        public virtual void RemoveSubTitle()
        {
            SubTitleInfo info = q_subTitleQueue.Peek();
            ic_ChatItemsGrid.RemoveItem(info.i_index);
            info.StopSingleCoroutine(0);
            q_subTitleQueue.Dequeue();
            AdjustPositions();
        }
        protected virtual void AdjustPositions()
        {
            if (q_subTitleQueue.Count == 0)
                return;
            int index = q_subTitleQueue.Peek().i_index;
            for (int i = 0; i < q_subTitleQueue.Count; i++)
            {
                T item = ic_ChatItemsGrid.GetItem(index);
                item.SetActivate(true);
                item.MoveTo(Vector3.zero - Vector3.up * 50 * i);
                index++;
                if (index > i_maxSubTitleAmount)
                    index = 0;
            }
        }
        public virtual void Destroy()
        {
            q_subTitleQueue.Clear();
        }
    }
    class UIT_SubTitle : UIT_InfoBar<UIT_SubTitleItem>,ISingleCoroutine       //Half-Life 2 Like SubTitle Bar
    {
        Image img_Background;
        public UIT_SubTitle(Transform _transform,int maxSubTitleAmount=5) : base(_transform, maxSubTitleAmount)
        {
            img_Background = tf_Container.GetComponent<Image>();
            img_Background.color = new Color(img_Background.color.r, img_Background.color.g, img_Background.color.b, 0f);
        }
        public override void Destroy()
        {
            this.StopAllSingleCoroutines();
        }
        public override void AddInfoText(string SubTitleInfo)
        {
            base.AddInfoText(SubTitleInfo);
            if (I_TitleCount == 1)
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
                {
                    img_Background.color = new Color(img_Background.color.r, img_Background.color.g, img_Background.color.b, value);
                }, 0f, .4f, .5f));
        }
        public override void RemoveSubTitle()
        {
            base.RemoveSubTitle();
            if (I_TitleCount == 0)
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
                {
                    img_Background.color = new Color(img_Background.color.r, img_Background.color.g, img_Background.color.b, value);
                }, .4f, .0f, .5f));
        }
    }
    class UIT_SubPageSelect<T> : SimpleBehaviour where T:struct
    {
        Action<int> OnPageSelected;
        Dictionary<int, Transform> dic_Page=new Dictionary<int, Transform>();
        Transform tf_SubPages;
        UIT_GridControllerMono<UIT_GridDefaultItem> gc_PageSelector;
        int i_curIndex;
        public UIT_SubPageSelect(Transform _transform, Action<int> _OnPageSelected = null) :base(_transform)
        {
            OnPageSelected = _OnPageSelected;
            gc_PageSelector = new UIT_GridControllerMono<UIT_GridDefaultItem>(transform.Find("Select/SelectGrid"), OnPageSelectorClick);
               tf_SubPages = transform.Find("SubPages");
            TCommon.TraversalEnum<T>((T type)=> {
                int index = (int)Convert.ChangeType(type, typeof(int));
                if (index == -1)
                    return;
                string key = type.ToString();
                gc_PageSelector.AddItem(index).SetItemInfo(TLocalization.Localize("UISP_"+key));
                dic_Page.Add(index, tf_SubPages.Find(key));
                dic_Page[index].SetActivate(false);
            });

            i_curIndex = 1;
            if (gc_PageSelector.Contains(1))
            {
                gc_PageSelector.OnItemSelect(1);
                dic_Page[1].SetActivate(true);
            }
        }
        void OnPageSelectorClick(int pageIndex)
        {
            if (dic_Page.ContainsKey(i_curIndex))
                dic_Page[i_curIndex].SetActivate(false);

            i_curIndex = pageIndex;

            if (dic_Page.ContainsKey(i_curIndex))
                dic_Page[i_curIndex].SetActivate(true);

            if (OnPageSelected != null)
                OnPageSelected(pageIndex);
        }
        public Transform GetPage(int type)
        {
            return dic_Page[type];
        }
    }
    class UIT_InputFieldButton : SimpleBehaviour
    {
        protected InputField if_field;
        protected Button btn_field;
        public string Text
        {
            get
            {
                return if_field.text;
            }
        }
        public UIT_InputFieldButton(Transform _transform) : base(_transform)
        {
            if_field = transform.GetComponent<InputField>();
            if_field.onValueChanged.AddListener(OnFieldTextChange);
            btn_field = transform.Find("BtnField").GetComponent<Button>();
            btn_field.onClick.AddListener(OnFieldBtnClick);
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        protected virtual void OnFieldBtnClick()
        {
        }
        protected virtual void OnFieldTextChange(string text)
        {
        }
    }
    class UIT_InputFieldClear: UIT_InputFieldButton
    {
        public UIT_InputFieldClear(Transform _transform) : base(_transform)
        {
            btn_field.interactable = if_field.text != "";
        }
        protected override void OnFieldBtnClick()
        {
            base.OnFieldBtnClick();
            if_field.text = "";
        }
        protected override void OnFieldTextChange(string text)
        {
            base.OnFieldTextChange(text);
            btn_field.interactable = if_field.text != "";
        }
    }
    class UIT_InputFieldShowState: UIT_InputFieldButton
    {
        public UIT_InputFieldShowState(Transform _transform):base(_transform)
        {
        }
        protected override void OnFieldBtnClick()
        {
            if_field.contentType = if_field.contentType == InputField.ContentType.Password ?
            InputField.ContentType.Standard : InputField.ContentType.Password;
            if_field.Select();
        }
    }
}