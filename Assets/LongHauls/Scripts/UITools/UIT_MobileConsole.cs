using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIT_MobileConsole : SingletonMono<UIT_MobileConsole> {

    public bool m_ShowErrorOnly = true;
    public bool m_ConsoleOpening { get; private set; } = false;
    public int LogExistCount = 10;
    public int LogSaveCount = 30;
    Text m_LogText, m_FrameText;
    ObjectPoolListClass<int, ConsoleCommand> m_ConsoleCommands;
    Action<bool> OnConsoleShow;
    protected override void Awake()
    {
        base.Awake();
        m_LogText = transform.Find("Log").GetComponent<Text>();
        m_LogText.text = "";
        m_FrameText = transform.Find("Frame").GetComponent<Text>();

        Transform tf_ConsoleCommand = transform.Find("ConsoleCommand");
        m_ConsoleCommands = new ObjectPoolListClass<int, ConsoleCommand>(tf_ConsoleCommand, "GridItem");
        m_ConsoleOpening = false;
        m_ConsoleCommands.transform.SetActivate(m_ConsoleOpening);
    }

    public void InitConsole(Action<bool> _OnConsoleShow)
    {
        OnConsoleShow = _OnConsoleShow;
        m_ConsoleCommands.ClearPool();
    }
#region Console
    public ConsoleCommand AddConsoleBinding() => m_ConsoleCommands.AddItem(m_ConsoleCommands.Count);
    
    public class ConsoleCommand : CSimplePoolObject<int>
    {
        InputField m_ValueInput;
        EnumSelection m_ValueSelection;
        Text m_CommandTitle;
        KeyCode m_KeyCode;
        Button m_CommonButton;
        public override void OnPoolInit(Transform _transform)
        {
            base.OnPoolInit(_transform);
            m_ValueInput = transform.Find("Input").GetComponent<InputField>();
            m_ValueSelection = new EnumSelection(transform.Find("Select"));
            m_CommonButton = transform.Find("Button").GetComponent<Button>();
            m_CommandTitle = transform.Find("Button/Title").GetComponent<Text>();
        }

        public void EditorKeycodeTick()
        {
            if (Input.GetKeyDown(m_KeyCode))
                m_CommonButton.onClick.Invoke();
        }

        void Play(string title,KeyCode keyCode)
        {
            m_KeyCode = keyCode;
            m_CommandTitle.text = string.Format("{0}|{1}", title, keyCode);
            m_CommonButton.onClick.RemoveAllListeners();
            m_ValueInput.SetActivate(false);
            m_ValueSelection.transform.SetActivate(false);
        }

        public void Play(string title,KeyCode keyCode,Action OnClick)
        {
            Play(title, keyCode);
            m_CommonButton.onClick.AddListener(()=>OnClick());
        }

        int selectionIndex = -1;
        public void Play<T>(string title,KeyCode keyCode,int defaultValue,T defaultEnum ,Action<int> OnClick)
        {
            Play(title, keyCode);
            m_ValueSelection.transform.SetActivate(true);
            selectionIndex = defaultValue;
            m_ValueSelection.Init(defaultEnum, (int value)=>  selectionIndex=value );
            m_CommonButton.onClick.AddListener(() => OnClick(selectionIndex));
        }


        public void Play(string title,KeyCode keyCode, string defaultValue,Action<string> OnValueClick)
        {
            Play(title, keyCode);
            m_ValueInput.SetActivate(true);
            m_ValueInput.text = defaultValue;
            m_CommonButton.onClick.AddListener(() => OnValueClick(m_ValueInput.text));
        }
    }
#endregion
    float m_fastKeyCooldown = 0f;

    private void Update()
    {
#if UNITY_EDITOR
        m_ConsoleCommands.m_ActiveItemDic.Traversal((ConsoleCommand command) => { command.EditorKeycodeTick(); });
#endif

        m_FrameText.text = ((int)(1 / Time.unscaledDeltaTime)).ToString();
        if (m_fastKeyCooldown>0f)
        {
            m_fastKeyCooldown -= Time.unscaledDeltaTime;
            return;
        }
        if (Input.touchCount >= 4 || Input.GetKey(KeyCode.BackQuote))
        {
            m_fastKeyCooldown = .5f;
            m_ConsoleOpening = !m_ConsoleOpening;
            m_ConsoleCommands.transform.SetActivate(m_ConsoleOpening);
            OnConsoleShow?.Invoke(m_ConsoleOpening);
            UpdateLogUI();
        }
    }


    #region Log
    private void OnEnable()
    {
        Application.logMessageReceived += OnLogReceived;
    }
    private void OnDisbable()
    {
        Application.logMessageReceived -= OnLogReceived;
    }

    List<log> List_Log = new List<log>();
    struct log
    {
        public string logInfo;
        public string logTrace;
        public LogType logType;
    }
    void OnLogReceived(string info, string trace, LogType type)
    {
        if (m_ShowErrorOnly && type != LogType.Error && type != LogType.Exception)
            return;

        log tempLog = new log();
        tempLog.logInfo = info;
        tempLog.logTrace = trace;
        tempLog.logType = type;
        List_Log.Add(tempLog);
        if (List_Log.Count > LogSaveCount)
            List_Log.RemoveAt(0);
        UpdateLogUI();
    }
    void UpdateLogUI()
    {
        if (m_LogText != null)
            m_LogText.text = "";

        if(!m_ConsoleOpening)
        {
            int errorCount=0,warningCount=0,logCount=0;
            List_Log.Traversal((log l) => {
                switch(l.logType)
                {
                    case LogType.Error:errorCount++;break;
                    case LogType.Warning: warningCount++; break;
                    case LogType.Log: logCount++; break;
                }
            });
            m_LogText.text += string.Format("<color=#FFFFFF>Errors:{0},Warnings:{1},Logs:{2}</color>",errorCount,warningCount, logCount);
            return;
        }

        int startIndex = 0;
        int listCount = List_Log.Count;
        if (listCount >= LogExistCount)
        {
            startIndex = listCount - LogExistCount;
        }
        for (int i = startIndex; i < listCount; i++)
        {
            if (m_LogText != null)
                m_LogText.text += "<color=#" + LogColor(List_Log[i].logType) + ">" + List_Log[i].logInfo + "</color>\n";
        }
    }
    string LogColor(LogType type)
    {
        string colorParam = "";
        switch (type)
        {
            case LogType.Log:
                colorParam = "00FF28";
                break;
            case LogType.Warning:
                colorParam = "FFA900";
                break;
            case LogType.Exception:
            case LogType.Error:
                colorParam = "FF0900";
                break;
            case LogType.Assert:
            default:
                colorParam = "00E5FF";
                break;
        }
        return colorParam;
    }
    public void ClearLog()
    {
        List_Log.Clear();
        UpdateLogUI();
    }
    #endregion
}
