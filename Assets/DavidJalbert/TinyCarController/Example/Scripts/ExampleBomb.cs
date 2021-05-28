using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavidJalbert
{
    public class ExampleBomb : MonoBehaviour
    {
        private void OnTriggerEnter(Collider collider)
        {
            /*
            TinyCarExplosiveBody car = collider.GetComponentInParent<TinyCarExplosiveBody>();
            if (car != null && !car.hasExploded())
            {
                car.explode();
                StartCoroutine(resetCar(car));
            }*/
        }

        /*
        private IEnumerator resetCar(TinyCarExplosiveBody car)
        {
            
            yield return new WaitForSeconds(2);
            car.restore();
            yield return null;
        }*/
    }
}