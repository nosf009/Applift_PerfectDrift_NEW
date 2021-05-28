using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TurnTriggerType { Enter, Exit }
public enum TurnTriggerDir { Left, Right }

public class TurnTrigger : MonoBehaviour
{
    public TurnTriggerType triggerType;
    public float duration;
    public TurnTriggerDir dir;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            if (HCFW.GameManager.Instance.isNewTurningEnabled)
            {
                if (triggerType == TurnTriggerType.Enter)
                {
                    float m = 1f;
                    if (dir == TurnTriggerDir.Left)
                    {
                        m = -1f;
                        HCFW.GameManager.Instance.MenuManager.PickGreenPosition(true);
                    }
                    else
                    {
                        HCFW.GameManager.Instance.MenuManager.PickGreenPosition(false);
                    }
                    HCFW.GameManager.Instance.OnEnterTurnTrigger(duration, m);

                }
                else
                {
                    HCFW.GameManager.Instance.OnExitTurnTrigger();
                }
            }

        }

    }
}
