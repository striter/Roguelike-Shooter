using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<UIT_ImageExtend>().sprite = TResources.GetUIAtlas_Weapon()["detail_101"];
	}
	
}
