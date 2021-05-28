using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DavidJalbert;

public class CarAiHelper : MonoBehaviour
{
    TinyCarController tcc;
    public float distanceTreshold = 10f;
    public Transform currentTarget;
    Transform trans;

    // Start is called before the first frame update
    void Start()
    {
        tcc = GetComponentInChildren<TinyCarController>();
        trans = transform.GetChild(0).GetChild(0);
        currentTarget = LevelEndManager.Instance.moveToThisTransform;
        distanceTreshold = 20f;
    }

    public float timeDividerOnDonut = 1f;
    public void InitSelf(SteerArea picked)
    {
        switch (picked)
        {
            case SteerArea.Green:
                timeDividerOnDonut = 10f;
                break;
            case SteerArea.Orange:
                timeDividerOnDonut = 6f;
                break;
            case SteerArea.Red:
                timeDividerOnDonut = 6f;
                break;
            case SteerArea.Invalid:
                timeDividerOnDonut = 2f;
                break;
            default:
                break;
        }
        this.enabled = true;
    }

    bool startDrift = false;
    private void Update()
    {
        Vector3 localTarget = trans.InverseTransformPoint(currentTarget.position);
        var targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp((targetAngle / tcc.maxSteering), -1f, 1f); // / 100;

        //tcc.setBoostMultiplier(3f);
        
        Vector3 directionToTarget = currentTarget.transform.position - trans.position;
        float dSqrToTarget = directionToTarget.sqrMagnitude;
        //Debug.Log(dSqrToTarget);
        if (dSqrToTarget < distanceTreshold * distanceTreshold && startDrift == false)
        {
            //Debug.Log(currentTarget.tag);
            if (currentTarget.tag == "DOTweenPath")
            {
                Debug.Log("ksfksdfskfdf");
                currentTarget = GameObject.FindGameObjectWithTag("EolTarget").transform;
            } else
            {
                Debug.Log("startDrift = true;");
                startDrift = true;
            }
        }

        if (startDrift)
        {
            float speed = tcc.getMotor() - (Time.unscaledDeltaTime / timeDividerOnDonut);
            if (speed < 0.0f) { speed = 0.0f; }
            if (speed > 0f)
            {
                tcc.setBoostMultiplier(2f);
                tcc.setSteering(1.5f);
            } else
            {
                tcc.setBoostMultiplier(0f);
                tcc.setSteering(0f);
            }
            tcc.setMotor(speed);

        }
        else
        {
            tcc.setSteering(steer);
            tcc.setMotor(1f);
            //float speed = tcc.getMotor() - Time.unscaledDeltaTime;
            //if (speed < 0.7f) { speed = 0.7f; }
            //tcc.setMotor(speed);
            tcc.setBoostMultiplier(2f);
        }

    }

}
