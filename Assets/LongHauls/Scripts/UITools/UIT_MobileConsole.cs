using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIT_MobileConsole : SimpleSingletonMono<UIT_MobileConsole> {

    public bool m_ShowErrorOnly = true;
    public int LogExistCount = 10;
    public int LogSaveCount = 30;
    Text m_LogText, m_FrameText;
    ObjectPoolSimpleClass<int, ConsoleCommand> m_ConsoleCommands;
    protected override void Awake()
    {
        base.Awake();
        m_LogText = transform.Find("Log").GetComponent<Text>();
        m_LogText.text = "";
        m_FrameText = transform.Find("Frame").GetComponent<Text>();

        Transform tf_ConsoleCommand = transform.Find("ConsoleCommand");
        m_ConsoleCommands = new ObjectPoolSimpleClass<int, ConsoleCommand>(tf_ConsoleCommand, "GridItem");
        transform.Find("ConsoleCommand/Cancel").GetComponent<Button>().onClick.AddListener(() => { m_ConsoleCommands.transform.SetActivate(false); });
        m_ConsoleCommands.transform.SetActivate(false);
    }

    public struct CommandBinding
    {
        public string title;
        public string defaultValue;
        public Action<string> command;
        public KeyCode keyCode;
        public static CommandBinding Create(string _title,string _defaultValue, KeyCode _keyCode, Action<string> _command) => new CommandBinding() { title = _title, command = _command, defaultValue = _defaultValue, keyCode = _keyCode };
    }
    class ConsoleCommand : CSimplePool<int>
    {
        Action<string> OnCommand;
        InputField inputField;
        KeyCode m_keyCode;
        public override void OnPoolInit(Transform _transform, int _identity)
        {
            base.OnPoolInit(_transform, _identity);
            inputField = transform.Find("InputField").GetComponent<InputField>();
            transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        public void Play(CommandBinding binding)
        {
            m_keyCode = binding.keyCode;
            inputField.SetActivate(binding.defaultValue != "");
            inputField.text = binding.defaultValue;
            transform.Find("Title").GetComponent<Text>().text = binding.title + "," + m_keyCode.ToString();
            OnCommand = binding.command;
        }

        public void EditorTick()
        {
            if (Input.GetKeyDown(m_keyCode))
                OnButtonClick();
        }

        void OnButtonClick() => OnCommand(inputField.text);
    }
    public void AddConsoleBindings(List<CommandBinding> Bindings)
    {
        m_ConsoleCommands.ClearPool();
        int commandCount=0;
        Bindings.Traversal((CommandBinding binding) => { m_ConsoleCommands.AddItem(commandCount++).Play(binding); });
    }

    private void Update()
    {
        if (Input.touchCount >= 4||Input.GetKeyDown(KeyCode.BackQuote))
            m_ConsoleCommands.transform.SetActivate(true);

        m_FrameText.text = ((int)(1 / Time.unscaledDeltaTime)).ToString();
#if UNITY_EDITOR
        m_ConsoleCommands.m_ActiveItemDic.Traversal((ConsoleCommand command) => { command.EditorTick(); });
#endif
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
