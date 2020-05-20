using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameSetting;
using UnityEngine.U2D;
public enum enum_Scene
{
    Invalid=-1,
    Loading = 0,
    Camp = 1,
    Game = 2,
}
public class LoadingManager : SingletonMono<LoadingManager>
{
    public static bool m_GameBegin { get; private set; } = false;
    public static enum_Scene m_CurrentScene { get; private set; } = enum_Scene.Invalid;
    public static void BeginLoad(enum_Scene scene, Func<bool> onLoadFinish=null)
    {
        if (m_CurrentScene != enum_Scene.Invalid)
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        m_CurrentScene = enum_Scene.Invalid;
        Instance.StartCoroutine(LoadScene(SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive), onLoadFinish));
    }
    static IEnumerator LoadScene(AsyncOperation operation,Func<bool> OnFinishLoading)
    {
        operation.allowSceneActivation = false;
        for(; ; )
        {
            if(operation.progress>=.9f&&(OnFinishLoading==null||OnFinishLoading()))
            {
                operation.allowSceneActivation = true;
                yield break;
            }
            yield return null;
        }
    }

    public static void OnOtherSceneEnter(enum_Scene scene)
    {
        m_CurrentScene = scene;
        m_GameBegin = true;
        if (!m_HaveInstance) SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }

    GameLogo m_Logo;
    GameLoading m_Loading;

    protected override void Awake()
    {
        base.Awake();
        m_Logo = new GameLogo( transform.Find("GameLogo"));
        m_Loading = new GameLoading(transform.Find("Loading"));
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (m_GameBegin)
            return;
        m_GameBegin = true;
        SceneManager.LoadScene((int)enum_Scene.Camp);
    }

    void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        if (scene.buildIndex != (int)enum_Scene.Loading)
            SceneManager.SetActiveScene(scene);
    }

    private void Update()
    {
        m_Logo.Tick(Time.unscaledDeltaTime);
    }

    public void ShowLoading(enum_GameStage level= enum_GameStage.Invalid) => m_Loading.Begin(level);
    public void EndLoading() => m_Loading.Finish();

    class GameLogo
    {
        public Transform transform { get; private set; }
        Action OnFinished;
        public GameLogo(Transform _transform)
        {
            transform = _transform;
            transform.SetActivate(false);
        }
        float animationCheck;
        public void Begin(Action _OnFinished)
        {
            OnFinished = _OnFinished;
            transform.SetActivate(true);
            animationCheck = 5f;
        }
        
        public void Tick(float deltaTime)
        {
            if (animationCheck < 0)
                return;
            animationCheck -= deltaTime;
            if (OnFinished!=null&& animationCheck < .5f)
            {
                OnFinished?.Invoke();
                OnFinished = null;
            }

            if (animationCheck < 0)
                transform.SetActivate(false);
        }
    }

    class GameLoading
    {
        public Transform transform { get; private set; }
        Transform tf_GameStage;
        Dictionary<enum_GameStage, RectTransform> m_Stages=new Dictionary<enum_GameStage, RectTransform>();
        RectTransform tf_Player;
        UIT_TextExtend m_Title;
        public GameLoading(Transform _transform)
        {
            transform = _transform;
            transform.SetActivate(false);
            m_Title = transform.Find("Title").GetComponent<UIT_TextExtend>();
            tf_GameStage = transform.Find("GameStage");
            TCommon.TraversalEnum((enum_GameStage level) =>
            {
                m_Stages.Add(level, tf_GameStage.Find(level.ToString()).GetComponent<RectTransform>());
            });
            tf_Player = tf_GameStage.Find("Player").GetComponent<RectTransform>();
        }
        public void Begin(enum_GameStage m_Stage)
        {
            bool inGame = m_Stage != enum_GameStage.Invalid;
            m_Title.localizeKey = inGame ? "UI_Loading_Game" : "UI_Loading_Camp";
            tf_GameStage.SetActivate(inGame);
            if (inGame) tf_Player.anchoredPosition = new Vector2(m_Stages[m_Stage].anchoredPosition.x, tf_Player.anchoredPosition.y);
            transform.SetActivate(true);
        }
        public void Finish()
        {
            transform.SetActivate(false);
        }
    }
}
