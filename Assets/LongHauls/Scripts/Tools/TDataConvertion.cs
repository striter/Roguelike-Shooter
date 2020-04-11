using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;


[Serializable]
public struct RangeFloat
{
    public float start;
    public float length;
    public float end => start + length;
    public RangeFloat(float _start, float _length)
    {
        start = _start;
        length = _length;
    }
}

#region DataPhrase
public interface IDataConvert
{
}
public static class TDataConvert
{
    static readonly char[] m_PhraseLiterateBreakPoints = new char[9] { '[', ']', '{', '}', '(', ')', '/', '|', '/' };
    const char m_PhraseBaseBreakPoint = ',';

    public static string Convert(object value) => ConvertToString(value.GetType(), value, 0);
    public static T Convert<T>(string xmlData) => (T)ConvertToObject(typeof(T), xmlData, 0);
    public static object Convert(Type type, string xmlData) => ConvertToObject(type, xmlData, 0);
    public static object Default(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
    static string ConvertToString(Type type, object value, int iteration)
    {
        if (type.IsEnum)
            return value.ToString();

        if (m_BaseTypeToXmlData.ContainsKey(type))
            return m_BaseTypeToXmlData[type](value);

        if (CheckIXmlParseType(type))
            return IXmlPhraseToString(type, value, iteration + 1);

        if (CheckListPhrase(type))
            return ListPhraseToString(type, value, iteration + 1);

        Debug.LogError("Xml Error Invlid Type:" + type.ToString() + " For Base Type To Phrase");
        return null;
    }
    static object ConvertToObject(Type type, string xmlData, int iteration)
    {
        if (type.IsEnum)
            return Enum.Parse(type, xmlData);

        if (m_BaseTypeToObject.ContainsKey(type))
            return m_BaseTypeToObject[type](xmlData);

        if (CheckIXmlParseType(type))
            return IXmlPraseToData(type, xmlData, iteration + 1);

        if (CheckListPhrase(type))
            return ListPhraseToData(type, xmlData, iteration + 1);

         Debug.LogError("Xml Error Invlid Type:" + type.ToString() + " For Xml Data To Phrase");
        return null;
    }

    #region BaseType
    static Dictionary<Type, Func<object, string>> m_BaseTypeToXmlData = new Dictionary<Type, Func<object, string>>() {
        { typeof(int), (object target) => { return target.ToString(); }},
        { typeof(long), (object target) => { return target.ToString(); } },
        { typeof(double), (object target) => { return target.ToString(); }},
        { typeof(float), (object target) => { return target.ToString(); }},
        { typeof(string), (object target) => { return target as string; }},
        {typeof(bool), (object data) => { return (((bool)data ? 1 : 0)).ToString(); }},
        { typeof(RangeInt),(object data) => { return ((RangeInt)data).start.ToString() + m_PhraseBaseBreakPoint + ((RangeInt)data).length.ToString(); } },
        { typeof(RangeFloat), (object data) => { return ((RangeFloat)data).start.ToString() + m_PhraseBaseBreakPoint + ((RangeFloat)data).length.ToString(); }}
    };
    static Dictionary<Type, Func<string, object>> m_BaseTypeToObject = new Dictionary<Type, Func<string, object>>()
    {
        { typeof(int), (string xmlData) => { return int.Parse(xmlData); }},
        { typeof(long), (string xmlData) => { return long.Parse(xmlData); } },
        { typeof(double), (string xmlData) => { return double.Parse(xmlData); }},
        { typeof(float), (string xmlData) => { return float.Parse(xmlData); } },
        { typeof(string), (string xmlData) => { return xmlData; }},
        { typeof(bool), (string xmlData) => { return int.Parse(xmlData) == 1; } },
        { typeof(RangeInt), (string xmlData) => { string[] split = xmlData.Split(m_PhraseBaseBreakPoint); return new RangeInt(int.Parse(split[0]), int.Parse(split[1])); }},
        { typeof(RangeFloat), (string xmlData) => { string[] split = xmlData.Split(m_PhraseBaseBreakPoint); return new RangeFloat(float.Parse(split[0]), float.Parse(split[1])); }},
    };
    #endregion
    #region ListType
    static bool CheckListPhrase(Type type) => type.GetGenericTypeDefinition() == typeof(List<>);
    static string ListPhraseToString(Type type, object data, int iteration)
    {
        if (iteration >= m_PhraseLiterateBreakPoints.Length)
        {
            Debug.LogError("Iteration Max Reached!");
            return "";
        }
        StringBuilder sb_xmlData = new StringBuilder();
        Type listType = type.GetGenericArguments()[0];
        char dataBreak = m_PhraseLiterateBreakPoints[iteration];
        foreach (object obj in data as IEnumerable)
        {
            sb_xmlData.Append(ConvertToString(listType, obj, iteration + 1));
            sb_xmlData.Append(dataBreak);
        }
        if (sb_xmlData.Length != 0)
            sb_xmlData.Remove(sb_xmlData.Length - 1, 1);
        return sb_xmlData.ToString();
    }

    static object ListPhraseToData(Type type, string xmlData, int iteration)
    {
        if (iteration >= m_PhraseLiterateBreakPoints.Length)
        {
            Debug.LogError("Iteration Max Reached!");
            return null;
        }
        char dataBreak = m_PhraseLiterateBreakPoints[iteration];
        Type listType = type.GetGenericArguments()[0];
        IList iList_Target = (IList)Activator.CreateInstance(type);
        string[] list_Split = xmlData.Split(dataBreak);
        if (list_Split.Length != 1 || list_Split[0] != "")
            for (int i = 0; i < list_Split.Length; i++)
                iList_Target.Add(ConvertToObject(listType, list_Split[i], iteration + 1));
        return iList_Target;
    }
    #endregion
    #region IXmlConvertType
    static readonly Type m_XmlPhraseType = typeof(IDataConvert);
    static Dictionary<Type, FieldInfo[]> m_XmlConvertFieldInfos = new Dictionary<Type, FieldInfo[]>();
    static bool CheckIXmlParseType(Type type)
    {
        if (!m_XmlPhraseType.IsAssignableFrom(type))
            return false;

        if (!m_XmlConvertFieldInfos.ContainsKey(type))
            m_XmlConvertFieldInfos.Add(type, type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic));
        return true;
    }

