using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class VikingAxeAttack : AttackPerSecondsBehaviour_OneMeleeWeapon
    {
        [SerializeField] private float jumpForce = 20;

        protected override IEnumerator AttackProcess()
        {
            weapon.TurnOnCanSendDamage();
            
            Vector3 jumpDirection = (UnitController.Up * 2 + UnitController.Forward).normalized;
            RagdollAnimator.RootFollowBone.AddForce(jumpDirection * jumpForce, ForceMode.VelocityChange);
            
            var tween1 = RagdollAnimator.TargetPose.DOLocalRotate(new Vector3(360 + 90, 0, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal);
            yield return tween1.WaitForCompletion();
                
            var tween2 = RagdollAnimator.TargetPose.DOLocalRotate(new Vector3(0, 0, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal);
            yield return tween2.WaitForCompletion();
            
            weapon.TurnOffCanSendDamage();

            yield return new WaitForSeconds(1);
        }
    }
}