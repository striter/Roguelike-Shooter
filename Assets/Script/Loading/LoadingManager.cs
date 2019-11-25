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
public class LoadingManager : SimpleSingletonMono<LoadingManager>
{
    public SpriteAtlas m_LoadingSprites;
    public static bool m_GameBegin { get; private set; } = false;
    public static enum_Scene m_CurrentScene { get; private set; } = enum_Scene.Invalid;
    public static void BeginLoad(enum_Scene scene)
    {
        if (m_CurrentScene != enum_Scene.Invalid)
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex((int)m_CurrentScene));
        m_CurrentScene = enum_Scene.Invalid;
        SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive);
    }

    public static void OnOtherSceneEnter(enum_Scene scene)
    {
        m_CurrentScene = scene;
        m_GameBegin = true;
        if (!Instance) SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }

    GameLogo m_Logo;
    GameLoading m_Loading;
    protected override void Awake()
    {
        base.Awake();
        m_Logo = new GameLogo( transform.Find("GameLogo"));
        m_Loading = new GameLoading(transform.Find("Loading"),m_LoadingSprites);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (m_GameBegin)
            return;
        m_Logo.Begin(() => { BeginLoad(enum_Scene.Camp); });
        m_GameBegin = true;
    }

    void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        if (scene.buildIndex != (int)enum_Scene.Loading)
            SceneManager.SetActiveScene(scene);
    }

    private void Update()
    {
        m_Logo.Tick(Time.unscaledDeltaTime);
        m_Loading.Tick(Time.unscaledDeltaTime);
    }

    public void ShowLoading(enum_StageLevel level= enum_StageLevel.Invalid) => m_Loading.Begin(level);
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
            if (animationCheck < 0)
            {
                transform.SetActivate(false);
                OnFinished?.Invoke();
            }
        }
    }

    class GameLoading
    {
        AtlasAnim m_LoadingSprites;
        public Transform transform { get; private set; }
        Image m_Loading;
        Transform tf_GameStage;
        Dictionary<enum_StageLevel, RectTransform> m_Stages=new Dictionary<enum_StageLevel, RectTransform>();
        RectTransform tf_Player;
        UIT_TextExtend m_Title;
        public GameLoading(Transform _transform,SpriteAtlas atlas)
        {
            transform = _transform;
            transform.SetActivate(false);
            m_LoadingSprites = new AtlasAnim(atlas);
            m_Loading = transform.Find("Loading").GetComponent<Image>();
            m_Title = transform.Find("Title").GetComponent<UIT_TextExtend>();
            tf_GameStage = transform.Find("GameStage");
            TCommon.TraversalEnum((enum_StageLevel level) =>
            {
                m_Stages.Add(level, tf_GameStage.Find(level.ToString()).GetComponent<RectTransform>());
            });
            tf_Player = tf_GameStage.Find("Player").GetComponent<RectTransform>();
        }
        bool playing;
        public void Begin(enum_StageLevel m_Stage)
        {
            bool inGame = m_Stage != enum_StageLevel.Invalid;
            m_Title.localizeKey = inGame ? "UI_Loading_Game" : "UI_Loading_Camp";
            tf_GameStage.SetActivate(inGame);
            if (inGame) tf_Player.anchoredPosition = new Vector2(m_Stages[m_Stage].anchoredPosition.x, tf_Player.anchoredPosition.y);

            playing = true;
            transform.SetActivate(true);
            m_Loading.sprite = m_LoadingSprites.Reset();
        }
        public void Finish()
        {
            playing = false;
            transform.SetActivate(false);
        }

        public void Tick(float deltaTime)
        {
            if (!playing)
                return;

            m_Loading.sprite = m_LoadingSprites.Tick();
        }
    }
}
