using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using ZeroX.FsmSystem;

namespace BattleSimulatorV2.StateBehaviours
{
    public class ExplosionAllWheel : MonoStateBehaviour
    {
        [Min(0)] public float explosionForce = 5;
        [Min(0)] public ForceMode explosionForceMode = ForceMode.VelocityChange;
        [Min(0)] public float explosionRadius = 3;
        
        public List<GameObject> listWheelToDisable = new List<GameObject>();
        public List<Rigidbody> listWheelToExplosion = new List<Rigidbody>();
        public bool changeParentOfWheelToExplosion = true;
        
        [Tooltip("Nếu null sẽ sử dụng vị trí của chính nó")] 
        public Transform explosionPoint;
        
        
        public override void OnStateEnter()
        {
            //Đưa các wheel lên parent.parent
            if (changeParentOfWheelToExplosion)
            {
                foreach (var wheelToExplosion in listWheelToExplosion)
                {
                    if(wheelToExplosion.transform.parent != null && wheelToExplosion.transform.parent.parent != null)
                        wheelToExplosion.transform.SetParent(wheelToExplosion.transform.parent.parent);
                }
            }
            
            
            
            //Disable wheel
            foreach (var wheelObj in listWheelToDisable)
            {
                wheelObj.SetActive(false);
            }
            
            
            
            //Explosion
            if(explosionForce <= 0)
                return;

            Vector3 explosionPos = explosionPoint == null ? transform.position : explosionPoint.position;
            
            foreach (var wheelToExplosion in listWheelToExplosion)
            {
                wheelToExplosion.gameObject.SetActive(true);
                wheelToExplosion.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 1, explosionForceMode);
            }
        }
    }
}