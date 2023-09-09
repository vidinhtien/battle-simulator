using UnityEngine;
using ZeroX.FsmSystem;

namespace BattleSimulatorV2.StateBehaviours
{
    public class DestroyDelay : MonoStateBehaviour
    {
        public float delayTime = 5;
        public bool ignoreTimeScale = false;
        public GameObject objectWantDestroy;

        
        
        private float timeEnter;
        private float unscaledTimeEnter;

        
        
        
        
        public override void OnStateEnter()
        {
            timeEnter = Time.time;
            unscaledTimeEnter = Time.unscaledTime;
        }

        public override void OnStateUpdate()
        {
            if (ignoreTimeScale)
            {
                if(Time.unscaledTime - unscaledTimeEnter > delayTime)
                    Destroy(objectWantDestroy);
            }
            else
            {
                if(Time.time - timeEnter > delayTime)
                    Destroy(objectWantDestroy);
            }
        }
    }
}