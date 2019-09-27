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
    public string formatKeys(string formatKey, string key) => base.text = string.Format(TLocalization.GetKeyLocalized(formatKey), TLocalization.GetKeyLocalized(key));
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
    float GetCalculatedSpacing(int linedLetterIndex, int lineCount)
    {
        switch (alignment)
        {
            case TextAnchor.UpperLeft:
            case TextAnchor.MiddleLeft:
            case TextAnchor.LowerLeft:
                return linedLetterIndex == 0 ? 0 : m_characterSpacing*linedLetterIndex;
            case TextAnchor.UpperRight:
            case TextAnchor.MiddleRight:
            case TextAnchor.LowerRight:
                return linedLetterIndex == lineCount - 1 ? 0 : -m_characterSpacing* linedLetterIndex;
            case TextAnchor.UpperCenter:
            case TextAnchor.MiddleCenter:
            case TextAnchor.LowerCenter:
                    return (linedLetterIndex - (lineCount / 2)+ ((lineCount+1)%2)*.5f) * m_characterSpacing;
        }
        return 0;
    }
    UIVertex m_tempVertex;
    List<UIVertex> m_AllVertexes = new List<UIVertex>();
    List<List<LetterVertex>> m_LinedLetters=new List<List<LetterVertex>>();
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);

        if (m_characterSpacing == 0)
            return;

        m_LinedLetters.Clear();
        m_AllVertexes.Clear();
        toFill.GetUIVertexStream(m_AllVertexes);
        if (m_AllVertexes.Count == 0)
            return;

        string[] letters = text.Split('\n');
        int totalVertexes=0;
        for (int i = 0; i < letters.Length; i++)
        {
            m_LinedLetters.Add(new List<LetterVertex>());
            for (int j = 0; j < letters[i].Length; j++)
                m_LinedLetters[m_LinedLetters.Count - 1].Add(new LetterVertex((totalVertexes++) * 6));
            totalVertexes++;
        }
        
        int vertexCount = 0;
        for (int i = 0; i < m_LinedLetters.Count; i++)
        {
            for (int j = 0; j < m_LinedLetters[i].Count; j++)
            {
                for (int k = 0; k < 6; k++)
                {
                    if (k > 2 && k != 4)
                        continue;
                    m_tempVertex = m_AllVertexes[m_LinedLetters[i][j].beginIndex+k];
                    m_tempVertex.position += new Vector3(GetCalculatedSpacing(j,m_LinedLetters[i].Count), 0f, 0f);
                    int uiFillVertex= vertexCount * 4 + (k == 4 ? 3 : k);
                    toFill.SetUIVertex(m_tempVertex, uiFillVertex);
                }
                vertexCount++;
            }
            vertexCount++;
        }
    }
    

    struct LetterVertex
    {
        public int letterIndex { get; private set; }
        public int beginIndex { get; private set; }
        public LetterVertex(int _startIndex)
        {
            beginIndex = _startIndex;
            letterIndex = _startIndex / 6;
        }
    }
    #endregion
}