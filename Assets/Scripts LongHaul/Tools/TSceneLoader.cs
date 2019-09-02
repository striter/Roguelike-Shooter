using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum enum_Scene
{
    Invalid = -1,
    Main = 0,
    Game = 1,
    STest = 2,
}
public class TSceneLoader:SingletonMono<TSceneLoader>,ISingleCoroutine {

    public void LoadScene(enum_Scene scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync((int)scene,LoadSceneMode.Single); 
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
