namespace BattleSimulatorV2.Flexible
{
    public abstract class AttackBehaviour : FlexibleUnitBehaviour, IHaveProcess
    {
        public abstract bool IsInAttackProcess { get; }
        public bool IsInProcess => IsInAttackProcess;
    }
}