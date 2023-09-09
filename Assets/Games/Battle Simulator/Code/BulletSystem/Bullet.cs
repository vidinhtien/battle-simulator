using BattleSimulatorV2.DamageSystem;
using UnityEngine;

namespace BattleSimulatorV2.BulletSystem
{
    public abstract class Bullet : MonoBehaviour
    {
        public float muzzleSpeed = 30;
        [SerializeField] protected Rigidbody rigidbody;
        public DamageMessage damageMessage;


        public abstract void Fire(Vector3 direction);



        #region Transform

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public Vector3 Position => transform.position;

        #endregion

        public bool IsKinematic
        {
            get => rigidbody.isKinematic;
            set => rigidbody.isKinematic = value;
        }


        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public void DestroySelfDelay(float delayTime)
        {
            Destroy(gameObject, delayTime);
        }
    }
}