using System.Collections;
using BattleSimulatorV2.BulletSystem;
using BattleSimulatorV2.DamageSystem;
using BattleSimulatorV2.Flexible;
using DG.Tweening;
using UnityEngine;

namespace BattleSimulatorV2.FlexibleV2
{
    public class CatapultAttack : AttackBehaviour
    {
        public bool IsInFiringProcess => crFireTimeline != null;
        
        [SerializeField] private float firePerSeconds = 4;
        [SerializeField] private float pullDuration = 3;
        [SerializeField] private Transform lever;
        [SerializeField] private Vector3 startRotation;
        [SerializeField] private Vector3 endRotation;
        
        [Header("Bullet")]
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletLifeTime = 15;
        [SerializeField] private Transform bulletHolder;
        [SerializeField] private bool ignoreOwnerCollider = true;
        
        private Coroutine crFireTimeline;
        private Bullet currentBullet;
        private float lastTimeEndFire = -999;



        public override void OnStateEnter()
        {
            firePerSeconds = UnitController.TraitData.AttackPerSeconds;
            lastTimeEndFire = Time.time;
            
            if(currentBullet == null)
                PrepareBullet();
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
            
            if(ignoreOwnerCollider)
                IgnoreOwnerCollider();
        }


        private IEnumerator FireTimeline()
        {
            //Setup
            
            
            //Spawn bullet
            if(currentBullet == null)
                PrepareBullet();
            
            
            
            //Kéo cung
            var pullTween = lever.DOLocalRotate(endRotation, pullDuration).SetEase(Ease.Linear);
            yield return pullTween.WaitForCompletion();
            
            var releasePullTween = lever.DOLocalRotate(startRotation, 0.2f).SetEase(Ease.OutBounce);
            yield return releasePullTween;
            
            //Fire Bullet
            yield return new WaitForSeconds(.1f);
            currentBullet.SetParent(null);
            
            currentBullet.damageMessage = DamageMessageCreator.Create(UnitController.transform, currentBullet.transform, UnitController.TraitData.Damage);
            currentBullet.Fire(GetFireDirection());
            currentBullet.DestroySelfDelay(bulletLifeTime);
            currentBullet = null;
            
            
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

        public override bool IsInAttackProcess { get; }
    }
}
