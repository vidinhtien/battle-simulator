using System.Collections.Generic;
using BattleSimulatorV2.DamageSystem;
using BattleSimulatorV2.UnitSystem;
using UnityEngine;

namespace BattleSimulatorV2.BulletSystem
{
    public class ExplosionBomb : Bullet
    {
        public List<Collider> listCollider = new List<Collider>();

        [Header("Bom chỉ nổ một lần duy nhất tại vị trí sau đó sẽ biến mất")] [Tooltip("Bán kính nổ của bom")] [Min(0)]
        public float explosionRadius = 1;

        [Tooltip("Giá trị damage có nên lerp theo: khoảng cách đến tâm nổ/bán kính nổ")]
        public bool damageBasedOnImpactSpeed = false;

        public bool damageToTeamMates = true;

        [Header("Add Impact Force")] [Tooltip("Khi va chạm sẽ AddForce vào đối tượng va chạm")] [Min(0)]
        public float addImpactForce = 0;

        public bool addImpactForceToFullBody = false;
        public ForceMode addImpactForceMode = ForceMode.Impulse;
        
        [Header("Effect Explosion")] public GameObject efx_GO;
        
        
        private List<IDamageable> damagedObjects;

        public override void Fire(Vector3 direction)
        {
            damagedObjects = new List<IDamageable>();
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var col in colliders)
            {
                Explode(col);
            }
            efx_GO.SetActive(true);
        }


        public void Explode(Collider other)
        {
            //Check damageable
            var damageable = other.gameObject.GetComponentInParent<IDamageable>();
            if (damageable == null)
                return;
            //Check if object was exploded
            if (damagedObjects.Contains(damageable)) return;
            damagedObjects.Add(damageable);
            //Check teammates
            if (damageToTeamMates == false && IsTeamMate(other.gameObject))
            {
                return;
            }

            SendDamageOnImpact(other, damageable);
        }

        protected virtual void SendDamageOnImpact(Collider other, IDamageable damageable)
        {
            if (damageBasedOnImpactSpeed)
                ReCalculateDamageMessage(other.transform.position);

            damageable.TakeDamage(damageMessage);

            AddImpactForce(other.attachedRigidbody, damageable);
        }

        protected virtual void ReCalculateDamageMessage(Vector3 targetPosition)
        {
            var distance = Vector3.Distance(targetPosition, transform.position);
            damageMessage.physicDamage = Mathf.Lerp(0, damageMessage.physicDamage, distance / muzzleSpeed);
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

            var direct = otherRigidbody.position - transform.position;

            var force = direct.normalized * addImpactForce;

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
    }
}