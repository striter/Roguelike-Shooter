using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class UIT_TextLocalization : Text
{
    #region Localization
    public bool B_AutoLocalize = false;
    public string S_AutoLocalizeKey;
    protected override void Awake()
    {
        base.Awake();
        if(B_AutoLocalize)
            TLocalization.OnLocaleChanged += OnKeyLocalize;
    }
    protected override void Start()
    {
        base.Start();
        if (B_AutoLocalize&&TLocalization.IsInit)
            OnKeyLocalize();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(B_AutoLocalize)
            TLocalization.OnLocaleChanged -= OnKeyLocalize;
    }

    void OnKeyLocalize()
    {
        text = TLocalization.GetKeyLocalized(S_AutoLocalizeKey);
    }

    public string formatText(string formatKey, params object[] subItems) => base.text = string.Format(TLocalization.GetKeyLocalized(formatKey), subItems);
    public string localizeText
    {
        set
        {
            text = TLocalization.GetKeyLocalized(value);
        }
    }
    #endregion
    #region CharacterSpacing
    public float m_characterSpacing;
    UIVertex m_tempVertex;
    List<UIVertex> m_AllVertexes = new List<UIVertex>();
    List<List<LetterVertex>> m_Letters=new List<List<LetterVertex>>();
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);

        if (m_characterSpacing == 0)
            return;

        m_Letters.Clear();
        m_AllVertexes.Clear();
        toFill.GetUIVertexStream(m_AllVertexes);
        int lineIndex = 0;
        float anchoredHeight = m_AllVertexes[0].position.y;
        m_Letters.Add(new List<LetterVertex>());
        for (int i = 0; i < m_AllVertexes.Count; i+=6)
        {
            m_Letters[lineIndex].Add(new LetterVertex(i,i+6));
        }
        Debug.Log(m_Letters[0].Count);

        int indexCount = toFill.currentIndexCount;
        for (int i = 6; i < indexCount; i++)
        {
            int vertex = i % 6;
            if (vertex > 2 && vertex != 4)
                continue;
            int letterIndex = (i / 6);
            m_tempVertex = m_AllVertexes[i];
            m_tempVertex.position += new Vector3(m_characterSpacing * letterIndex, 0f, 0f);
            toFill.SetUIVertex(m_tempVertex, VertexStreamToUIVertex(letterIndex,vertex));
        }
    }

    struct LetterVertex
    {
        public int letterIndex { get; private set; }
        public int beginIndex { get; private set; }
        public int endIndex { get; private set; }
        public LetterVertex(int _startIndex,int _endIndex)
        {
            beginIndex = _startIndex;
            endIndex = _endIndex;
            letterIndex = _startIndex / 6;
        }
    }
    int VertexStreamToUIVertex(int letterIndex, int vertexIndex)=>letterIndex * 4 + (vertexIndex == 4 ? 3 : vertexIndex);
    #endregion
}