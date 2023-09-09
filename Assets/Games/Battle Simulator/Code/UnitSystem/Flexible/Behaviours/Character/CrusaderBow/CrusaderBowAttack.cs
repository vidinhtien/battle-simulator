using System.Collections;
using BattleSimulatorV2.BulletSystem;
using BattleSimulatorV2.DamageSystem;
using DG.Tweening;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class CrusaderBowAttack : AttackPerSecondsBehaviour
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;
        

        protected override IEnumerator AttackProcess()
        {
            FireBullet();
            
            
            yield return new WaitForSeconds(0.5f);
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