using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class TouchPanel : MonoBehaviour
{
    public enum SteerType { Left, Right }
    public SteerType steerType;
    LeanTinyInput lti;

    protected virtual void OnEnable()
    {
        // Hook into the events we need
        LeanTouch.OnFingerUp += OnFingerUp;
        LeanTouch.OnFingerDown += OnFingerDown;
    }

    protected virtual void OnDisable()
    {
        // Unhook the events
        LeanTouch.OnFingerUp -= OnFingerUp;
        LeanTouch.OnFingerDown -= OnFingerDown;
    }

    void OnFingerDown(LeanFinger finger)
    {
        if (lti == null)
        {
            lti = FindObjectOfType<LeanTinyInput>();
        }

        Debug.Log(this.gameObject.name + " steer " + steerType.ToString());
        if (steerType == SteerType.Left)
        {
            lti.SteerLeft();
        } else
        {
            lti.SteerRight();
        }
    }

    void OnFingerUp(LeanFinger finger)
    {
        if (lti == null)
        {
            lti = FindObjectOfType<LeanTinyInput>();
        }
    }
}
