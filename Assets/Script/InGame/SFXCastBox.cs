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
        RaycastHit[] casts = Physics.BoxCastAll(transform.position+transform.forward*0.5f,new Vector3(m_Collider.size.x/2, m_Collider.size.y/2, 1f) ,transform.forward,Quaternion.LookRotation(transform.forward,transform.up),f_castLength-1.5f,GameLayer.Physics.I_EntityOnly);
        Collider[] hits = new Collider[casts.Length];
        for (int i = 0; i < casts.Length; i++)
            hits[i] = casts[i].collider;
        return hits;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward*3,Color.red);
    }
#endif
}
