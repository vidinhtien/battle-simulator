using UnityEngine;

namespace BattleSimulatorV2.FlexibleV2
{
    public abstract class AbstractEffect : MonoBehaviour
    {
        public bool isUsing = false;
    
        public abstract bool IsUsing();
        
        public abstract void Active();

        public void DeActive()
        {
            isUsing = false;
            gameObject.SetActive(false);
        }
    }
}