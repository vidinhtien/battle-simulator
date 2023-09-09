namespace BattleSimulatorV2.Flexible
{
    public class FreeFallRagdoll : FlexibleUnitBehaviour
    {
        public override void OnStateEnter()
        {
            RagdollAnimator.LostBalance();
            RagdollAnimator.MasterMuscleWeightFactor = 0;
        }
    }
}