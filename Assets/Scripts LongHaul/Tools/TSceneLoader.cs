﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TSceneLoader:SimpleSingletonMono<TSceneLoader>,ISingleCoroutine {
    public enum enum_Scene
    {
        Invalid = -1,
        Main=0,
        Game=1,
        STest=2,
    }
    public void LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(0,LoadSceneMode.Single); 
        operation.allowSceneActivation = false;
        this.StartSingleCoroutine(0, TIEnumerators.Tick(() =>
        {
            Debug.Log(operation.progress);
            if (operation.progress == .9f)
            {
                operation.allowSceneActivation = true;
                this.StopSingleCoroutine(0);
            }
        },.3f));
    }
}
