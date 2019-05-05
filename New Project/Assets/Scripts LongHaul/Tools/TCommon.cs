using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public static class TCommon
{
    public static void SetActivate(this MonoBehaviour behaviour, bool active)
    {
        if (behaviour.gameObject.activeSelf != active)
            SetActivate(behaviour.gameObject, active);
    }
    public static void SetActivate(this Transform tra, bool active)
    {
        SetActivate(tra.gameObject, active);
    }
    public static void SetActivate(this GameObject go, bool active)
    {
        if (go.activeSelf != active)
            go.SetActive(active);
    }
    public static void DestroyChildren(this Transform trans)
    {
        if (trans.childCount > 0)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                GameObject.Destroy(trans.GetChild(i).gameObject);
            }
        }
    }
    public static void SetChildLayer(this Transform trans, int layer)
    {
        foreach (Transform temp in trans.gameObject.GetComponentsInChildren<Transform>(true))
        {
            temp.gameObject.layer = layer;
        }
    }
    public static void SetTransformShow(Transform tra, bool active)
    {
        tra.localScale = active ? Vector3.one : Vector3.zero;
    }
    public static float GetXZDistance(Vector3 start, Vector3 end)
    {
        return new Vector2(start.x - end.x, start.z - end.z).magnitude;
    }
    public static Vector3 GetXZLookDirection(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 lookDirection = endPoint - startPoint;
        lookDirection.y = 0;
        lookDirection.Normalize();
        return lookDirection;
    }
    public static Color HexToColor(string hex)
    {
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f; float g = bg / 255f; float b = bb / 255f; float a = cc / 255f;
        return new Color(r, g, b, a);
    }
    public static float GetAngle180(float angle)
    {
        if (angle > 180)
            angle -= 360;
        return angle;
    }
    public static float GetAngle(Vector3 first, Vector3 second, Vector3 up)
    {
        float angle = Vector3.Angle(first, second);
        angle *= Mathf.Sign(Vector3.Dot(up, Vector3.Cross(first, second)));
        return angle;
    }

    public static float GetAngleY(Vector3 first, Vector3 second, Vector3 up)
    {
        Vector3 newFirst = new Vector3(first.x, 0, first.z);
        Vector3 newSecond = new Vector3(second.x, 0, second.z);
        return GetAngle(newFirst, newSecond, up);
    }

    public static float GetIncludedAngle(float angle1, float angle2)
    {
        float angle = 0;
        if (angle1 >= 270 && angle2 < 90)
        {
            angle = (angle1 - (angle2 + 360)) % 180;
        }
        else if (angle1 <= 90 && angle2 >= 270)
        {
            angle = (angle1 + 360 - angle2) % 180;
        }
        else
        {
            angle = (angle1 - angle2);
            if (Mathf.Abs(angle) > 180)
            {
                angle -= 360;
            }
        }
        return angle;
    }
    public static Quaternion RandomRotation()
    {
        return Quaternion.Euler(UnityEngine.Random.Range(0,360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
    }
    public static Vector3 RandomPositon(Vector3 startPosition, float offset = .2f)
    {
        return startPosition + new Vector3(UnityEngine.Random.Range(-offset,offset), UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset));
    }
    public static Transform FindOrCreateNewTransform(this Transform parentTrans, string name)
    {
        Transform toTrans;
        toTrans = parentTrans.Find(name);
        if (toTrans == null)
        {
            toTrans = new GameObject().transform;
            toTrans.SetParent(parentTrans);
            toTrans.name = name;
        }
        return toTrans;
    }
    #region List/Array/Enum Traversal
    public static List<int> SplitIndexComma(this string toSplit)
    {
        List<int> indexes = new List<int>();
        string[] splitIndexes = toSplit.Split(',');
        for (int i = 0; i < splitIndexes.Length; i++)
        {
            indexes.Add(int.Parse(splitIndexes[i]));
        }
        return indexes;
    }
    public static T ListRandom<T>(this List<T> randomList)
    {
        return randomList[UnityEngine.Random.Range(0, randomList.Count)];
    }
    public static int ListRandomIndex<T>(this List<T> randomList)
    {
        return UnityEngine.Random.Range(0, randomList.Count);
    }
    public static void TraversalList<T>(this List<T> list, Action<T> OnEachItem)
    {
        for (int i = 0; i < list.Count; i++)
        {
            OnEachItem(list[i]);
        }
    }
    public static void TraversalList<T>(this List<T> list, Action<int, T> OnEachItem)
    {
        for (int i = 0; i < list.Count; i++)
            OnEachItem(i, list[i]);
    }
    public static void TraversalDicKeys<T, Y>(this Dictionary<T, Y> dic, Action<T> OnEachKey)
    {
        foreach (T temp in dic.Keys)
            OnEachKey(temp);
    }
    public static void TraversalDicValues<T, Y>(this Dictionary<T, Y> dic, Action<Y> OnEachValue)
    {
        foreach (Y temp in dic.Values)
            OnEachValue(temp);
    }
    public static void TraversalDic<T, Y>(this Dictionary<T, Y> dic, Action<T, Y> OnEachPair)
    {
        foreach (T temp in dic.Keys)
            OnEachPair(temp, dic[temp]);
    }
    public static void TraversalArray<T>(this T[] array, Action<T> OnEachItem)
    {
        for (int i = 0; i < array.Length; i++)
            OnEachItem(array[i]);
    }
    public static void TraversalArray<T>(this T[] array, Action<T, int> OnEachItem)
    {
        for (int i = 0; i < array.Length; i++)
            OnEachItem(array[i], i);
    }
    public static void TraversalEnum<T>(Action<T, int> enumAction)    //Can't Constraint T to System.Enum?
    {
        if (!typeof(T).IsSubclassOf(typeof(Enum)))
        {
            Debug.LogError("Can't Traversal EnEnum Class!");
            return;
        }

        foreach (object temp in Enum.GetValues(typeof(T)))
        {
            if (temp.ToString() == "Invalid")
                continue;
            enumAction((T)temp, (int)temp);
        }
    }

    public static void TraversalEnum<T>(Action<T> enumAction)    //Can't Constraint T to System.Enum?
    {
        if (!typeof(T).IsSubclassOf(typeof(Enum)))
        {
            Debug.LogError("Can't Traversal EnEnum Class!");
            return;
        }

        foreach (object temp in Enum.GetValues(typeof(T)))
        {
            if (temp.ToString() == "Invalid")
                continue;
            enumAction((T)temp);
        }
    }

    public static string ToStringLog<T>(this List<T> tempList)
    {
        string target = "";
        for (int i = 0; i < tempList.Count; i++)
        {
            target += tempList[i].ToString();
            target += " ";
        }
        return target;
    }
    #endregion
    public static T GetComponentNullable<T>(this Transform parent) where T : MonoBehaviour
    {
        if (parent == null)
            return null;
        return parent.GetComponent<T>();
    }
    public static void SortChildByNameIndex(Transform transform, bool higherUpper = true)
    {
        List<Transform> childList = new List<Transform>();
        List<int> childIndexList = new List<int>();

        for (int i = 0; i < transform.childCount; i++)
        {
            childList.Add(transform.GetChild(i));
            childIndexList.Add(int.Parse(childList[i].gameObject.name));
        }
        childIndexList.Sort((a, b) => { return a <= b ? (higherUpper ? 1 : -1) : (higherUpper ? -1 : 1); });

        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].SetSiblingIndex(childIndexList.FindIndex(p => p == int.Parse(childList[i].name)));
        }
    }
    public static void SetAlpha(this MaskableGraphic graphic, float alpha)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
    }

    public static string ToLogText<T, Y>(this Dictionary<T, Y> dic)
    {
        string target = "";
        foreach (T temp in dic.Keys)
        {
            target += temp.ToString() + "|" + dic[temp].ToString() + " ";
        }
        return target;
    }

    //public static void InitComponent<T>(this T initItem,Transform parentTransform)  //Test Try Init Item Within One Func
    //{
    //    initItem = parentTransform.Find(initItem.ToString()).GetComponent<T>();
    //    if (initItem == null)
    //        Debug.LogError("Null Path Of "+parentTransform.name+"/" +"");
    //}
}

public static class TXmlParse
{
    public static string ToXmlData(this Quaternion qt)
    {
        return qt.x.ToString() + "," + qt.y.ToString() + "," + qt.z.ToString() + "," + qt.w.ToString();
    }
    public static Quaternion XmlDataToQuaternion(this string data)
    {
        try
        {
            Quaternion qt = Quaternion.identity;
            string[] split = data.Split(',');
            qt = new Quaternion(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
            return qt;
        }
        catch
        {
            return Quaternion.identity;
        }
    }
    public static string ToXmlData(this Vector3 v3)
    {
        return v3.x.ToString() + "," + v3.y.ToString() + "," + v3.z.ToString();
    }
    public static Vector3 XmlDataToVector3(this string data)
    {
        try
        {
            Vector3 v3 = Vector3.zero;
            string[] split = data.Split(',');
            v3 = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
            return v3;
        }
        catch
        {
            return Vector3.zero;
        }
    }

    public static int ToXmlData(this bool b)
    {
        return b ? 1 : 0;
    }
    public static bool ToBool10(this int i)
    {
        return i == 1;
    }
}