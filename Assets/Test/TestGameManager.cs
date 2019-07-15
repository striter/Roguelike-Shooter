using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour {

    private void Start()
    {
        PostEffectManager.AddPostEffect<PE_ScreenSpaceAmbientOcclusion>();
    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            Time.timeScale = Time.timeScale == 1 ? .1f : 1f;
	}
}
