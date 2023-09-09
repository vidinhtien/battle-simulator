using System.Collections;
using BattleSimulatorV2.WeaponSystem;
using DG.Tweening;
using UnityEngine;
using ZeroX.RagdollSystem;

namespace BattleSimulatorV2.Flexible
{
    public class EuropeBoxingAttack : AttackPerSecondsBehaviour_TwoMeleeWeapon
    {
        private bool nextIsRightHand = true;
        
        

        protected override IEnumerator AttackProcess()
        {
            if (nextIsRightHand)
            {
                nextIsRightHand = false;
                var upperArmMuscle = RagdollAnimator.GetMuscleDataByName("right_upper_arm");
                var lowerArmMuscle = RagdollAnimator.GetMuscleDataByName("right_lower_arm");
                var handMuscle = RagdollAnimator.GetMuscleDataByName("right_hand");
                yield return StartCoroutine(Punch(upperArmMuscle, lowerArmMuscle, handMuscle, rightWeapon));
            }
            else
            {
                nextIsRightHand = true;
                var upperArmMuscle = RagdollAnimator.GetMuscleDataByName("left_upper_arm");
                var lowerArmMuscle = RagdollAnimator.GetMuscleDataByName("left_lower_arm");
                var handMuscle = RagdollAnimator.GetMuscleDataByName("left_hand");
                yield return StartCoroutine(Punch(upperArmMuscle, lowerArmMuscle, handMuscle, leftWeapon));
            }
            
            //yield return new WaitForSeconds(0.5f);
        }


        IEnumerator Punch(MuscleData upperArmMuscle, MuscleData lowerArmMuscle, MuscleData handMuscle, MeleeWeapon punch)
        {
            //Tắt muscle của lower Arm
            lowerArmMuscle.muscleStrengthFactor = 0;
            upperArmMuscle.muscleStrengthFactor = 0;
            
            //Bật collider
            punch.TurnOnCanSendDamage();

            //Add force để đấm
            //Vector3 direction = (UnitController.CurrentEnemy.Position + UnitController.CurrentEnemy.Up * 2) - handMuscle.Rigidbody.position;
            Vector3 direction = (UnitController.Up + UnitController.Forward * 2).normalized;
            punch.Rigidbody.AddForce(direction.normalized * 50, ForceMode.VelocityChange);
            handMuscle.Rigidbody.AddForce(direction.normalized * 40, ForceMode.VelocityChange);
            lowerArmMuscle.Rigidbody.AddForce(direction.normalized * 30, ForceMode.VelocityChange);
            //upperArmMuscle.Rigidbody.AddForce(direction.normalized * 20, ForceMode.VelocityChange);
            

            yield return new WaitForSeconds(0.25f);
            punch.TurnOffCanSendDamage();
            var tweener = DOVirtual.Float(0, 1, 0.25f, v =>
            {
                lowerArmMuscle.muscleStrengthFactor = v;
                upperArmMuscle.muscleStrengthFactor = v;
            });

            yield return tweener.WaitForCompletion();
        }
    }
}