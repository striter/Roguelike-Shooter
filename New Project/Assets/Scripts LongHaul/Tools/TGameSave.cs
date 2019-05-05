using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections;
using System.Text;

public interface ISave
{
}
public static class TGameData<T> where T : class,ISave, new()
{
    static readonly string s_Directory = Application.persistentDataPath + "/Save";
    static string s_FullPath
    {
        get
        {
            return s_Directory + "/" + typeof(T).ToString() + ".xml";
        }
    }
    static XmlDocument xml_Doc = new XmlDocument();
    static XmlElement xml_Element;
    static XmlNode xml_Node;
    static XmlNode xml_TotalNode;
    static FieldInfo[] fi_current;
    public static T Read()
    {
        if (fi_current == null)
        {
            fi_current = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            CheckFile();
        }
        return ReadFile();
    }
    public static void Save(T target)
    {
        for (int i = 0; i < fi_current.Length; i++)
        {
            xml_Node = xml_TotalNode.SelectSingleNode(fi_current[i].Name);
            xml_Node.InnerText = TXmlPhrase.Phrase[fi_current[i].FieldType, fi_current[i].GetValue(target)];
            xml_TotalNode.AppendChild(xml_Node);
        }
        xml_Doc.Save(s_FullPath);
     }
    static void CheckFile()
    {
        if (!Directory.Exists(s_Directory))
            Directory.CreateDirectory(s_Directory);
        
        try    //Check If File Complete
        {
            if (!File.Exists(s_FullPath))
                throw new Exception("None Xml Data Found:"+s_FullPath);

            xml_Doc.Load(s_FullPath);
            xml_TotalNode = xml_Doc.SelectSingleNode(typeof(T).ToString());
            if (xml_TotalNode != null)
            {
                for (int i = 0; i < fi_current.Length; i++)
                {
                    xml_Node = xml_TotalNode.SelectSingleNode(fi_current[i].Name);
                    if (xml_Node == null)
                        throw new Exception("Invalid Xml Child:"+fi_current[i].Name);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning("Xml Check File Error:"+e.Message);
            CreateDefault();
        }
    }
    static T ReadFile()
    {
        try
        {
            T temp = new T();
            for (int i = 0; i < fi_current.Length; i++)
            {
                string data = xml_TotalNode.SelectSingleNode(fi_current[i].Name).InnerText;
                fi_current[i].SetValue(temp,TXmlPhrase.Phrase[fi_current[i].FieldType, data]);
            }
            return temp;
        }
        catch(Exception e)
        {
            Debug.LogWarning("Xml Read File Error:"+e.Message);
            return CreateDefault();
        }
    }
    static T CreateDefault()
    {
        Debug.LogWarning("New Default Xml Doc Created.");
        if (File.Exists(s_FullPath))
            File.Delete(s_FullPath);
        xml_Doc = new XmlDocument();
        T temp = new T();
        xml_Element = xml_Doc.CreateElement(typeof(T).ToString());
        xml_TotalNode = xml_Doc.AppendChild(xml_Element);

        for (int i = 0; i < fi_current.Length; i++)
        {
            xml_Element = xml_Doc.CreateElement(fi_current[i].Name);
            xml_Element.InnerText =TXmlPhrase.Phrase[fi_current[i].FieldType,fi_current[i].GetValue(temp)];
            xml_TotalNode.AppendChild(xml_Element);
        }

        xml_Doc.Save(s_FullPath);
        return temp;
    }
}
public class TXmlPhrase : SingleTon<TXmlPhrase>
{
    Dictionary<Type, Func<object, string>> dic_valueToXmlData = new Dictionary<Type, Func<object, string>>();
    Dictionary<Type, Func<string, object>> dic_xmlDataToValue = new Dictionary<Type, Func<string, object>>();
    public TXmlPhrase()
    {
        dic_valueToXmlData.Add(typeof(int), (object target) => { return target.ToString(); });
        dic_xmlDataToValue.Add(typeof(int), (string xmlData) => { return int.Parse(xmlData); });
        dic_valueToXmlData.Add(typeof(float), (object target) => { return target.ToString(); });
        dic_xmlDataToValue.Add(typeof(float), (string xmlData) => { return float.Parse(xmlData); });
        dic_valueToXmlData.Add(typeof(string), (object target) => { return target as string; });
        dic_xmlDataToValue.Add(typeof(string), (string xmlData) => { return xmlData; });
    }
    public static TXmlPhrase Phrase
    {
        get
        {
            return Instance;
        }
    }
    public string this[Type type, object value]
    {
        get
        {
            StringBuilder sb_xmlData = new StringBuilder();
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type listType = type.GetGenericArguments()[0];
                    foreach (object obj in value as IEnumerable)
                    {
                        sb_xmlData.Append(ValueToXmlData(listType, obj));
                        sb_xmlData.Append(";");
                    }
                    sb_xmlData.Remove(sb_xmlData.Length - 1, 1);
                }
                else if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type keyType = type.GetGenericArguments()[0];
                    Type valueType = type.GetGenericArguments()[1];
                    foreach (DictionaryEntry obj in (IDictionary)value)
                    {
                        sb_xmlData.Append(ValueToXmlData(keyType, obj.Key));
                        sb_xmlData.Append(":");
                        sb_xmlData.Append(ValueToXmlData(valueType, obj.Value));
                        sb_xmlData.Append(";");
                    }
                    sb_xmlData.Remove(sb_xmlData.Length - 1, 1);
                }
            }
            else
            {
                sb_xmlData.Append(ValueToXmlData(type, value));
            }
            return sb_xmlData.ToString();
        }
    }
    public object this[Type type, string xmlData]
    {
        get
        {
            object obj_target = null;
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type listType = type.GetGenericArguments()[0];
                    IList iList_Target = (IList)Activator.CreateInstance(type);
                    string[] as_split = xmlData.Split(';');
                    for (int i = 0; i < as_split.Length; i++)
                    {
                        iList_Target.Add(XmlDataToValue(listType, as_split[i]));
                    }
                    obj_target = iList_Target;
                }
                else if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type keyType = type.GetGenericArguments()[0];
                    Type valueType = type.GetGenericArguments()[1];
                    IDictionary iDic_Target = (IDictionary)Activator.CreateInstance(type);
                    string[] as_split = xmlData.Split(';');
                    for (int i = 0; i < as_split.Length; i++)
                    {
                        string[] as_subSplit = as_split[i].Split(':');
                        iDic_Target.Add(XmlDataToValue(keyType, as_subSplit[0])
                            , XmlDataToValue(valueType, as_subSplit[1]));
                    }
                    obj_target = iDic_Target;
                }
            }
            else
            {
                obj_target = XmlDataToValue(type, xmlData);
            }
            return obj_target;
        }
    }
    string ValueToXmlData(Type type, object value)
    {
        if (!dic_valueToXmlData.ContainsKey(type))
            Debug.LogWarning("Xml Error Invlid Type:" + type.ToString() + " For Base Type To Phrase");
        return dic_valueToXmlData[type](value);
    }
    object XmlDataToValue(Type type, string xmlData)
    {
        if (!dic_xmlDataToValue.ContainsKey(type))
            Debug.LogWarning("Xml Error Invlid Type:" + type.ToString() + " For Xml Data To Phrase");
        return dic_xmlDataToValue[type](xmlData);
    }
}
#region elder Version
//#region Example
////class PlayerSave : TGameSave<PlayerSave>
////{
////    protected override string s_Name
////    {
////        get
////        {
////            return "PlayerSave";
////        }
////    }
////    #region Params  
////    const string ListItemsInPack = "IIP";
////    #endregion
////    public List<GSPackItem> l_CurrentItem = new List<GSPackItem>();

