using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "BallSettingsSO", menuName = "ScriptableObjects/BallSettingsSO")]
    public class BallSettingsSO : ScriptableObject
    {
        public Rigidbody BallRigidbodyPrefab;

        public PhysicalParameters physicalParameters;

        [System.Serializable]
        public class PhysicalParameters
        {
            public float movementSpeed = 2.5f;
            public float attractionForce = 5.0f;
            public float UpperDistanceBorder = 10.0f;
            public float LowerDistanceBorder = 0.1f;
            public LayerMask RaycastMask;
        }
        public List<Vector3> positions;

        #if UNITY_EDITOR
        public CoreGameplay.BallsAttractionSystem ballsSystem { get; set; }

        private void OnValidate()
        {
            if (ballsSystem != null) 
            {
                ballsSystem.UpperDistanceBorder = physicalParameters.UpperDistanceBorder;
                ballsSystem.LowerDistanceBorder = physicalParameters.LowerDistanceBorder;
                ballsSystem.attractionForce = physicalParameters.attractionForce;
                ballsSystem.mask = physicalParameters.RaycastMask;
            }
            
        }
        #endif
    }
}
