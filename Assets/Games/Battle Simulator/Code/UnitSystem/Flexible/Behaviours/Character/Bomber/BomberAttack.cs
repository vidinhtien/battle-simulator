using System.Collections;
using BattleSimulatorV2.BulletSystem;
using BattleSimulatorV2.DamageSystem;
using BattleSimulatorV2.Flexible;
using UnityEngine;

namespace BattleSimulatorV2.FlexibleV2
{
    public class BomberAttack : FlexibleUnitBehaviour
    {
        [Header("Bullet")] [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletLifeTime = 3;
        [SerializeField] private Transform bulletHolder;


        private Coroutine crFireTimeline;
        private Bullet currentBullet;
        private bool attackDone;

        public override void OnStateEnter()
        {
            if (!attackDone)
            {
                Fire();
                attackDone = true;
            }
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
            currentBullet.transform.localPosition = Vector3.zero;
        }

        private IEnumerator FireTimeline()
        {
            //Spawn Bullet
            if (currentBullet == null)
                PrepareBullet();
            //Fire Bullet
            yield return null;
            currentBullet.SetParent(null);

            currentBullet.damageMessage = DamageMessageCreator.Create(UnitController.transform, currentBullet.transform,
                UnitController.TraitData.Damage);
            currentBullet.Fire(Vector3.zero);
            currentBullet.DestroySelfDelay(bulletLifeTime);
            currentBullet = null;

            //End Fire
            crFireTimeline = null;
        }
    }
}