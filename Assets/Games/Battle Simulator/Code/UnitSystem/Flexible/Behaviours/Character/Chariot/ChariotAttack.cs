using BattleSimulatorV2.Flexible;

namespace BattleSimulatorV2.FlexibleV2
{
    public class ChariotAttack : AttackBehaviour
    {
        private bool inAtkProgress;
        public override bool IsInAttackProcess => inAtkProgress;

        
    }
}