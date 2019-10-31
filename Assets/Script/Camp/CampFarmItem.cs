using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFarmItem : MonoBehaviour {

    CampFarmPlot m_Plot;
    public void Bind(CampFarmPlot _plot)
    {
        m_Plot = _plot;
    }

    public void Unbind()
    {
        m_Plot = null;
    }

}
