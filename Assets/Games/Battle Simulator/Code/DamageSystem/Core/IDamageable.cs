using UnityEngine;

namespace BattleSimulatorV2.DamageSystem
{
    public interface IDamageable
    {
        public GameObject GetDamageableObject();
        
        public DamageResult TakeDamage(DamageMessage damageMessage);
    }
}