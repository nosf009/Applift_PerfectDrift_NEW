using DavidJalbert;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example skid script. Put this on a WheelCollider.
// Copyright 2017 Nition, BSD licence (see LICENCE file). http://nition.co
public class WheelSkid : MonoBehaviour
{

    // INSPECTOR SETTINGS

    public Rigidbody carRb;
    public SteeringWheel steeringWheel;
    public TinyCarController tcc;
    public Vector3 offset;

    public float SKID_FX_SPEED = 0.5f; // Min side slip speed in m/s to start showing a skid
    public float MAX_SKID_INTENSITY = 20.0f; // m/s where skid opacity is at full intensity
    public float WHEEL_SLIP_MULTIPLIER = 10.0f; // For wheelspin. Adjust how much skids show
    public int lastSkid = -1; // Array index for the skidmarks controller. Index of last skidmark piece this wheel used
    public float lastFixedUpdateTime;

    // #### UNITY INTERNAL METHODS ####

    protected void Awake()
    {
        lastFixedUpdateTime = Time.time;

        tcc = carRb.GetComponent<TinyCarController>();

    }

    private void Start()
    {

    }

    protected void FixedUpdate()
    {
        lastFixedUpdateTime = Time.time;
    }

    protected void LateUpdate()
    {
        if (steeringWheel == null)
        {
            steeringWheel = FindObjectOfType<SteeringWheel>();
        }

        if (HCFW.GameManager.Instance)
        {
            if (HCFW.GameManager.Instance.isSkidding)
            {
                Vector3 skidPoint = this.transform.position + offset; //+ offset + (carRb.velocity * (Time.time - lastFixedUpdateTime));
                lastSkid = Skidmarks.Instance.AddSkidMark(skidPoint, Vector3.up, Color.black, lastSkid);


            }
            else
            {
                //lastSkid = -1;
                return;
            }
        }



        // if we on the ground
        /*
        if (tcc.isGrounded())
        {
            // Check sideways speed

            // Gives velocity with +z being the car's forward axis
            Vector3 localVelocity = transform.InverseTransformDirection(carRb.velocity);
            float skidTotal = Mathf.Abs(localVelocity.x);

            // Check wheel spin as well
            float wheelAngularVelocity = 20f * ((2 * Mathf.PI * tcc.getForwardVelocity()) / 60);
            //Debug.Log(wheelAngularVelocity);
            float carForwardVel = Vector3.Dot(carRb.velocity, transform.forward);
            float wheelSpin = Mathf.Abs(carForwardVel - wheelAngularVelocity) * WHEEL_SLIP_MULTIPLIER;

            // NOTE: This extra line should not be needed and you can take it out if you have decent wheel physics
            // The built-in Unity demo car is actually skidding its wheels the ENTIRE time you're accelerating,
            // so this fades out the wheelspin-based skid as speed increases to make it look almost OK
            wheelSpin = Mathf.Max(0, wheelSpin * (0 - Mathf.Abs(carForwardVel)));
            skidTotal += wheelSpin;

            // Skid if we should
            //if (skidTotal >= SKID_FX_SPEED true)
            {
                float intensity = 20f;//Mathf.Clamp01(skidTotal / MAX_SKID_INTENSITY);
                // Account for further movement since the last FixedUpdate
                Vector3 skidPoint = this.transform.position + offset + (carRb.velocity * (Time.time - lastFixedUpdateTime));
                lastSkid = Skidmarks.Instance.AddSkidMark(skidPoint, Vector3.up, intensity, lastSkid);

                HCFW.GameManager.Instance.driftValue += Mathf.Abs(steeringWheel.joystick.ScaledValue.x) * 0.1f * Mathf.Abs(carRb.velocity.magnitude) * Time.deltaTime;
                //HCFW.GameManager.Instance.isSkidding = true; // not the best way to implement this, because each wheel overwrites the other one, but so far it functions good
            }
            else
            {
                //HCFW.GameManager.Instance.isSkidding = false;
                lastSkid = -1;
            }
        }
        else
        {
            //HCFW.GameManager.Instance.isSkidding = false;
            lastSkid = -1;
        }*/
    }

    // #### PUBLIC METHODS ####

    // #### PROTECTED/PRIVATE METHODS ####


}
