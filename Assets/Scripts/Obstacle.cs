using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount > 0)
        {
            if (collision.contacts[0].otherCollider.gameObject.CompareTag("Player"))
            {
                HCFW.GameManager.Instance.ResetCarToPoint();
            }
        }
    }

}