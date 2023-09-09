using UnityEngine.Serialization;

namespace BattleSimulatorV2.UnitSystem
{
    [System.Serializable]
    public class UnitTraitData
    {
        public int Id;
        public string Name;
        
        public float HealthPoint = 15;
        public float Damage = 7;
        public float AttackPerSeconds = 3;
        public float AttackRange = 1.5f;
        public float SightRange = 100;
        public float MoveSpeed = 2;
        
        public int PriceCoin = 100;
        public int LevelStar;
    }
}