using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class GamePlayTestUI : MonoBehaviour , TReflection.UI.IUIPropertyFill
{

    Button m_Generate;
    Text m_Generate_Text;
    RawImage m_Map;
    RectTransform m_Map_Player;
    InputField m_Generate_Seed;

    public Transform GetFillParent() => transform;
    private void Awake()
    {
        TReflection.UI.UIPropertyFill(this);

        m_Generate.onClick.AddListener(OnTestGenerateClick);
    }
    void OnTestGenerateClick()
    {
        GameLevelManager.Instance.Generate(m_Generate_Seed.text);
        m_Generate_Text.text = GameLevelManager.Instance.m_Seed;
        m_Map.texture = GameLevelManager.Instance.m_MapTexture;
        m_Map.SetNativeSize();
    }

    private void Update()
    {
        m_Map_Player.anchoredPosition = GameLevelManager.Instance.GetMapPosition(CameraController.Instance.m_Camera.transform.position);
    }
}
