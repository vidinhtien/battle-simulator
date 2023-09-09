using System.Collections;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class CrusaderSwordAttack : AttackPerSecondsBehaviour_OneMeleeWeapon
    {
        protected override IEnumerator AttackProcess()
        {
            weapon.TurnOnCanSendDamage();
            
            var spineMuscle = RagdollAnimator.GetMuscleDataByName("spine");
            var rightHandMuscle = RagdollAnimator.GetMuscleDataByName("right_hand");
            var leftHandMuscle = RagdollAnimator.GetMuscleDataByName("left_hand");


            Vector3 direction = (UnitController.Forward + UnitController.Down).normalized;
            weapon.Rigidbody.AddForce(direction * 40, ForceMode.VelocityChange);
            spineMuscle.Rigidbody.AddForce(direction * 30, ForceMode.VelocityChange);
            rightHandMuscle.Rigidbody.AddForce(direction * 20, ForceMode.VelocityChange);
            leftHandMuscle.Rigidbody.AddForce(direction * 20, ForceMode.VelocityChange);

            
            RagdollAnimator.LostBalance();
            yield return new WaitForSeconds(0.5f);
            
            weapon.TurnOffCanSendDamage();
            RagdollAnimator.KeepBalance();
            yield return new WaitForSeconds(0.5f);
        }
    }
}