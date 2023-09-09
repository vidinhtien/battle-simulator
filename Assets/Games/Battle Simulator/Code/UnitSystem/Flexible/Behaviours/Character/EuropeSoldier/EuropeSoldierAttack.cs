using System.Collections;
using BattleSimulatorV2.BulletSystem;
using BattleSimulatorV2.DamageSystem;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class EuropeSoldierAttack : AttackPerSecondsBehaviour
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private Rigidbody weaponRigidbody;
        

        protected override IEnumerator AttackProcess()
        {
            StopMove();

            Vector3 direction = -UnitController.Forward;
            
            weaponRigidbody.AddForce(Vector3.up * 10, ForceMode.VelocityChange);

            var spineMuscle = RagdollAnimator.GetMuscleDataByName("spine");
            spineMuscle.Rigidbody.AddForce(direction * 5, ForceMode.VelocityChange);
            
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

        void StopMove()
        {
            foreach (var muscleData in RagdollAnimator.ListMuscleData)
            {
                muscleData.Rigidbody.velocity = Vector3.zero;
                muscleData.Rigidbody.angularVelocity = Vector3.zero;
            }
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