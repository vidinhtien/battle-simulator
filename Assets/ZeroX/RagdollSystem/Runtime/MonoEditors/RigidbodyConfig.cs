using UnityEngine;

namespace ZeroX.RagdollSystem.MonoEditors
{
    [System.Serializable]
    public class RigidbodyConfig
    {
        [Min(0.00001f)]
        public float massMultiplier = 1f;
        
        [Min(0.001f)]
        public float totalMass = 50f;
        
        [Min(0)]
        public float drag = 0;
        
        [Min(0)]
        public float angularDrag = 0.05f;
        
        public bool useGravity = true;
        public bool isKinematic = false;
        public RigidbodyInterpolation interpolate = RigidbodyInterpolation.None;
        public CollisionDetectionMode collisionDetection = CollisionDetectionMode.Discrete;
    }
}