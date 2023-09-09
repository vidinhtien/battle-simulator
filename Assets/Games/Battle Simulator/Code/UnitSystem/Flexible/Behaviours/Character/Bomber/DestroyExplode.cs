using BattleSimulatorV2.BulletSystem;
using UnityEngine;
using ZeroX.FsmSystem;

namespace BattleSimulatorV2.StateBehaviours
{
    public class DestroyExplode : MonoStateBehaviour
    {
        [SerializeField] private Bullet bullet;

        public override void OnStateEnter()
        {
            bullet.gameObject.SetActive(true);
            bullet.Fire(Vector3.zero);
            bullet.DestroySelfDelay(3f);
        }
    }
}