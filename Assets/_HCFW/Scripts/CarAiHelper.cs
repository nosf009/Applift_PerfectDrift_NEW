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
        tcc.setMotor(1f);
        if (startDrift)
        {
            tcc.setBoostMultiplier(2f);
            tcc.setSteering(1.5f);
            
        } else
        {
            tcc.setSteering(steer);
            //float speed = tcc.getMotor() - Time.unscaledDeltaTime;
            //if (speed < 0.7f) { speed = 0.7f; }
            //tcc.setMotor(speed);
            tcc.setBoostMultiplier(2f);
        }

    }

}
