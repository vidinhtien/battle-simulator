using System.Collections;
using BattleSimulatorV2.BulletSystem;
using BattleSimulatorV2.DamageSystem;
using DG.Tweening;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class VikingSpearAttack : AttackPerSecondsBehaviour
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private GameObject spearModel;
        [SerializeField] private Transform bulletSpawnPoint;
        

        protected override IEnumerator AttackProcess()
        {
            var spineMuscle = RagdollAnimator.GetMuscleDataByName("spine");
            spineMuscle.Rigidbody.AddForce((UnitController.Forward - UnitController.Up).normalized * 25, ForceMode.VelocityChange);
            
            
            spearModel.transform.localScale = new Vector3(1, 1, 0);
            FireBullet();

            StartCoroutine(RestoreSpearModel());
            
            yield return new WaitForSeconds(0.5f);
        }


        IEnumerator RestoreSpearModel()
        {
            yield return new WaitForSeconds(AttackPerSeconds * 0.5f);
            spearModel.transform.DOScaleZ(1, 0.3f);
        }


        void FireBullet()
        {
            Bullet bullet = CreateBullet();

            Vector3 direction = (UnitController.CurrentEnemy.Position + UnitController.CurrentEnemy.Up) - bullet.Position;
            bullet.Fire(direction.normalized);
            bullet.DestroySelfDelay(10);
        }
        
        

        Bullet CreateBullet()
        {
            var currentBullet = Instantiate(bulletPrefab);
            currentBullet.transform.position = bulletSpawnPoint.position;
            currentBullet.transform.rotation = bulletSpawnPoint.rotation;
            IgnoreOwnerCollider(currentBullet);
            
            currentBullet.damageMessage = DamageMessageCreator.Create(UnitController.transform, currentBullet.transform, UnitController.TraitData.Damage);
            
            return currentBullet;
        }
        
        private void IgnoreOwnerCollider(Bullet bullet)
        {
            var listOwnerCollider = UnitController.GetComponentsInChildren<Collider>();
            var bulletColliders = bullet.GetComponentsInChildren<Collider>();

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