using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShaderTest : MonoBehaviour
{
    public GameObject target;
    void Start ()
    {
        GetComponent<CameraEffectManager>().GetOrAddCameraEffect<PE_BloomSpecific>();
//        GetComponent<CameraEffectManager>().GetOrAddCameraEffect<CB_GenerateOpaqueTexture>();
//PE_FocalDepth focal= GetComponent<CameraEffectManager>().GetOrAddCameraEffect<PE_FocalDepth>();
//focal.SetEffect(4);
//focal.SetFocalTarget(target.transform.position,1f);

        for(int i=0;i<2000;i++)
        {
            Transform temp = GameObject.Instantiate(target).transform;
            temp.position = TCommon.RandomXZSphere(10);
        }
           
    }

}