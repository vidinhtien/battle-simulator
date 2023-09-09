using BattleSimulatorV2.WeaponSystem;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public abstract class AttackPerSecondsBehaviour_OneMeleeWeapon : AttackPerSecondsBehaviour
    {
        [SerializeField] protected MeleeWeapon weapon;

        public override void OnStateEnter()
        {
            weapon.TurnOffCanSendDamage();
            
            base.OnStateEnter();
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            
            weapon.TurnOffCanSendDamage();
        }
    }
}