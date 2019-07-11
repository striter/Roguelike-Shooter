using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoadSceneOnClick : MonoBehaviour
{
    public void CollectionFlamethrower()  {
		SceneManager.LoadScene ("CollectionFlamethrower");
	}
    public void CollectionFull()  {
        SceneManager.LoadScene ("CollectionFull");
	}
    public void CollectionMissiles()  {
        SceneManager.LoadScene ("CollectionMissiles");
	}
    public void CollectionSmall()  {
		SceneManager.LoadScene ("CollectionSmall");
	}
    public void FX1Fire()  {
        SceneManager.LoadScene ("FX1Fire");
	}
    public void FX1FireFull()  {
        SceneManager.LoadScene ("FX1FireFull");
	}
    public void FX1Flamethrower()  {
        SceneManager.LoadScene ("FX1Flamethrower");
	}
    public void FX1Missiles()  {
        SceneManager.LoadScene ("FX1Missiles");
    }
    public void FX2Fire()  {
        SceneManager.LoadScene ("FX2Fire");
    }
    public void FX2FireFull() {
        SceneManager.LoadScene("FX2FireFull");
    }
    public void FX2Flamethrower() {
        SceneManager.LoadScene("FX2Flamethrower");
    }
    public void FX2Missiles() {
        SceneManager.LoadScene("FX2Missiles");
    }
}