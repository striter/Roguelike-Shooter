using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {
    public float F_RotateSpeed;
	void Update () {
        transform.Rotate(0, F_RotateSpeed * Time.deltaTime, 0);
	}
}
