using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DavidJalbert;
using Lean.Touch;
using DG.Tweening;

public class LeanTinyInput : MonoBehaviour
{
    public TinyCarController carController;
    public LeanMultiSet lms;
    public Vector2 lastDelta;

    public float divider = 20f;

    [NaughtyAttributes.Button]
    public void InitLeanTinyInput()
    {
        carController = FindObjectOfType<TinyCarVisuals>().carController;
        lms = FindObjectOfType<LeanMultiSet>();
        lms.OnSetDelta.RemoveAllListeners();
        lms.OnSetDelta.AddListener(OnTouchDelta);

        HCFW.GameManager.Instance.joystick.OnSet.RemoveAllListeners();
        HCFW.GameManager.Instance.joystick.OnSet.AddListener(OnTouchDelta);
    }

    public void OnTouchDelta(Vector2 delta)
    {
        if (HCFW.GameManager.Instance.state != HCFW.GameState.InGame) { return; }
        if (lms == null)
        {
            InitLeanTinyInput();
        }
        lastDelta = delta;
    }

    public void StopCar()
    {
        if (carController != null)
        {
            //Debug.Log("Stop car");
            carController.setSteering(0);
            carController.setMotor(0);
            carController.setBoostMultiplier(0f);
        }
    }


    public void SlowDownCarToStop()
    {
        if (HCFW.GameManager.Instance.startedSlowingDown == false)
        {
            HCFW.GameManager.Instance.startedSlowingDown = true;
            DOTween.To(() => HCFW.GameManager.Instance.gasValue, x => HCFW.GameManager.Instance.gasValue = x, 0f, 2f).SetEase(Ease.Linear).SetUpdate(true).OnUpdate(() =>
            {
                if (carController != null)
                {
                    Debug.Log("Player car is slowing down...");
                    carController.setMotor(HCFW.GameManager.Instance.gasValue);
                }
            });
        }

    }

    private void Update()
    {

        if (carController == null) { return; }

        /*
        Vector3 delta = HCFW.GameManager.Instance.tcv.vehicleBody.transform.position - LevelEndManager.Instance.positionToEnd.transform.position;
        Quaternion look = Quaternion.LookRotation(delta);
        float vertical = look.eulerAngles.x;
        float horizontal = look.eulerAngles.y;

        if (horizontal < 178f)
        {
            carController.setSteering(-.35f);
        }
        else if (horizontal > 182f)
        {
            carController.setSteering(.35f);
        Debug.Log(look.eulerAngles.y);
        }*/


        switch (HCFW.GameManager.Instance.state)
        {
            case HCFW.GameState.NotInGame:
                StopCar();
                return;
            case HCFW.GameState.InGame:
                break;
            case HCFW.GameState.InEOL:
                break;
            case HCFW.GameState.FinishedEOL:
                //SlowDownCarToStop();
                return;
            default:
                break;
        }

        float steer = lastDelta.x;

        if (HCFW.GameManager.Instance.isNewTurningEnabled)
        {
            if (HCFW.GameManager.Instance.isInTurn)
            {
                steer = HCFW.GameManager.Instance.steerValueThatWasSet;
            }
        }

        //Debug.Log(steer);
        //if (Mathf.Abs(steer) < 0.5f)
        //{
        //    steer = 0f;
        //}
        carController.setSteering(steer);
        if (HCFW.GameManager.Instance.startedSlowingDown != true)
        {
            carController.setMotor(1);
        } else
        {
            var mot = carController.getMotor();
            var toSet = mot -= Time.unscaledDeltaTime;
            //Debug.Log("End, setting car motor to: " + toSet);
            if (toSet <= 0f) { toSet = 0f; }
            carController.setMotor(toSet);
        }
        //carController.setSteering(Mathf.Lerp(lastDelta.x / divider, Mathf.Clamp(lastDelta.x / divider, -1f, 1f), 0.7f));
        //carController.setSteering(Mathf.Lerp(carController.getSteering(), Mathf.Clamp(steer / divider, -1f, 1f), 0.3f));
        carController.setBoostMultiplier(3f);
    }

    public void SteerLeft()
    {
        Debug.Log("steerleft");
        carController.setSteering(-1f);
    }

    public void SteerRight()
    {
        Debug.Log("steerright");
        carController.setSteering(1f);
    }

    public void OnEnable()
    {
        // Hook into the events we need
        LeanTouch.OnFingerUp += OnFingerUp;
        LeanTouch.OnFingerSet += OnFingerSet;
    }

    public void OnDisable()
    {
        // Unhook the events
        LeanTouch.OnFingerUp -= OnFingerUp;
        LeanTouch.OnFingerSet -= OnFingerSet;
    }

    void OnFingerSet(LeanFinger finger)
    {
        //carController.setMotor(1);
    }

    void OnFingerUp(LeanFinger finger)
    {
        if (carController == null) { return; }
        lastDelta = Vector2.zero;
        carController.setSteering(0f);
    }

}
