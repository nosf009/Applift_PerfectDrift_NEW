using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTriggerHelper : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            HCFW.GameManager.Instance.MenuManager.joystick.Size = HCFW.GameManager.Instance.driftSteering;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            HCFW.GameManager.Instance.MenuManager.joystick.Size = HCFW.GameManager.Instance.defaultSteering;
        }
    }

}
