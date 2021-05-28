using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SteerArea { Green, Orange, Red, Invalid }

public class SteeringWheel : MonoBehaviour
{
    public LeanCircleJoystick joystick;
    public RectTransform wheelRect;
    public Vector2 wheelCenter;
    public float wheelRotationMultiplier;

    public SteerArea currentArea;


    [Header("Rotation")]
    public float zValue;

    private void Start()
    {
        joystick = FindObjectOfType<LeanCircleJoystick>();
        wheelCenter = wheelRect.position;
    }


    private void Update()
    {
        zValue = -1 * joystick.ScaledValue.x * wheelRotationMultiplier;
        wheelRect.rotation = Quaternion.Euler(0f, 0f, zValue);
        //Debug.Log(zValue);

        if (Mathf.Approximately(zValue, 0f))
        {
            currentArea = SteerArea.Invalid;
            return;
        }

        if (zValue > 0.0001f)
        {

            if (zValue > HCFW.GameManager.Instance.MenuManager.greenLowerBoundaryZ && zValue < HCFW.GameManager.Instance.MenuManager.greenHigherBoundaryZ)
            {
                currentArea = SteerArea.Green;
                //Debug.Log("<color=Green>CORRECT!</color> :: zValue:" + zValue + " is bigger than LOWER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.greenLowerBoundaryZ + " and smaller than HIGHER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.greenHigherBoundaryZ);
            }
            else  if (zValue > HCFW.GameManager.Instance.MenuManager.lowerBoundaryOrangeZ && zValue < HCFW.GameManager.Instance.MenuManager.higherBoundaryOrangeZ)
            {
                currentArea = SteerArea.Orange;
                //Debug.Log("<color=Orange>ORANGE!</color> :: zValue:" + zValue + " is bigger than LOWER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.lowerBoundaryOrangeZ + " and smaller than HIGHER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.higherBoundaryOrangeZ);
            }
            else if (zValue > HCFW.GameManager.Instance.MenuManager.lowerBoundaryRedZ && zValue < HCFW.GameManager.Instance.MenuManager.higherBoundaryRedZ)
            {
                currentArea = SteerArea.Red;
                //Debug.Log("<color=Red>RED!</color> :: zValue:" + zValue + " is bigger than LOWER BOUNDARY  " + HCFW.GameManager.Instance.MenuManager.lowerBoundaryRedZ + " and smaller than HIGHER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.higherBoundaryRedZ);
            }
            else
            {
                currentArea = SteerArea.Invalid;
            }
        }
        else
        {
            if (zValue < HCFW.GameManager.Instance.MenuManager.greenLowerBoundaryZ && zValue > HCFW.GameManager.Instance.MenuManager.greenHigherBoundaryZ)
            {
                currentArea = SteerArea.Green;
                //Debug.Log("<color=Green>CORRECT!</color> :: zValue:" + zValue + " is bigger than LOWER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.greenLowerBoundaryZ + " and smaller than HIGHER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.greenHigherBoundaryZ);
            }
            else  if (zValue < HCFW.GameManager.Instance.MenuManager.lowerBoundaryOrangeZ && zValue > HCFW.GameManager.Instance.MenuManager.higherBoundaryOrangeZ)
            {
                currentArea = SteerArea.Orange;
                //Debug.Log("<color=Orange>ORANGE!</color> :: zValue:" + zValue + " is bigger than LOWER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.lowerBoundaryOrangeZ + " and smaller than HIGHER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.higherBoundaryOrangeZ);
            }
            else if (zValue < HCFW.GameManager.Instance.MenuManager.lowerBoundaryRedZ && zValue > HCFW.GameManager.Instance.MenuManager.higherBoundaryRedZ)
            {
                currentArea = SteerArea.Red;
                //Debug.Log("<color=Red>RED!</color> :: zValue:" + zValue + " is bigger than LOWER BOUNDARY  " + HCFW.GameManager.Instance.MenuManager.lowerBoundaryRedZ + " and smaller than HIGHER BOUNDARY " + HCFW.GameManager.Instance.MenuManager.higherBoundaryRedZ);
            }
            else
            {
                currentArea = SteerArea.Invalid;
            }
        }


    }

}
