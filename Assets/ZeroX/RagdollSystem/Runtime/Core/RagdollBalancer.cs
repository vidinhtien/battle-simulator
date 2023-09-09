using UnityEngine;

namespace ZeroX.RagdollSystem
{
    public abstract class RagdollBalancer : MonoBehaviour
    {
        public abstract bool IsBalancing {get;}
        
        public abstract void KeepBalance();
        public abstract void LostBalance();

        //Nếu ko ghi đè kiểm soát cơ bắp thì trả về false
        public virtual bool OverrideMuscleControl(MuscleData muscleData)
        {
            return false;
        }

        public virtual bool OverrideMassControl(MuscleData muscleData)
        {
            return false;
        }
        
        public virtual bool OverrideGravityControl(MuscleData muscleData)
        {
            return false;
        }
    }
}