using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public static class TCommonUI
{
    public static void SetAlpha(this MaskableGraphic graphic, float alpha)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
    }
    public static void RaycastAll(Vector2 castPos)      //Bind UIT_EventTriggerListener To Items Need To Raycast By EventSystem
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = castPos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        for (int i = 0; i < results.Count; i++)
        {
            UIT_EventTriggerListener listener = results[i].gameObject.GetComponent<UIT_EventTriggerListener>();
            if (listener != null)
                listener.OnRaycast();
        }
    }
}
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
            temp.gameObject.layer = layer;
    }
    public static Transform FindInAllChild(this Transform trans, string name)
    {
        foreach (Transform temp in trans.gameObject.GetComponentsInChildren<Transform>(true))
            if (temp.name == name) return temp;
        Debug.LogWarning("Null Child Name:" + name + ",Find Of Parent:" + trans.name);
        return null;
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
    public static Color ColorAlpha(Color origin, float alpha)
    {
        return new Color(origin.r, origin.g, origin.b, alpha);
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
    public static Vector3 RotateDirection(this Vector3 Direction, Vector3 axis, float angle) => (Quaternion.AngleAxis(angle, axis) * Direction).normalized;
    public static Quaternion RandomRotation()
    {
        return Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
    }
    public static Vector3 RandomVector(float offset)
    {
        return  new Vector3(UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset));
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

    public static T RandomItem<T>(this List<T> randomList, System.Random randomSeed = null)
    {
        return randomList[randomSeed != null ? randomSeed.Next(randomList.Count) : UnityEngine.Random.Range(0, randomList.Count)];
    }
    public static int RandomIndex<T>(this List<T> randomList, System.Random randomSeed = null)
    {
        return randomSeed != null ? randomSeed.Next(randomList.Count) : UnityEngine.Random.Range(0, randomList.Count);
    }
    public static T RandomItem<T>(this T[] array, System.Random randomSeed = null)
    {
        return randomSeed != null ? array[randomSeed.Next(array.Length)] : array[UnityEngine.Random.Range(0, array.Length)];
    }
    public static T RandomItem<T>(this T[,] array, System.Random randomSeed = null)
    {
        return randomSeed != null ? array[randomSeed.Next(array.GetLength(0)), randomSeed.Next(array.GetLength(1))] : array[UnityEngine.Random.Range(0, array.GetLength(0)), UnityEngine.Random.Range(0, array.GetLength(1))];
    }
    public static void Traversal<T>(this List<T> list, Action<T> OnEachItem)
    {
        for (int i = 0; i < list.Count; i++)
            OnEachItem(list[i]);
    }
    public static void Traversal<T>(this List<T> list, Action<int, T> OnEachItem)
    {
        for (int i = 0; i < list.Count; i++)
            OnEachItem(i, list[i]);
    }
    public static void Traversal<T, Y>(this Dictionary<T, Y> dic, Action<T> OnEachKey)
    {
        foreach (T temp in dic.Keys)
            OnEachKey(temp);
    }
    public static void Traversal<T, Y>(this Dictionary<T, Y> dic, Action<Y> OnEachValue)
    {
        foreach (Y temp in dic.Values)
            OnEachValue(temp);
    }
    public static void Traversal<T, Y>(this Dictionary<T, Y> dic, Action<T, Y> OnEachPair)
    {
        foreach (T temp in dic.Keys)
            OnEachPair(temp, dic[temp]);
    }
    public static void Traversal<T>(this T[] array, Action<T> OnEachItem)
    {
        for (int i = 0; i < array.Length; i++)
            OnEachItem(array[i]);
    }
    public static void Traversal<T>(this T[,] array, Action<T> OnEachItem)
    {
        for (int i = 0; i < array.GetLength(0); i++)
            for (int j = 0; j < array.GetLength(1); j++)
                OnEachItem(array[i, j]);
    }
    public static void Traversal<T>(this T[] array, Action<T, int> OnEachItem)
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
    public static void TraversalRandom<T>(this List<T> list, Func<T, bool> OnRandomItemStop = null, System.Random seed = null)
    {
        if (list.Count == 0)
            return;

        int index = list.RandomIndex(seed);
        for (int i = 0; i < list.Count; i++)
        {
            if (OnRandomItemStop != null && OnRandomItemStop(list[index]))
                break;

            index++;
            if (index == list.Count)
                index = 0;
        }
    }
    public static void TraversalRandom<T, Y>(this Dictionary<T, Y> dictionary, Func<T, Y, bool> OnRandomItemStop = null, System.Random seed = null)
    {
        if (dictionary.Count == 0)
            return;

        int index = UnityEngine.Random.Range(0, dictionary.Count);
        for (int i = 0; i < dictionary.Count; i++)
        {
            KeyValuePair<T, Y> element = dictionary.ElementAt(index);
            if (OnRandomItemStop != null && OnRandomItemStop(element.Key,element.Value))
                break;

            index++;
            if (index == dictionary.Count)
                index = 0;
        }
    }
    #endregion
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
    public static T Find<T>(this T[,] array, Predicate<T> predicate)
    {
        for (int i = 0; i < array.GetLength(0); i++)
            for (int j = 0; j < array.GetLength(1); j++)
                if (predicate(array[i, j])) return array[i, j];
        return default(T);
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
    public static int RandomRangeInt(this RangeInt ir,System.Random seed = null)=> seed != null ? seed.Next(ir.start, ir.end + 1) : UnityEngine.Random.Range(ir.start, ir.end + 1);
    public static float RandomRangeFloat(this RangeFloat ir, System.Random seed = null)=> seed != null ? seed.Next((int)(ir.start * 1000), (int)(ir.end * 1000)) / 100 : UnityEngine.Random.Range(ir.start, ir.end);
    public static bool RandomBool(System.Random seed = null) => seed != null ? seed.Next(0, 2) > 0 : UnityEngine.Random.Range(0, 2) > 0;
    public static int RandomPercentage(System.Random seed=null)=> seed != null ? seed.Next(0, 101)  : UnityEngine.Random.Range(0, 101);
    public static Vector3 RandomXZSphere(float radius) => Vector3.forward.RotateDirection(Vector3.up, UnityEngine.Random.Range(0, 360)) * UnityEngine.Random.Range(0, radius);

    public static T RandomEnumValues<T>()        //Can't Constraint T to System.Enum
    {
        if (!typeof(T).IsSubclassOf(typeof(Enum)))
        {
            Debug.LogError("Can't Traversal EnEnum Class!");
            return default(T);
        }
        Array allEnums = Enum.GetValues(typeof(T));
        int randomIndex =UnityEngine.Random.Range(1,allEnums.Length);
        int count=0;
        foreach (object temp in allEnums)
        {
            count++;
            if (temp.ToString() == "Invalid"||count!=randomIndex)
                continue;
            return (T)temp;
        }
        return default(T);
    }
}