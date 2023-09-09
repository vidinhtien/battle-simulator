using UnityEngine;

namespace BattleSimulatorV2.DamageSystem
{
    [System.Serializable]
    public class DamageMessage
    {
        public Transform owner; //Là chủ thể gây ra damage, ví dụ như bản thân nhân vật
        public Transform directSender; //Là đối tượng trực tiếp gửi damage, ví dụ như nhân vật cầm súng bắn 1 viên đạn, viên đạn va chạm và gây damage, thì directSender là viên đạn
        
        public float physicDamage;


        public DamageMessage Clone()
        {
            DamageMessage damageMessage = new DamageMessage();
            
            damageMessage.owner = owner;
            damageMessage.directSender = directSender;
            damageMessage.physicDamage = physicDamage;

            return damageMessage;
        }
    }
}