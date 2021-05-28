using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTutorial : MonoBehaviour
{
    public bool entered = false;
    public float duration;
    public TurnTriggerDir dir;
    public float enterDelay;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!entered)
            {
                entered = true;
                Debug.Log("Player entered tutorial trigger");
                StartCoroutine(HCFW.GameManager.Instance.TutorialCoroutine(duration, dir, enterDelay));
            }
           
        }
    }
}
