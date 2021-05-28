using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class LeanRccInput : MonoBehaviour
{
    public LeanMultiSet lms;
    public RCC_CarControllerV3 rcc;

    public Vector2 lastDelta;

    public float divider = 20f;

    protected virtual void OnEnable()
    {
        // Hook into the events we need
        LeanTouch.OnFingerUp += OnFingerUp;
    }

    protected virtual void OnDisable()
    {
        // Unhook the events
        LeanTouch.OnFingerUp -= OnFingerUp;
    }

    private void Start()
    {
        rcc = GetComponent<RCC_CarControllerV3>();
        lms = FindObjectOfType<LeanMultiSet>();
        lms.OnSetDelta.RemoveAllListeners();
        lms.OnSetDelta.AddListener(OnTouchDelta);
    }

    void OnFingerUp(LeanFinger finger)
    {
        lastDelta = Vector2.zero;
    }

    public void OnTouchDelta(Vector2 delta)
    {
        if (lms == null)
        {
            Start();
        }
        //Debug.Log(delta);
        lastDelta = delta;
        SetInputs(lastDelta);
    }

    
    void SetInputs(Vector2 delta)
    {
        //if (!HCFW.GameManager.Instance.inGame) { return; }
        //rcc.inputs.SetInput(1f, 0f, Mathf.Clamp(delta.x / 50f, -0.5f, 0.5f));
        //rcc.throttleInput = 0.1f;
        //rcc.handbrakeInput = 0.1f;
    }

    private void Update()
    {
        //if (!HCFW.GameManager.Instance.inGame) { return; }
        //Debug.Log("SettingInputs");
        rcc.inputs.SetInput(1f, 0f, Mathf.Lerp(lastDelta.x / divider, Mathf.Clamp(lastDelta.x / divider, -1f, 1f), 0.7f));
        //rcc.throttleInput = 0.5f;

        Debug.Log("Speed: " + rcc.speed + " Gear: " + rcc.currentGear);
    }

}
