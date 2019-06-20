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
        public static List<T> Init(string _extraPath="")        //Load Sync
        {
            TextAsset asset = Resources.Load<TextAsset>("Excel/" + typeof(T).Name.ToString() + _extraPath);
            if (asset == null)
            {
                Debug.LogError("Path: Resources/Excel/" + typeof(T).Name.ToString() + _extraPath+ ".bytes Not Found");
                return null;
            }
            SetUpList(asset.bytes);
            return l_PropertyList;
        }

        static void SetUpList(byte[] bytes)
        {
            try
            {

                l_PropertyList = new List<T>();
                IExcelDataReader reader = ExcelReaderFactory.CreateBinaryReader(new MemoryStream(bytes));
                DataSet result = reader.AsDataSet();

                Type type = typeof(T);
                object obj = Activator.CreateInstance(type, true);
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < fields.Length; i++)
                {
                    string temp = result.Tables[0].Rows[0][i].ToString();
                    if (!temp.Equals(fields[i].Name) && !temp.Equals(-1))
                    {
                        throw new Exception(" Struct Or Excel Pos Not Equals:(" + type.ToString() + "Struct Property:(Column:"+i+"|" + fields[i].Name + ") Excel Property:(Row:"+  i +"|" + temp + ")");
                    }
                }

                for (int i = 0; i < result.Tables[0].Rows.Count - 1; i++)
                {
                    for (int j = 0; j < fields.Length; j++)
                    {
                        try
                        {
                            fields[j].SetValue(obj, TXmlPhrase.Phrase[fields[j].FieldType, result.Tables[0].Rows[i + 1][j].ToString()]);
                        }
                        catch(Exception e)
                        {
                            throw new Exception("Invalid Info:'"+ result.Tables[0].Rows[i + 1][j].ToString()+"'" + ",FieldType:" + fields[j].FieldType.ToString() + ", Rows/Column:" + (i+2).ToString() + "/" + (j+1).ToString()+"    Message:"+e.Message);
                        }
                    }
                    T temp = (T)obj;
                    temp.InitOnValueSet();
                    l_PropertyList.Add(temp);
                }
            }
            catch(Exception e)
            {
                Debug.LogError("Excel|"+typeof(T).Name.ToString()+" Error:"+e.Message);
            }
        }

        // Abandoned Cause No Such Huge Excel Needs To Load Async!
        //static Coroutine cor_Load;
        //public static void InitAsync(Delegates.DelVoid OnFinished,string _extraPath="")
        //{
        //    s_extraPath = _extraPath;
        //    TCoroutine.SafeStartCoroutine(ref cor_Load,(LoadExcelAsync(OnFinished)));
        //}
        //static  IEnumerator LoadExcelAsync(Delegates.DelVoid OnFinished)
        //{
        //    for (; ; )
        //    {
        //        WWW file = new WWW(Application.streamingAssetsPath + "/Excel/" + typeof(T).Name.ToString() + s_extraPath + ".xls");
        //        while (!file.isDone)
        //        {
        //            yield return null;
        //        }
        //        SetUpList(file.bytes);
        //        OnFinished();
        //        yield break;
        //    }
        //}
    }
}