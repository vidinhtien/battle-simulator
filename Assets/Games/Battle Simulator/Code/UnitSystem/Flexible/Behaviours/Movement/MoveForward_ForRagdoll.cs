using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class MoveForward_ForRagdoll : MoveForward_UseRigidbody_Base
    {
        protected override Rigidbody RigidbodyToMove => RagdollAnimator.RootFollowBone;
    }
}