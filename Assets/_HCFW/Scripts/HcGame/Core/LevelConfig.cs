using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCFW
{
    public class LevelConfig : MonoBehaviour
    {

        /*
         *  Use this class to define level objectives such as score to beat, elements in game etc 
         *  Think of it as a level configurator script. 
         *  Usually in games each level can have different parameters, all of which can be standardized per game type
         *  and filled out here for each level directly in editor inspector once the variables are defined.
         *  
         *  Example: public int targetScore;
         *  Another example: public Transform finishLine;
         *  etc...
         * 
         *  This is used by LevelManager to populate "Current" object, which then will be used by other managers
         * 
         */

        public int targetScore;

        [Header("Lightning")]
        public Material skyBoxMaterial;
        public Color skyColor;
        public Color equatorColor;
        public Color groundColor;

        public List<Transform> levelTransforms = new List<Transform>();

        // Use this for initialization
        void OnEnable()
        {
            // enable this, enable that, set something, etc...
            RenderSettings.skybox = skyBoxMaterial;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = skyColor;
            RenderSettings.ambientEquatorColor = equatorColor;
            RenderSettings.ambientGroundColor = groundColor;
        }

        public Transform GetClosestTransformToPlayer()
        {
            Transform closestTransform = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = GameManager.Instance.tcv.gameObject.transform.position;
            foreach (Transform potentialTarget in levelTransforms)
            {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestTransform = potentialTarget;
                }
            }
            return closestTransform;
        }

    }
}


