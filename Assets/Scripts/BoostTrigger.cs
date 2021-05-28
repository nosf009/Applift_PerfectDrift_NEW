using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTrigger : MonoBehaviour
{

    public float boostValueToSet;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerStay(Collider other)
    {
        Debug.Log($"other is {other.gameObject.name}.");
        if(other.gameObject == HCFW.GameManager.Instance.tcc.gameObject)
        {
            HCFW.GameManager.Instance.tcc.setBoostMultiplier(boostValueToSet);
        }
    }
}
