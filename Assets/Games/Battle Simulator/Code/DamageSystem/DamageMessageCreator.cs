using UnityEngine;

namespace BattleSimulatorV2.DamageSystem
{
    public static class DamageMessageCreator
    {
        public static DamageMessage Create(Transform owner, Transform directSender, float physicDamage)
        {
            DamageMessage damageMessage = new DamageMessage();
            damageMessage.owner = owner;
            damageMessage.directSender = directSender;
            damageMessage.physicDamage = physicDamage;

            return damageMessage;
        }
        
        public static DamageMessage Create(Transform owner, float physicDamage)
        {
            DamageMessage damageMessage = new DamageMessage();
            damageMessage.owner = owner;
            damageMessage.physicDamage = physicDamage;

            return damageMessage;
        }
        
        public static DamageMessage Create(float physicDamage)
        {
            DamageMessage damageMessage = new DamageMessage();
            damageMessage.physicDamage = physicDamage;

            return damageMessage;
        }
    }
}