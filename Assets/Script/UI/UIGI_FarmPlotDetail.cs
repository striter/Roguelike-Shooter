using UnityEngine;
using UnityEngine.UI;
public class UIGI_FarmPlotDetail : UIT_GridItem,ISingleCoroutine {
    CampFarmPlot m_Plot;
    Text m_status, m_progress, m_coin;
    public override void Init()
    {
        base.Init();
        m_status = tf_Container.Find("Status").GetComponent<Text>();
        m_progress = tf_Container.Find("Progress").GetComponent<Text>();
        m_coin = tf_Container.Find("Coin").GetComponent<Text>();
        m_coin.SetActivate(false);
    }

    private void OnDisable()
    {
        m_Plot = null;
        this.StopAllSingleCoroutines();
    }

    public void SetPlotInfo(CampFarmPlot _plot)
    {
        m_Plot = _plot;
    } 

    public void OnGenerateProfit(float profit)
    {
        m_coin.text = profit.ToString();
        m_coin.SetActivate(true);
        this.StartSingleCoroutine(0,TIEnumerators.ChangeValueTo((float value)=> { m_coin.rectTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, 200), value); },0,1,2f,()=> { m_coin.SetActivate(false); }));
    }

    private void Update()
    {
        if (!m_Plot)
            return;
        rtf_RectTransform.SetWorldViewPortAnchor(m_Plot.transform.position, CameraController.MainCamera, .1f);
        m_status.text = m_Plot.m_Status.ToString();
        m_progress.text = m_Plot.m_DecayProgress.ToString();
    }

}
