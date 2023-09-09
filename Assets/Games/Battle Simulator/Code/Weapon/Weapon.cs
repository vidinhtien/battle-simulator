using UnityEngine;

namespace BattleSimulatorV2.WeaponSystem
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] protected Rigidbody rigidbody;

        public Rigidbody Rigidbody => rigidbody;
    }
}