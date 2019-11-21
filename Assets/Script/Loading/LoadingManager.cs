using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum enum_Scene
{
    Invalid=-1,
    Loading = 0,
    Camp = 1,
    Game = 2,
}
public class LoadingManager : SimpleSingletonMono<LoadingManager>, ISingleCoroutine
{
    public static bool m_ShowEnterTitle { get; private set; } = false;
    public static enum_Scene m_CurrentScene { get; private set; } = enum_Scene.Invalid;

    public static void BeginLoad(enum_Scene scene)
    {
        if (m_CurrentScene != enum_Scene.Invalid)
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex((int)m_CurrentScene));
        m_CurrentScene = enum_Scene.Invalid;
        SceneManager.LoadScene((int)scene, LoadSceneMode.Additive);
    }

    public static void OnOtherSceneEnter(enum_Scene scene)
    {
        m_CurrentScene = scene;
        m_ShowEnterTitle = true;
        if (!Instance) SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }

    Image m_Test;
    protected override void Awake()
    {
        base.Awake();
        m_Test = GetComponentInChildren<Image>();
        if (!m_ShowEnterTitle) ShowEnterLog();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.StopAllSingleCoroutines();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        if (scene.buildIndex == (int)enum_Scene.Loading)
            return;
        SceneManager.SetActiveScene(scene);
    }

    void ShowEnterLog()
    {
        m_ShowEnterTitle = true;
        m_Test.SetActivate(true);
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => { m_Test.color = TCommon.ColorAlpha(m_Test.color, value); }, 0, 1, 1f, OnGameEnter));
    }

    void OnGameEnter()
    {
        m_Test.SetActivate(false);
        BeginLoad(enum_Scene.Camp);
    }
}
