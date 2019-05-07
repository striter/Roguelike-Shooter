using UnityEngine;
using TExcel;
#pragma warning disable 0649
namespace GameSetting
{
    public static class GameConst
    {
        public static readonly int I_BulletMaxLastTime = 5;
    }
    public static class GameLayer
    {
        public static readonly int I_Static = LayerMask.NameToLayer("static");
        public static readonly int I_Entity = LayerMask.NameToLayer("entity");
    }
    #region BroadCastEnum
    enum enum_BC_UIStatusChanged
    {
        Invalid = -1,
        AmmoLeftChanged,
        PitchChanged,
    }
    #endregion
    #region GameEnum
    public enum enum_HitCheck
    {
        Invalid=-1,
        Static=1,
        Entity=2,
    }
    public enum enum_Weapon
    {
        Invalid=-1,
        Rifle=1,
        SnipeRifle=2,
    }
    public enum enum_Entity
    {
        Invalid=-1,
        Player=1,
        Dummy=2,
    }
    public enum enum_SFX
    {
        Invalid=-1,
        Bullet=1,
    }
    public static class GameEnum_Extend
    {
        public static int ToLayer(this enum_HitCheck layerType)
        {
            switch (layerType)
            {
                default:
                    Debug.LogError("Null Layer Can Be Transferd From:" + layerType.ToString());
                    return 0;
                case enum_HitCheck.Entity:
                    return GameLayer.I_Entity;
                case enum_HitCheck.Static:
                    return GameLayer.I_Static;
            }
        }
    }
    #endregion
    #region GameStruct
    public struct SEntity : ISExcel
    {
        int index;
        string s_name;
        int i_maxHealth;
        float f_moveSpeed;
        public enum_Entity m_Type =>(enum_Entity) index;
        public string m_Name => s_name;
        public int m_MaxHealth => i_maxHealth;
        public float m_moveSpeed => f_moveSpeed;
    }
    public struct SWeapon : ISExcel
    {
        int index;
        string s_name;
        float f_damage;
        float f_fireRate;
        int i_clipAmount;
        float f_reloadTime;
        float f_recoilHorizontal;
        float f_recoilVertical;
        public enum_Weapon m_Type => (enum_Weapon)index;
        public string m_Name => s_name;
        public float m_Damage => f_damage;
        public float m_FireRate => f_fireRate;
        public int m_ClipAmount => i_clipAmount;
        public float m_ReloadTime => f_reloadTime;
        public float m_RecoilHorizontal => f_recoilHorizontal;
        public float m_RecoilVertical => f_recoilVertical;
    }
    #endregion
}
