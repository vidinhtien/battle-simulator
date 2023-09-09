using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class MoveForward_UseRigidbody : MoveForward_UseRigidbody_Base
    {
        [SerializeField] private Rigidbody rigidbodyToMove;

        protected override Rigidbody RigidbodyToMove => rigidbodyToMove;
    }
}