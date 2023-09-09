using ZeroX.FsmSystem;
using ZeroX.RagdollSystem;

namespace BattleSimulatorV2.Flexible
{
    public class FlexibleUnitBehaviour : MonoStateBehaviour
    {
        public FlexibleUnitController UnitController { get; private set; }
        public RagdollAnimator RagdollAnimator => UnitController.RagdollAnimator;

        public override void OnStateAwake()
        {
            UnitController = GetComponentInParent<FlexibleUnitController>();
        }
    }
}