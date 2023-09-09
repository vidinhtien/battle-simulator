using System.Collections;
using BattleSimulatorV2.BulletSystem;
using BattleSimulatorV2.DamageSystem;
using BattleSimulatorV2.Flexible;
using DG.Tweening;
using UnityEngine;

namespace BattleSimulatorV2.FlexibleV2
{
    public class CanonAttack : FlexibleUnitBehaviour
    {
        public bool IsInFiringProcess => crFireTimeline != null;

        [SerializeField] private float firePerSeconds = 4;
        [SerializeField] private Rigidbody parentRigidbody;
        [SerializeField] private float blowBackForce;
        [SerializeField] private float delayFire = 2.5f;

        [Header("Bullet")] [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletLifeTime = 15;
        [SerializeField] private Transform bulletHolder;
        [SerializeField] private bool ignoreOwnerCollider = true;

        [Header("Efx")] [SerializeField] private AbstractEffect efx_Fire;


        private Coroutine crFireTimeline;
        private Bullet currentBullet;
        private float lastTimeEndFire = -999;


        public override void OnStateEnter()
        {
            firePerSeconds = UnitController.TraitData.AttackPerSeconds;
            lastTimeEndFire = Time.time;

            //if (currentBullet == null)
                //PrepareBullet();
        }

        public override void OnStateUpdate()
        {
            switch (IsInFiringProcess)
            {
                case false when UnitController.CurrentEnemy == null:
                    SetTrigger("no_enemy");
                    return;
                case false when Time.time - lastTimeEndFire > firePerSeconds:
                    Fire();
                    break;
            }
        }

        public override void OnStateExit()
        {
            StopFire();
        }

        private void Fire()
        {
            StopFire();
            crFireTimeline = StartCoroutine(FireTimeline());
        }

        protected void StopFire()
        {
            if (crFireTimeline == null) return;
            StopCoroutine(crFireTimeline);
            crFireTimeline = null;
        }

        private void PrepareBullet()
        {
            if (currentBullet != null)
            {
                currentBullet.DestroySelf();
                currentBullet = null;
            }

            currentBullet = Instantiate(bulletPrefab, bulletHolder);
            currentBullet.IsKinematic = true;
            currentBullet.transform.localPosition = Vector3.zero;

            if (ignoreOwnerCollider)
                IgnoreOwnerCollider();
        }


        private IEnumerator FireTimeline()
        {
            //Setup
            FireEffect(false);
            yield return new WaitForSeconds(delayFire);
            //Effect
            
            FireEffect(true);
            //Spawn bullet
            if (currentBullet == null)
                PrepareBullet();
            yield return null;
            //Fire Bullet
            currentBullet.SetParent(null);
            yield return null;
            
            currentBullet.damageMessage = DamageMessageCreator.Create(UnitController.transform, currentBullet.transform,
                UnitController.TraitData.Damage);
            var direction = GetFireDirection();
            currentBullet.Fire(direction);
            currentBullet.DestroySelfDelay(bulletLifeTime);
            currentBullet = null;
            
            // Blow back parent 
            parentRigidbody.AddForce(-1* blowBackForce * direction, ForceMode.Impulse);
            //End Fire
            lastTimeEndFire = Time.time;
            crFireTimeline = null;
        }

        private Vector3 GetFireDirection()
        {
            if (UnitController.CurrentEnemy == null)
            {
                return currentBullet.transform.forward;
            }

            var enemyRigidbody = UnitController.CurrentEnemy.GetComponentInChildren<Rigidbody>();
            return (enemyRigidbody.worldCenterOfMass + Vector3.up) - currentBullet.transform.position;
        }

        private void IgnoreOwnerCollider()
        {
            var listOwnerCollider = UnitController.GetComponentsInChildren<Collider>();
            var bulletColliders = currentBullet.GetComponentsInChildren<Collider>();

            foreach (var bulletCollider in bulletColliders)
            {
                foreach (var ownerCollider in listOwnerCollider)
                {
                    Physics.IgnoreCollision(bulletCollider, ownerCollider);
                }
            }
        }

        private void FireEffect(bool state)
        {
            if (efx_Fire == null) return;
            if (state)
            {
                efx_Fire.Active();
            }
            else
            {
                efx_Fire.DeActive();
            }
        }
    }
}