////    public void SaveData()
////    {
////        AdjustListData<GSPackItem>(l_CurrentItem, ListItemsInPack);
////        Save();
////    }
////    public void ReadData()
////    {
////        l_CurrentItem = ReadListData<GSPackItem>(ListItemsInPack);
////    }
////}
//#endregion
//public class TGameSave<T> : SingleTon<T>  where T:new()
//{
//    #region Const Identities
//    //Paths
//    static readonly string s_Directory = Application.persistentDataPath + "/Save";
//    protected virtual string s_Name
//    {
//        get
//        {
//            return "";
//        }
//    }
//    string s_FullPath
//    {
//        get
//        {
//            return s_Directory + "/" + s_Name+".xml";
//        }
//    }
//    //Identities
//    const string si_Version = "Version";
//    const string si_Total = "Main";
//    #endregion
//    const int i_Version = 3;  //Clear Save If Not Match
//    XmlDocument xml_Doc = new XmlDocument();
//    XmlElement xml_Element;
//    XmlNode xml_Node, xml_NodeParent, xml_TotalNode;
//    public TGameSave()
//    {
//        CheckFile();
//    }
//    void CheckFile()
//    {
//        bool needCreate = true;
//        if (!Directory.Exists(s_Directory))
//        {
//            Directory.CreateDirectory(s_Directory);
//        }
//        if (File.Exists(s_FullPath))
//        {
//            xml_Doc.Load(s_FullPath);
//            xml_TotalNode = xml_Doc.SelectSingleNode(si_Total);
//            if (xml_TotalNode == null||!(int.Parse(ReadXML(si_Version)) == i_Version))
//            {
//                File.Delete(s_FullPath);
//            }
//            else
//            {
//                needCreate = false;
//            }
//        }
//        if (needCreate)
//        {
//            xml_Doc = new XmlDocument();
//            xml_Element = xml_Doc.CreateElement(si_Total);
//            xml_TotalNode = xml_Doc.AppendChild(xml_Element);
//            UpdateData(si_Version, i_Version.ToString());
//            Save();
//        }
//    }
//    void CleanChild(string childOfIdentity)
//    {
//        xml_NodeParent = xml_TotalNode.SelectSingleNode(childOfIdentity);
//        if (xml_NodeParent != null)
//        {
//            xml_NodeParent.RemoveAll();
//        }
//    }
//    void UpdateData(string nodeIdentity, string innerText, string childOfIdentity = "")
//    {
//        xml_Node = null;
//        xml_NodeParent = null;
//        if (childOfIdentity != "")
//        {
//            xml_NodeParent = xml_TotalNode.SelectSingleNode(childOfIdentity);

