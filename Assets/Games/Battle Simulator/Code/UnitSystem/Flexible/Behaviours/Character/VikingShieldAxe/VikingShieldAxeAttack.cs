using System.Collections;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class VikingShieldAxeAttack : AttackPerSecondsBehaviour_TwoMeleeWeapon
    {
        protected override IEnumerator AttackProcess()
        {
            leftWeapon.TurnOnCanSendDamage();
            rightWeapon.TurnOnCanSendDamage();
            
            var rightHandMuscle = RagdollAnimator.GetMuscleDataByName("right_hand");
            var spineMuscle = RagdollAnimator.GetMuscleDataByName("spine");
            
            Vector3 direction = (UnitController.Forward - UnitController.Up).normalized;
            
            rightWeapon.Rigidbody.AddForce(direction * 30, ForceMode.VelocityChange);
            rightHandMuscle.Rigidbody.AddForce(direction * 30, ForceMode.VelocityChange);
            spineMuscle.Rigidbody.AddForce(UnitController.Forward * 30, ForceMode.VelocityChange);
            
            RagdollAnimator.LostBalance();
            yield return new WaitForSeconds(0.5f);
            
            leftWeapon.TurnOffCanSendDamage();
            rightWeapon.TurnOffCanSendDamage();
            RagdollAnimator.KeepBalance();
            yield return new WaitForSeconds(0.5f);
        }
    }
}