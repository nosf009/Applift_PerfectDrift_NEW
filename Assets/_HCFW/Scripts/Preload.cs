using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour {

	public float preloadTime = 1.5f;

	// Use this for initialization
	IEnumerator Start () {

		yield return new WaitForSeconds (preloadTime);
		SceneManager.LoadScene (1);

	}

}
