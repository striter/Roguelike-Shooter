using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {
    public Vector3 V3_RotateDireciton=Vector3.up;
    public float F_RotateSpeed;
	void Update () {
        transform.Rotate(V3_RotateDireciton*F_RotateSpeed);
	}
}
