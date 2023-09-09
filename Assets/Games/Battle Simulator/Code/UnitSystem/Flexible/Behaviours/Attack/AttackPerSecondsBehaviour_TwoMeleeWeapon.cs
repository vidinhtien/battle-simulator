using BattleSimulatorV2.WeaponSystem;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public abstract class AttackPerSecondsBehaviour_TwoMeleeWeapon : AttackPerSecondsBehaviour
    {
        [SerializeField] protected MeleeWeapon leftWeapon;
        [SerializeField] protected MeleeWeapon rightWeapon;

        public override void OnStateEnter()
        {
            leftWeapon.TurnOffCanSendDamage();
            rightWeapon.TurnOffCanSendDamage();
            
            base.OnStateEnter();
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            
            leftWeapon.TurnOffCanSendDamage();
            rightWeapon.TurnOffCanSendDamage();
        }
    }
}