using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<MeshRenderer>())
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelEndManager.Instance.OnPlayerEnteredWinTrigger();
        }
    }



}
