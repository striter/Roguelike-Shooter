using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesViewerManager : MonoBehaviour {

    Transform tf_Muzzle, tf_Indicator, tf_Impact,tf_Trail;
    ObjectPoolListComponent<int,Transform> m_TrailHelperPool;
    TimeCounter m_Repeater=new TimeCounter(2f);
    private void Awake()
    {
        GameObjectManager.Init();
        GameObjectManager.PresetRegistCommonObject();
        tf_Muzzle = transform.Find("Muzzle");
        tf_Indicator = transform.Find("Indicator");
        tf_Impact = transform.Find("Impact");
        tf_Trail = transform.Find("Trail");
        m_TrailHelperPool = new ObjectPoolListComponent<int, Transform>(transform.Find("TrailHelper"), "Transform");
    }

    private void Update()
    {
        m_TrailHelperPool.m_ActiveItemDic.Traversal((Transform transform) => { transform.Translate(Vector3.forward * Time.deltaTime*10, Space.World); });

        if (Input.GetKeyDown(KeyCode.BackQuote))
            Time.timeScale = Time.timeScale == 1 ? .1f : 1;

        m_Repeater.Tick(Time.deltaTime);
        if (m_Repeater.m_Timing)
            return;
        m_Repeater.Replay();

        ObjectPoolManager<int, SFXBase>.RecycleAll();
        m_TrailHelperPool.ClearPool();
        int muzzleIndex = 0;
        int indicatorIndex = 0;
        int impactIndex = 0;
        int trailIndex = 0;
        Quaternion rotation = Quaternion.LookRotation(Vector3.up,Vector3.left);
        ObjectPoolManager<int, SFXBase>.GetRegistedList().Traversal((int identity) =>
        {
            SFXBase particle = ObjectPoolManager<int, SFXBase>.GetRegistedSpawnItem(identity);
            if ((particle as SFXIndicator) != null)
                (ObjectPoolManager<int, SFXBase>.Spawn(identity, tf_Indicator, new Vector3(-10, 0, indicatorIndex++ * 3), rotation) as SFXIndicator).PlayUncontrolled(-1);
            if ((particle as SFXMuzzle) != null)
                (ObjectPoolManager<int, SFXBase>.Spawn(identity, tf_Muzzle, new Vector3(0, 0, muzzleIndex++*3), rotation) as SFXMuzzle).PlayUncontrolled(-1);
            if ((particle as SFXImpact) != null)
                (ObjectPoolManager<int, SFXBase>.Spawn(identity, tf_Impact, new Vector3(5, 0, impactIndex++*3), rotation) as SFXImpact).PlayUncontrolled(-1);
            if ((particle as SFXTrail) != null)
            {
                Transform attachTrans = m_TrailHelperPool.AddItem(trailIndex);
                rotation = Quaternion.LookRotation(Vector3.forward);
                Vector3 position = new Vector3(10, trailIndex++ * 2, 0);
                attachTrans.position = position;
                SFXTrail trail = (ObjectPoolManager<int, SFXBase>.Spawn(identity, tf_Trail,position, rotation) as SFXTrail);
                trail.PlayControlled(-1);
                trail.AttachTo(attachTrans);
            }
        });
    }
}
