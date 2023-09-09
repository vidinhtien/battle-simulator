using System.Collections.Generic;
using UnityEngine;
using ZeroX.FsmSystem;

namespace BattleSimulatorV2.StateBehaviours
{
    public class ExplosionAllRigidbody : MonoStateBehaviour
    {
        [Min(0)] public float explosionForce = 5;
        [Min(0)] public ForceMode explosionForceMode = ForceMode.VelocityChange;
        [Min(0)] public float explosionRadius = 3;
        public List<Rigidbody> listRigidbody = new List<Rigidbody>();


        [Tooltip("Nếu null sẽ sử dụng vị trí của chính nó")] 
        public Transform explosionPoint;
        
        
        
        
        
        public override void OnStateEnter()
        {
            if(explosionForce <= 0)
                return;

            Vector3 explosionPos = explosionPoint == null ? transform.position : explosionPoint.position;
            
            foreach (var rigidbody in listRigidbody)
            {
                if (rigidbody.isKinematic)
                {
                    rigidbody.isKinematic = false;
                }
                rigidbody.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 1, explosionForceMode);
            }
        }
    }
}