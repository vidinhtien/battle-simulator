using System;
using DG.Tweening;
using UnityEngine;

namespace ZeroX.RagdollSystem.Demo
{
    public class TestCharacter : MonoBehaviour
    {
        [SerializeField] private RagdollAnimator ragdollAnimator;
        [SerializeField] private float jumpForce = 5;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Attack");
                Vector3 jumpDirection = (transform.up * 2 + transform.forward).normalized;
                ragdollAnimator.RootFollowBone.AddForce(jumpDirection * jumpForce, ForceMode.VelocityChange);

                Sequence sequence = DOTween.Sequence();
                var tween1 = ragdollAnimator.TargetPose.DORotate(new Vector3(360 + 90, 0, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal);
                sequence.Append(tween1);
                
                var tween2 = ragdollAnimator.TargetPose.DORotate(new Vector3(0, 0, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal);
                sequence.Append(tween2);
                //ragdollAnimator.RootFollowBone.DORotate(new Vector3(360 + 90, 0, 0), 2f, RotateMode.WorldAxisAdd).SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed);
            }
        }
    }
}