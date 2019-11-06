using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum enum_Scene
{
    Invalid = -1,
    Camp = 0,
    Game = 1,
}
public class TSceneLoader:SingletonMono<TSceneLoader>,ISingleCoroutine {

    public void LoadScene(enum_Scene scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync((int)scene,LoadSceneMode.Single); 
        operation.allowSceneActivation = false;
        this.StartSingleCoroutine(0, LoadScene(operation));
    }
    IEnumerator LoadScene(AsyncOperation operation)
    {
        operation.allowSceneActivation = false;
        for (; ; )
        {
            if (operation.progress == .9f)
            {
                operation.allowSceneActivation = true;
                GL.Clear(true, true, Color.black);
                yield break;
            }

            yield return null;
        }
    }
}
