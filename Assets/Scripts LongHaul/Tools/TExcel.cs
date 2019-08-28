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
        static List<T> l_PropertyList=null;
        public static bool B_Inited => l_PropertyList != null;
        public static int Count => PropertiesList.Count;
        public static List<T> PropertiesList
        {
            get
            {
                if (l_PropertyList != null)
                {
                    return l_PropertyList;
                }
                else
                {
                    Debug.LogError(typeof(T).ToString()+ ",Excel Not Inited,Shoulda Init Property First");
                    return null;
                }
            }
        }
        public static void Init()        //Load Sync
        {
            l_PropertyList = new List<T>();
            Type type = typeof(T);
            l_PropertyList=Tools.GetFieldData<T>(Tools.GetExcelData(type.Name), type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
        }
        public static void Clear()
        {
            l_PropertyList.Clear();
            l_PropertyList = null;
        }
    }

    class Tools
    {
        public static List<string[]> GetExcelData(string dataSource,bool extraSheets=false) 
        {
            TextAsset asset = Resources.Load<TextAsset>("Excel/" + dataSource);
            if (asset == null)
            {
                Debug.LogError("Path: Resources/Excel/" + dataSource + ".bytes Not Found");
                return null;
            }

            IExcelDataReader reader = ExcelReaderFactory.CreateBinaryReader(new MemoryStream(asset.bytes));
            List<string[]> result = new List<string[]>();
            do
            {
                while (reader.Read())
                {
                    string[] row = new string[reader.FieldCount];
                    for (int i = 0; i < row.Length; i++)
                    {
                        string data = reader.GetString(i);  
                        row[i] = data == null ? "" : data;
                    }
                    result.Add(row);
                }
            } while (extraSheets && reader.NextResult());
            return result;
        }

        public static List<T> GetFieldData<T>(List<string[]> data, FieldInfo[] fields) where T : ISExcel
        {
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