using System.Collections;
using System.Collections.Generic;
using TExcel;
using UnityEngine;
using UnityEngine.AI;

public class DebugTest : MonoBehaviour {

    public struct STest : IXmlPhrase
    {
        string test;
        string test2;
        public STest(string _test)
        {
            test = _test;
            test2 = "??";
        }

    }
     private void Awake()
    {

    }
}