//            if (xml_NodeParent == null)
//            {
//                xml_Element = xml_Doc.CreateElement(childOfIdentity);
//                xml_NodeParent = xml_TotalNode.AppendChild(xml_Element);
//                xml_Element = xml_Doc.CreateElement(nodeIdentity);
//                xml_Node = xml_NodeParent.AppendChild(xml_Element);
//            }
//            else
//            {
//                xml_Node = xml_NodeParent.SelectSingleNode(nodeIdentity);
//                if (xml_Node == null)
//                {
//                    xml_Element = xml_Doc.CreateElement(nodeIdentity);
//                    xml_Node = xml_NodeParent.AppendChild(xml_Element);
//                }
//            }
//        }
//        else
//        {
//            xml_Node = xml_TotalNode.SelectSingleNode(nodeIdentity);
//            if (xml_Node == null)
//            {
//                xml_Element = xml_Doc.CreateElement(nodeIdentity);
//                xml_Node = xml_TotalNode.AppendChild(xml_Element);
//            }
//        }

//        xml_Node.InnerText = innerText;
//    }
//    protected string ReadXML(string nodeIdentity, string childOfIdentity = "")
//    {
//        xml_Node = null;
//        xml_NodeParent = null;
//        string dataRead = "";
//        if (childOfIdentity != "")
//        {
//            xml_NodeParent = xml_TotalNode.SelectSingleNode(childOfIdentity);
//            if (xml_NodeParent != null)
//            {
//                xml_Node = xml_NodeParent.SelectSingleNode(nodeIdentity);
//            }
//        }
//        else
//        {
//            xml_Node = xml_TotalNode.SelectSingleNode(nodeIdentity);
//        }

