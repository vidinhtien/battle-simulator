using UnityEngine;

namespace ZeroX.RagdollSystem
{
    public class KinematicRagdollBalancer : RagdollBalancer
    {
        [SerializeField] private RagdollAnimator ragdollAnimator;

        public override bool IsBalancing => rigidbody.isKinematic;
        
        private Rigidbody rigidbody;


        private void Awake()
        {
            rigidbody = ragdollAnimator.RootFollowBone;
        }

        private void Reset()
        {
            ragdollAnimator = GetComponent<RagdollAnimator>();
        }


        

        public override void KeepBalance()
        {
            rigidbody.isKinematic = true;
        }

        public override void LostBalance()
        {
            rigidbody.isKinematic = false;
        }
    }
}