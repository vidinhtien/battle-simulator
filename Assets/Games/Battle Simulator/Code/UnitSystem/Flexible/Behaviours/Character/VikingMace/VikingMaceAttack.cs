using System.Collections;
using DG.Tweening;
using RootMotion.FinalIK;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class VikingMaceAttack : AttackPerSecondsBehaviour_OneMeleeWeapon
    {
        [SerializeField] private AimIK aimIk;

        protected override IEnumerator AttackProcess()
        {
            weapon.TurnOnCanSendDamage();
            
            aimIk.solver.SetIKPositionWeight(1);
            weapon.Rigidbody.AddForce((UnitController.Forward - UnitController.Up).normalized * 40, ForceMode.VelocityChange);
            
            var spineMuscle = RagdollAnimator.GetMuscleDataByName("spine");
            spineMuscle.Rigidbody.AddForce((UnitController.Forward - UnitController.Up).normalized * 20, ForceMode.VelocityChange);
            
            RagdollAnimator.LostBalance();
            yield return new WaitForSeconds(0.35f);
            
            weapon.TurnOffCanSendDamage();
            DOVirtual.Float(1, 0, 0.5f, v => aimIk.solver.SetIKPositionWeight(v));
            yield return new WaitForSeconds(0.15f);
            
            RagdollAnimator.KeepBalance();
            yield return new WaitForSeconds(0.5f);
        }
    }
}