//        if (xml_Node != null)
//        {
//            dataRead = xml_Node.InnerText;
//        }
//        //        Debug.Log(dataRead);
//        return dataRead;
//    }
//    #region Common API
//    protected void Save()
//    {
//        xml_Doc.Save(s_FullPath);
//    }
//    //Int
//    int i_temp;
//    protected void AdjustInt(int dataInfo, string nodeIdentity)
//    {
//        UpdateData(nodeIdentity, dataInfo.ToString());
//    }
//    protected int ReadInt(string nodeIdentity)
//    {
//        return int.TryParse(ReadXML(nodeIdentity), out i_temp) ? i_temp : -1;
//    }
//    //Bool
//    protected void AdjustBool(bool dataInfo, string nodeIdentity)
//    {
//        UpdateData(nodeIdentity,dataInfo.ToXmlData().ToString());
//    }
//    protected bool ReadBool(string nodeIdentity)
//    {
//        return ReadInt(nodeIdentity).ToBool10();
//    }
//    //float
//    float f_temp;
//    protected void AdjustFloat(float dataInfo, string nodeIdentity)
//    {
//        UpdateData(nodeIdentity, dataInfo.ToString());
//    }
//    protected float ReadFloat(string nodeIdentity)
//    {
//        return float.TryParse(ReadXML(nodeIdentity), out f_temp) ? f_temp : -1;
//    }
//    //Vector3
//    protected void AdjustVector3(Vector3 dataInfo, string nodeIdentity)
//    {
//        UpdateData(nodeIdentity, dataInfo.ToXmlData());
//    }
//    protected Vector3 ReadVector3(string nodeIdentity)
//    {
//       return ReadXML(nodeIdentity).XmlDataToVector3();
//    }
//    //Int List
//    protected void AdjustListData(List<int> inner, string parentNodeIdentity)
//    {
//        CleanChild(parentNodeIdentity);
//        for (int i = 0; i < inner.Count; i++)
//        {
//            UpdateData(parentNodeIdentity+i.ToString(), inner[i].ToString(), parentNodeIdentity);
//        }
//    }
//    protected List<int> ReadListData(string parentNodeIdentity)
//    {
//        try
//        {
//            xml_Node = xml_TotalNode.SelectSingleNode(parentNodeIdentity);
//            List<int> backList = new List<int>();
//            if (xml_Node != null)
//            {
//                XmlNodeList list = xml_Node.ChildNodes;
//                foreach (XmlNode node in list)
//                {
//                    backList.Add(int.Parse(node.InnerText));
//                }
//            }
//            else
//            {
//                Debug.LogWarning("Read Xml List Without Save:" + parentNodeIdentity);
//            }
//            return backList;
//        }
//        catch
//        {
//            return new List<int>();
//        }
//    }

//    //Struct List
//    protected void AdjustListData<Y>(List<Y> inner, string parentNodeIdentity) where Y : struct,ISaveMain
//    {
//        CleanChild(parentNodeIdentity);
//        FieldInfo[] fields = typeof(Y).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
//        for (int i = 0; i < inner.Count; i++)
//        {
//            for (int j = 0; j < fields.Length; j++) 
//            {
//                UpdateData(fields[j].Name + i.ToString(), fields[j].GetValue(inner[i]).ToString(),parentNodeIdentity);
//            }
//        }
//    }
//    protected List<Y> ReadListData<Y>(string parentNodeIdentity) where Y : struct,ISaveMain
//    {
//        try
//        {
//            xml_Node = xml_TotalNode.SelectSingleNode(parentNodeIdentity);
//            List<Y> tempList = new List<Y>();
//            FieldInfo[] fields = typeof(Y).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
//            int listCount = xml_Node.ChildNodes.Count / fields.Length;
//            for (int i = 0; i < listCount; i++)
//            {
//                object obj = Activator.CreateInstance(typeof(Y), true);
//                for (int j = 0; j < fields.Length; j++)
//                {
//                    XmlNode tempNode = xml_Node.SelectSingleNode(fields[j].Name + i.ToString());
//                    if (fields[j].FieldType == typeof(string))
//                    {
//                        fields[j].SetValue(obj, tempNode.InnerText);
//                    }
//                    else if (fields[j].FieldType == typeof(bool))
//                    {
//                        fields[j].SetValue(obj, int.Parse(tempNode.InnerText).ToBool10());
//                    }
//                    else
//                    {
//                        fields[j].SetValue(obj, Convert.ChangeType(tempNode.InnerText, fields[j].FieldType));
//                    }
//                }
//                tempList.Add((Y)obj);
//            }
//            return tempList;
//        }
//        catch
//        {
//            return new List<Y>();
//        }
//    }
//    #endregion
//}
#endregion