namespace BattleSimulatorV2.Flexible
{
    public class TriggerSetter_CheckEnemy_WhileAttack : FlexibleUnitBehaviour
    {
        public bool checkAttackRange = true;
        
        
        private AttackBehaviour attackBehaviour;
        

        
        
        public override void OnStateAwake()
        {
            base.OnStateAwake();

            attackBehaviour = State.GetBehaviour<AttackBehaviour>();
        }


        public override void OnStateUpdate()
        {
            if(attackBehaviour.IsInAttackProcess)
                return;
            
            
            var currentEnemy = UnitController.CurrentEnemy;
            if (currentEnemy == null)
            {
                SetTrigger("no_enemy");
                return;
            }

            if (checkAttackRange)
            {
                float sqrDistanceToEnemy = (currentEnemy.Position - UnitController.Position).sqrMagnitude;
                if (sqrDistanceToEnemy > UnitController.TraitData.AttackRange * UnitController.TraitData.AttackRange)
                {
                    SetTrigger("out_attack_range");
                }
            }
        }
    }
}