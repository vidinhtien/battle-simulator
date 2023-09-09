namespace BattleSimulatorV2.DamageSystem
{
    public class DamageResult
    {
        public DamageMessage damageMessage;
        
        public float physicDamageTaken;

        public float HpLose => physicDamageTaken;
    }
}