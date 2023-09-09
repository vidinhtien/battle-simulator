using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.DataTableSystem.Demo
{
    [System.Serializable]
    public class TestRow
    {
        public string id;
        public EnemyType type;
        public int powerLevel;
        public Range range;
        public List<string> listReward;
        [TextArea]
        public string des1;
        [TextArea]
        public string des2;
    }

    
    public enum EnemyType
    {
        Monster, Boss
    }
    
    [System.Serializable]
    public class Range
    {
        public float min;
        public float max;
    }
}