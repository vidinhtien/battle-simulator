using BattleSimulatorV2.UnitSystem;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class FindNearestEnemy : FlexibleUnitBehaviour
    {
        private float lastTimeFind = -999;
        
        public override void OnStateEnter()
        {
            lastTimeFind = -999f;
        }

        public override void OnStateUpdate()
        {
            if (Time.time - lastTimeFind > 0.5f)
            {
                lastTimeFind = Time.time;
                UnitController.CurrentEnemy = Find();
            
                if (UnitController.CurrentEnemy != null)
                {
                     SetTrigger("has_enemy");
                }
            }
        }

        UnitController Find()
        {
            var listEnemy = UnitManager.GetUnitsAlive_NotSame_TeamId(UnitController.TeamId);

            float minSqrDistance = Mathf.Infinity;
            UnitController enemy = null;

            foreach (var unit in listEnemy)
            {
                float sqrDistance = (unit.transform.position - UnitController.transform.position).sqrMagnitude;
                if(sqrDistance >= minSqrDistance)
                    continue;

                minSqrDistance = sqrDistance;
                enemy = unit;
            }

            return enemy;
        }
    }
}