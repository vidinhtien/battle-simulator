using System;
using BattleSimulatorV2.DamageSystem;
using BattleSimulatorV2.UnitSystem;
using UnityEngine;

namespace BattleSimulatorV2.WeaponSystem
{
    public class MeleeWeapon : Weapon
    {
        public UnitController ownerUnit;
        [SerializeField] protected bool canSendDamage = false;
        public float damagePerSeconds = 0.3f;

        [Tooltip("Tốc độ va chạm cần lớn hơn giá trị này để có thể gây damage")]
        [Min(0)] public float impactSpeedThresholdToDamage = 1;
        
        [Tooltip("Giá trị damage có nên lerp theo: tốc độ va chạm/muzzleSpeed")]
        public bool damageBasedOnImpactSpeed = false;
        
        [Tooltip("Giá trị để tính toán damageBasedOnImpactSpeed")]
        public float maxImpactSpeed = 3;
        
        public bool damageToTeamMates = false;
        
        [Header("Add Impact Force")]
        
        [Tooltip("Khi va chạm sẽ AddForce vào đối tượng va chạm")]
        [Min(0)] public float addImpactForce = 5;
        
        public bool addImpactForceToFullBody = false;
        public ForceMode addImpactForceMode = ForceMode.VelocityChange;
        public float addImpactUpForce = 1;


        
        //Properties
        public bool CanSendDamage => canSendDamage;
        
        
        //Local Fields
        private float lastTimeSendDamage = -999;


        
        
        
        private void Reset()
        {
            rigidbody = GetComponent<Rigidbody>();
            ownerUnit = GetComponentInParent<UnitController>();
        }


        private void OnCollisionEnter(Collision other)
        {
            if(canSendDamage == false)
                return;
            
            if(ownerUnit.UnitTrait.IsDead)
                return;
            
            if(Time.time - lastTimeSendDamage < damagePerSeconds)
                return;

            //Check impact speed threshold
            if(other.relativeVelocity.magnitude < impactSpeedThresholdToDamage)
                return;
            
            //Check nếu va vào vũ khí khác thì ko gây damage
            if(other.gameObject.GetComponentInParent<Weapon>() != null)
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
            
            
            SendDamageOnImpact(other, damageable);
            lastTimeSendDamage = Time.time;
        }
        
        protected bool IsOwner(GameObject other)
        {
            UnitController otherUnit = other.gameObject.GetComponentInParent<UnitController>();

            return otherUnit == ownerUnit;
        }
        
        protected bool IsTeamMate(GameObject other)
        {
            UnitController otherUnit = other.gameObject.GetComponentInParent<UnitController>();

            return otherUnit.TeamId == ownerUnit.TeamId;
        }

        DamageMessage CreateDamageMessage(Vector3 impactVelocity)
        {
            DamageMessage damageMessage = DamageMessageCreator.Create(ownerUnit.transform, transform, ownerUnit.TraitData.Damage);

            if (damageBasedOnImpactSpeed)
            {
                damageMessage.physicDamage = Mathf.Lerp(0, damageMessage.physicDamage, impactVelocity.magnitude / maxImpactSpeed);
            }

            return damageMessage;
        }
        
        protected virtual void SendDamageOnImpact(Collision other, IDamageable damageable)
        {
            DamageMessage damageMessage = CreateDamageMessage(other.relativeVelocity);
            damageable.TakeDamage(damageMessage);
            
            
            AddImpactForce(other.rigidbody, damageable);
        }
        
        protected virtual void AddImpactForce(Rigidbody otherRigidbody, IDamageable damageable)
        {
            if (addImpactForce < 0 || Mathf.Approximately(addImpactForce, 0))
            {
                return;
            }

            Vector3 force = (rigidbody.velocity.normalized + Vector3.up * addImpactUpForce).normalized * addImpactForce;
            
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


        public void TurnOnCanSendDamage()
        {
            canSendDamage = true;
        }

        public void TurnOffCanSendDamage()
        {
            canSendDamage = false;
        }
    }
}