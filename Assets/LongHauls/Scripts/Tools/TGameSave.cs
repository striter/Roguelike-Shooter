using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using System.Reflection;
using System;
using System.Threading.Tasks;

namespace TGameSave
{
    public interface ISave
    {
        void DataRecorrect();
    }
    public static class TGameData<T> where T : class, ISave, new()
    {
        static T m_Data = null;
        static FieldInfo[] m_fieldInfo = null;

        public static void Init() => CheckFile();

        public static T Data
        {
            get
            {
                if (m_Data == null)
                    Debug.LogError("Please Check File First!" + typeof(T).ToString());
                return m_Data;
            }
        }

        public static void Save()
        {
            if (m_Data == null)
                Debug.LogError("Please Check File First!" + typeof(T).ToString());
            Task.Run(()=> { SaveFile(m_Data); });
        }

        public static void Reset() => m_Data = new T();


        public static readonly string s_Directory = Application.persistentDataPath + "/Save";
        public static readonly string s_FilePath = s_Directory + "/" + typeof(T).Name + ".xml";
        #region Tools
        static XmlDocument m_Doc;
        static XmlNode m_ParentNode;
        static XmlElement temp_Element;
        static XmlNode temp_SubNode;

        static void CheckFile()
        {
            m_fieldInfo = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (!Directory.Exists(s_Directory))
                Directory.CreateDirectory(s_Directory);

            try    //Check If File Complete
            {
                if (!File.Exists(s_FilePath))
                    throw new Exception("None Xml Data Found:" + s_FilePath);

                m_Doc = new XmlDocument();
                m_Doc.Load(s_FilePath);
                m_ParentNode = m_Doc.SelectSingleNode(typeof(T).Name);

                if (m_ParentNode != null)
                {
                    for (int i = 0; i < m_fieldInfo.Length; i++)
                    {
                        temp_SubNode = m_ParentNode.SelectSingleNode(m_fieldInfo[i].Name);
                        if (temp_SubNode == null)
                            throw new Exception("Invalid Xml Child:" + m_fieldInfo[i].Name);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Xml Check File Error:" + e.Message);
                SaveFile(CreateDefaultDoc());
            }

            m_Data = ReadFile();
        }

        static T ReadFile()
        {
            try
            {
                T temp = new T();
                for (int i = 0; i < m_fieldInfo.Length; i++)
                {
                    string data = m_ParentNode.SelectSingleNode(m_fieldInfo[i].Name).InnerText;
                    m_fieldInfo[i].SetValue(temp, TDataConvert.Convert(m_fieldInfo[i].FieldType, data));
                }

                temp.DataRecorrect();
                return temp;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Xml Read File Error:" + e.Message);
                return CreateDefaultDoc();
            }
        }

        static void SaveFile(T data)
        {
            for (int i = 0; i < m_fieldInfo.Length; i++)
            {
                temp_SubNode = m_ParentNode.SelectSingleNode(m_fieldInfo[i].Name);
                temp_SubNode.InnerText = TDataConvert.Convert(m_fieldInfo[i].GetValue(data));
                m_ParentNode.AppendChild(temp_SubNode);
            }
            m_Doc.Save(s_FilePath);
        }

        static T CreateDefaultDoc()
        {
            Debug.LogWarning("New Default Xml Doc Created.");
            if (File.Exists(s_FilePath))
                File.Delete(s_FilePath);

            T temp = new T();

            m_Doc = new XmlDocument();
            temp_Element = m_Doc.CreateElement(typeof(T).Name);
            m_ParentNode = m_Doc.AppendChild(temp_Element);

            for (int i = 0; i < m_fieldInfo.Length; i++)
            {
                temp_Element = m_Doc.CreateElement(m_fieldInfo[i].Name);
                temp_Element.InnerText = TDataConvert.Convert(m_fieldInfo[i].GetValue(temp));
                m_ParentNode.AppendChild(temp_Element);
            }

            return temp;
        }
        #endregion
    }
}