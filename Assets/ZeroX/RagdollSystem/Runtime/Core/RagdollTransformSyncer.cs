using UnityEngine;


namespace ZeroX.RagdollSystem
{
    public abstract class RagdollTransformSyncer : MonoBehaviour
    {
        public abstract bool IsSyncing {get;}
        
        public abstract void KeepSync();
        public abstract void LostSync();
    }
}