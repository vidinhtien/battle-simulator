using System.Collections.Generic;
using BattleSimulatorV2.DamageSystem;
using BattleSimulatorV2.UnitSystem;
using UnityEngine;

namespace BattleSimulatorV2.BulletSystem
{
    public class NormalBullet : Bullet
    {
        public List<Collider> listCollider = new List<Collider>();

        
        [Tooltip("Tốc độ va chạm cần lớn hơn giá trị này để có thể gây damage")]
        [Min(0)]
        public float impactSpeedThresholdToDamage = 3;
        
        
        [Tooltip("Giá trị damage có nên lerp theo: tốc độ va chạm/muzzleSpeed")]
        public bool damageBasedOnImpactSpeed = false;
        
        
        [Tooltip("Số lần có thể gây damage. -1 nghĩa là có thể gây damage vô hạn lần")]
        public int maxDamageCount = 1; //-1 nghĩa là vô hạn

        [Tooltip("Thời gian nghỉ giữa 2 lần gây damage để tránh gây damage liên tục")]
        public float damagePerSeconds = 0.3f;

        public bool damageToTeamMates = true;
        
        [Header("Stick")]
        public bool stickOnDamage = false;
        public bool disableColliderOnStick = false;

        
        [Header("Add Impact Force")]
        [Tooltip("Khi va chạm sẽ AddForce vào đối tượng va chạm")]
        [Min(0)]
        public float addImpactForce = 0;
        public bool addImpactForceToFullBody = true;
        public ForceMode addImpactForceMode = ForceMode.VelocityChange;

        
        
        //Local Fields
        protected bool fired = false;
        protected int damageCount = 0;
        protected float lastTimeSendDamage = -999;

        
        
        public override void Fire(Vector3 direction)
        {
            damageCount = 0;
            fired = true;
            lastTimeSendDamage = -999;
            
            transform.forward = direction;
            IsKinematic = false;
            rigidbody.velocity = transform.forward * muzzleSpeed;
        }
        
        
        protected virtual void OnCollisionEnter(Collision other)
        {
            if(fired == false)
                return;
            
            //Check damageCount
            if(maxDamageCount > 0 && damageCount >= maxDamageCount)
                return;
            
            //Check time
            if(Time.time - lastTimeSendDamage < damagePerSeconds)
                return;
            
            //Check impact speed threshold
            if(other.relativeVelocity.magnitude < impactSpeedThresholdToDamage)
                return;

            //Check damageable
            var damageable = other.gameObject.GetComponentInParent<IDamageable>();
            if(damageable == null)
                return;
            
            //Check owner
            if(IsOwner(other.gameObject))
                return;

            //Check teammates
            if (damageToTeamMates == false && IsTeamMate(other.gameObject))
            {
                return;
            }
            
            
            damageCount++;
            lastTimeSendDamage = Time.time;
            SendDamageOnImpact(other, damageable);
        }

        protected virtual void SendDamageOnImpact(Collision other, IDamageable damageable)
        {
            if(damageBasedOnImpactSpeed)
                ReCalculateDamageMessage(other.relativeVelocity);

            
            damageable.TakeDamage(damageMessage);
            
            
            AddImpactForce(other.rigidbody, damageable);
            
            if(stickOnDamage)
                Stick(other.transform);
        }

        
        
        
        
        
        protected virtual void ReCalculateDamageMessage(Vector3 impactVelocity)
        {
            damageMessage.physicDamage = Mathf.Lerp(0, damageMessage.physicDamage, impactVelocity.magnitude / muzzleSpeed);
        }

        protected bool IsOwner(GameObject other)
        {
            UnitController otherUnit = other.gameObject.GetComponentInParent<UnitController>();
            UnitController ownerUnit = damageMessage.owner.GetComponentInParent<UnitController>();

            return otherUnit == ownerUnit;
        }

        protected bool IsTeamMate(GameObject other)
        {
            UnitController otherUnit = other.gameObject.GetComponentInParent<UnitController>();
            UnitController ownerUnit = damageMessage.owner.GetComponentInParent<UnitController>();
            
            return otherUnit.TeamId == ownerUnit.TeamId;
        }

        protected virtual void AddImpactForce(Rigidbody otherRigidbody, IDamageable damageable)
        {
            if (addImpactForce < 0 || Mathf.Approximately(addImpactForce, 0))
            {
                return;
            }

            Vector3 force = rigidbody.velocity.normalized * addImpactForce;
            
            if (addImpactForceToFullBody == false)
            {
                otherRigidbody.AddForce(force, addImpactForceMode);
                return;
            }
            

            var listRigidbody = damageable.GetDamageableObject().GetComponentsInChildren<Rigidbody>();
            foreach (var rigidbody in listRigidbody)
            {
                rigidbody.AddForce(force, addImpactForceMode);
            }
        }

        private void Stick(Transform target)
        {
            if (disableColliderOnStick)
            {
                foreach (var collider in listCollider)
                {
                    collider.enabled = false;
                }
            }

            Destroy(rigidbody);
            IsKinematic = true;
            rigidbody.velocity = Vector3.zero;
            transform.SetParent(target);
        }
    }
}