using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GamePlayTest : MonoBehaviour {

    Transform tf_CameraAttach;
    void Start () {
        tf_CameraAttach = transform.Find("CameraAttach");
        FPSCameraController.Instance.Attach(tf_CameraAttach,true);
    }
    private void Update()
    {

        tf_CameraAttach.Translate((FPSCameraController.Instance.m_Camera.transform.forward* PCInputManager.Instance.m_MovementDelta.y+FPSCameraController.Instance.m_Camera.transform.right*PCInputManager.Instance.m_MovementDelta.x)*Time.deltaTime*20f);
        FPSCameraController.Instance.RotateCamera(PCInputManager.Instance.m_RotateDelta*Time.deltaTime*50f);
        if (Input.GetKey(KeyCode.Q))
            tf_CameraAttach.Translate(FPSCameraController.Instance.m_Camera.transform.up * Time.deltaTime * 20f);
        else if(Input.GetKey(KeyCode.E))
            tf_CameraAttach.Translate(FPSCameraController.Instance.m_Camera.transform.up*-1 * Time.deltaTime * 20f);
    }

}
