using System.Collections;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class VikingHelmetAttack : AttackPerSecondsBehaviour_OneMeleeWeapon
    {
        protected override IEnumerator AttackProcess()
        {
            weapon.TurnOnCanSendDamage();
            
            var spineMuscle = RagdollAnimator.GetMuscleDataByName("spine");
            spineMuscle.muscleStrengthFactor = 0;
            
            RagdollAnimator.LostBalance();
            
            spineMuscle.Rigidbody.AddForce((UnitController.Forward).normalized * 40, ForceMode.VelocityChange);
            
            
            yield return new WaitForSeconds(0.5f);
            weapon.TurnOffCanSendDamage();
            
            yield return new WaitForSeconds(1.5f);
            spineMuscle.muscleStrengthFactor = 1;
            RagdollAnimator.KeepBalance();

            yield return new WaitForSeconds(0.5f);
        }
    }
}