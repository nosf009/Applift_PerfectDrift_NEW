using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {

		yield return new WaitForSeconds (1f);
		SceneManager.LoadScene (1);

	}

}
