using System;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class AnimateMoving : FlexibleUnitBehaviour
    {
        [SerializeField] private Rigidbody rigidbodyToMove;
        [SerializeField] private Animator animator;
        [SerializeField] private string animationName = "Run";
        [SerializeField] private float crossfadeTime = .2f;


        private float maxMoveSpeed;
        public override void OnStateEnter()
        {
            if (animator != null)
            {
                animator.CrossFade(animationName, crossfadeTime);
            }
            maxMoveSpeed = UnitController.TraitData.MoveSpeed;
            maxMoveSpeed *= maxMoveSpeed;
        }

        public override void OnStateFixedUpdate()
        {
            var crrMoveSpeed = rigidbodyToMove.velocity.sqrMagnitude;
            var speedRun = crrMoveSpeed / maxMoveSpeed;
            if (animator != null)
            {
                animator.SetFloat("Speed", speedRun);
            }
        }
    }
}