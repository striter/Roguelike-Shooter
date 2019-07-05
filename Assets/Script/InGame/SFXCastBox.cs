using UnityEngine;
using GameSetting;
[RequireComponent(typeof(BoxCollider))]
public class SFXCastBox : SFXCast {

    protected BoxCollider m_Collider;
    protected float f_castLength;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Collider = GetComponent<BoxCollider>();
        m_Collider.enabled = false;
        f_castLength = m_Collider.size.z;
    }
    protected override Collider[] OnBlastCheck()
    {
        RaycastHit[] casts = Physics.BoxCastAll(transform.position,new Vector3(m_Collider.size.x/2, m_Collider.size.y/2, .01f) ,transform.forward,Quaternion.LookRotation(transform.forward,transform.up),f_castLength,GameLayer.Physics.I_EntityOnly);
        Collider[] hits = new Collider[casts.Length];
        for (int i = 0; i < casts.Length; i++)
            hits[i] = casts[i].collider;
        return hits;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position,Vector3.forward*3,Color.red);
    }
#endif
}
