using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : GameManager {
    private new void Awake()
    {
        instance = this;
    }
    private void Start()
    {
//        PostEffectManager.AddPostEffect<PE_ViewDepth>();
        PostEffectManager.AddPostEffect<PE_BloomSpecific>().SetEffect(2, 10, 2);
    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            Time.timeScale = Time.timeScale == 1 ? .1f : 1f;
	}
}
