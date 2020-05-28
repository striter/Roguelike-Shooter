using System.Collections;
using System.Collections.Generic;
using System.Text;
using TExcel;
using UnityEngine;

public class DebugTest : MonoBehaviour {
    public int code=100001;
    const string key = "Howdy";
    private void Awake()
    {
        string s = TDataConvert.Convert(code);

        s = TDataCrypt.EasyCryptData(s, key);
        Debug.Log(s);
        s = TDataCrypt.EasyCryptData(s, key);

        int test = (int)TDataConvert.Convert(typeof(int),s);
        Debug.Log(test);
    }


}
