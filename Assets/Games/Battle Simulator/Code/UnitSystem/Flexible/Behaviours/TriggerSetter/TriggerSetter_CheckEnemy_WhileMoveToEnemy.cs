namespace BattleSimulatorV2.Flexible
{
    public class TriggerSetter_CheckEnemy_WhileMoveToEnemy : FlexibleUnitBehaviour
    {
        public override void OnStateUpdate()
        {
            var currentEnemy = UnitController.CurrentEnemy;
            if (currentEnemy == null)
            {
                SetTrigger("no_enemy");
                return;
            }

            float sqrDistanceToEnemy = (currentEnemy.Position - UnitController.Position).sqrMagnitude;
            if (sqrDistanceToEnemy < UnitController.TraitData.AttackRange * UnitController.TraitData.AttackRange)
            {
                SetTrigger("in_attack_range");
                return;
            }
        }
    }
}