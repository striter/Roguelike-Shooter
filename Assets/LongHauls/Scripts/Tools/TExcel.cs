using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Reflection;
using Excel;
using UnityEngine;
namespace TExcel
{
    public interface ISExcel
    {
        void InitOnValueSet();
    }

    class Properties<T> where T : struct,ISExcel
    {
        static List<T> m_Properties=null;
        public static bool B_Inited => m_Properties != null;
        public static int I_ColumnCount => m_Properties.Count;
        public static List<T> PropertiesList
        {
            get
            {
                if (m_Properties == null)
                {
                    Debug.LogError(typeof(T).ToString() + ",Excel Not Inited,Shoulda Init Property First");
                    return null;
                }
                return m_Properties;
            }
        }
        public static void Init()
        {
            m_Properties =  Tools.GetFieldData<T>(Tools.ReadExcelFirstSheetData(TResources.GetExcelData(typeof(T).Name, false)));
        }
        public static void Clear()
        {
            m_Properties.Clear();
            m_Properties = null;
        }
    }
    class SheetProperties<T> where T : struct, ISExcel
    {
        static Dictionary<int, List<T>> m_AllProperties = null;
        public static bool B_Inited => m_AllProperties != null;
        public int I_SheetCount => m_AllProperties.Count;
        public static List<T> GetPropertiesList(int i)
        {
            if (m_AllProperties == null)
            {
                Debug.LogError(typeof(T).ToString() + ",Excel Not Inited,Shoulda Init Property First");
                return null;
            }
            return m_AllProperties[i];
        }
        public static void Init()
        {
            m_AllProperties = new Dictionary<int, List<T>>();
            Dictionary<int, List<string[]>> m_AllDatas = Tools.ReadExcelMultipleSheetData(TResources.GetExcelData(typeof(T).Name));
            for (int i = 0; i < m_AllDatas.Count; i++)
                m_AllProperties.Add(i, Tools.GetFieldData<T>(m_AllDatas[i]));
        }
        public static void Clear()
        {
            m_AllProperties.Clear();
            m_AllProperties = null;
        }
    }

    class Tools
    {
        public static List<string[]> ReadExcelFirstSheetData(TextAsset excelAsset) => ReadExcelData(excelAsset,false)[0];
        public static Dictionary<int, List<string[]>> ReadExcelMultipleSheetData(TextAsset excelAsset) => ReadExcelData(excelAsset, true);
        static Dictionary<int, List<string[]>> ReadExcelData(TextAsset excelAsset,bool readExtraSheet)
        {
            IExcelDataReader reader = ExcelReaderFactory.CreateBinaryReader(new MemoryStream(excelAsset.bytes));
            Dictionary<int, List<string[]>> result = new Dictionary<int, List<string[]>>();
            do
            {
                result.Add(result.Count, new List<string[]>());
                while (reader.Read())
                {
                    string[] row = new string[reader.FieldCount];
                    for (int i = 0; i < row.Length; i++)
                    {
                        string data = reader.GetString(i);
                        row[i] = data == null ? "" : data;
                    }
                    if (row[0] != "")
                        result[result.Count-1].Add(row);
                }
            } while (readExtraSheet&&reader.NextResult());
            return result;
        }

        public static List<T> GetFieldData<T>(List<string[]> data) where T : ISExcel
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            List<T> targetData = new List<T>();
            try
            {
                Type type = typeof(T);

                for (int i = 0; i < fields.Length; i++)
                {
                    string temp = data[0][i].ToString();
                    if (!temp.Equals(fields[i].Name) && !temp.Equals(-1))
                    {
                        throw new Exception(" Struct Or Excel Pos Not Equals:(" + type.ToString() + "Struct Property:(Column:" + i + "|" + fields[i].Name + ") Excel Property:(Row:" + i + "|" + temp + ")");
                    }
                }
                for (int i = 0; i < data.Count; i++)
                {
                    if (i <= 1)     //Ignore Row 0 and 1
                        continue;

                    object obj = Activator.CreateInstance(type, true);
                    for (int j = 0; j < fields.Length; j++)
                    {
                        try
                        {
                            Type phraseType = fields[j].FieldType;
                            object value = null;
                            string phraseValue = data[i][j].ToString();
                            if (phraseValue.Length == 0)
                                value = TXmlPhrase.Phrase.GetDefault(phraseType);
                            else
                                value = TXmlPhrase.Phrase[phraseType, phraseValue];

                            fields[j].SetValue(obj, value);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Inner Info:|" + data[i + 1][j].ToString() + "|,Field:" + fields[j].Name + "|" + fields[j].FieldType.ToString() + ", Rows/Column:" + (i + 1).ToString() + "/" + (j + 1).ToString() + "    Message:" + e.Message);
                        }
                    }
                    T temp = (T)obj;
                    temp.InitOnValueSet();
                    targetData.Add(temp);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Excel|" + typeof(T).Name.ToString() + " Error:" + e.Message + e.StackTrace);
            }
            return targetData;
        }
    }
}