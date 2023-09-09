using System.Collections;
using BattleSimulatorV2.BulletSystem;
using BattleSimulatorV2.DamageSystem;
using DG.Tweening;
using UnityEngine;


namespace BattleSimulatorV2.Flexible
{
    public class BallistaAttack : AttackPerSecondsBehaviour
    {
        [SerializeField] private float pullDuration = 3;
        [SerializeField] private Transform pullHandle;
        [SerializeField] private Transform pullPointA;
        [SerializeField] private Transform pullPointB;
        
        [Header("Bullet")]
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletLifeTime = 15;
        [SerializeField] private Transform bulletHolder;
        [SerializeField] private bool ignoreOwnerCollider = true;
        
        
        private Bullet currentBullet;

        private Rigidbody rigidbody;
        

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            rigidbody = UnitController.GetComponent<Rigidbody>();
            
            if(currentBullet == null)
                PrepareBullet();
        }

        protected override IEnumerator AttackProcess()
        {
            //Setup
            pullHandle.localPosition = pullPointA.localPosition;
            if(rigidbody != null)
                rigidbody.velocity = Vector3.zero;
            
            
            //Spawn bullet
            if(currentBullet == null)
                PrepareBullet();
            
            
            
            //Kéo cung
            var pullTween = pullHandle.DOLocalMove(pullPointB.localPosition, pullDuration).SetEase(Ease.Linear);
            yield return pullTween.WaitForCompletion();
            
            var releasePullTween = pullHandle.DOLocalMove(pullPointA.localPosition, 0.15f).SetEase(Ease.OutBounce);
            yield return releasePullTween;
            
            
            //Fire Bullet
            currentBullet.SetParent(null);
            
            currentBullet.damageMessage = DamageMessageCreator.Create(UnitController.transform, currentBullet.transform, UnitController.TraitData.Damage);
            currentBullet.Fire(GetFireDirection());
            currentBullet.DestroySelfDelay(bulletLifeTime);
            currentBullet = null;
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

        Vector3 GetFireDirection()
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
    }
}