    static string IXmlPhraseToString(Type type, object data, int iteration)
    {
        string phrase = "";
        if (iteration >= m_PhraseLiterateBreakPoints.Length)
        {
            Debug.LogError("Iteration Max Reached!");
            return phrase;
        }
        char dataBreak = m_PhraseLiterateBreakPoints[iteration];
        int fieldLength = m_XmlConvertFieldInfos[type].Length;
        for (int i = 0; i < fieldLength; i++)
        {
            FieldInfo field = m_XmlConvertFieldInfos[type][i];
            object fieldValue = field.GetValue(data);
            string fieldString = ConvertToString(field.FieldType, fieldValue, iteration);
            phrase += fieldString;
            if (i != fieldLength - 1)
                phrase += dataBreak;
        }
        return phrase;
    }

    static object IXmlPraseToData(Type type, string data, int iteration)
    {
        object objectData = Activator.CreateInstance(type);
        if (iteration >= m_PhraseLiterateBreakPoints.Length)
        {
            Debug.LogError("Iteration Max Reached!");
            return null;
        }
        char dataBreak = m_PhraseLiterateBreakPoints[iteration];
        int fieldLength = m_XmlConvertFieldInfos[type].Length;
        string[] splitString = data.Split(dataBreak);
        if (splitString.Length != fieldLength)
            throw new Exception("Field Not Match!");
        for (int i = 0; i < fieldLength; i++)
        {
            FieldInfo field = m_XmlConvertFieldInfos[type][i];
            string fieldString = splitString[i];
            object fieldValule = ConvertToObject(field.FieldType, fieldString, iteration);
            field.SetValue(objectData, fieldValule);
        }
        return objectData;
    }
    #endregion
}
#endregion