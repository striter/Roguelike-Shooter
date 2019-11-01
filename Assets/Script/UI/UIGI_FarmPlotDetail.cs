using UnityEngine;
using UnityEngine.UI;
public class UIGI_FarmPlotDetail : UIT_GridItem {
    CampFarmPlot m_Plot;
    Text m_status, m_progress, m_coin;
    public override void Init()
    {
        base.Init();

    }
    
    public void SetPlotInfo(CampFarmPlot _plot)=> m_Plot = _plot;

    public void Tick(int stampNow)
    {
        if (!m_Plot)
            return;
        m_status.text = m_Plot.m_Status.ToString();
        m_progress.text =(1- m_Plot.m_DecayProgress(stampNow)).ToString();
    }

    private void OnDisable()
    {
        m_Plot = null;
    }
}
