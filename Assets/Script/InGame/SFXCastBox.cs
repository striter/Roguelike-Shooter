using UnityEngine;
using GameSetting;
public class SFXCastBox : SFXCast {
    public Vector3 V3_BoxSize = Vector3.one;
    protected override Collider[] OnBlastCheck()
    {
        RaycastHit[] casts = Physics.BoxCastAll(transform.position+transform.forward*0.5f,new Vector3(V3_BoxSize.x/2, V3_BoxSize.y/2, 1f) ,transform.forward,Quaternion.LookRotation(transform.forward,transform.up),V3_BoxSize.z-1.5f,GameLayer.Physics.I_EntityOnly);
        Collider[] hits = new Collider[casts.Length];
        for (int i = 0; i < casts.Length; i++)
            hits[i] = casts[i].collider;
        return hits;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_GizmosInGame)
            return;
        Gizmos.color = GetGizmosColor();
        Gizmos_Extend.DrawWireCube(transform.position + transform.forward * V3_BoxSize.z / 2, Quaternion.LookRotation( transform.forward),V3_BoxSize);
    }
#endif
}
