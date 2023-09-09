using System;
using UnityEngine;

namespace ZeroX.RagdollSystem
{
    public class TorqueRagdollBalancer : RagdollBalancer
    {
        [SerializeField] private RagdollAnimator ragdollAnimator;

        [SerializeField] private bool useMuscleWeightFromRagdolAnimator = false;
        [SerializeField] private bool useMuscleWeightFactorFromRagdolAnimator = true;
        
        [SerializeField] private float selfMuscleWeight = 100;
        [SerializeField] public AnimationCurve upRightTorqueCurve;


        private bool isBalancing;

        public override bool IsBalancing => isBalancing;


        private void Reset()
        {
            ragdollAnimator = GetComponent<RagdollAnimator>();

            upRightTorqueCurve = new AnimationCurve();
            upRightTorqueCurve.AddKey(0, 0.5f);
            upRightTorqueCurve.AddKey(0.2f, 0.9f);
            upRightTorqueCurve.AddKey(0.5f, 1);

            var keys = upRightTorqueCurve.keys;
            keys[0].inTangent = 2.675275f;
            keys[0].outTangent = 2.675275f;
            
            keys[1].inTangent = 0.9401946f;
            keys[1].outTangent = 0.9401946f;
            
            keys[2].inTangent = 0.115281f;
            keys[2].inTangent = 0.115281f;
            
            upRightTorqueCurve.keys = keys;
        }

        public override void KeepBalance()
        {
            isBalancing = true;
        }

        public override void LostBalance()
        {
            isBalancing = false;
        }


        private void LateUpdate()
        {
            if(isBalancing == false)
                return;
            
            UpdateBalance();
        }

        void UpdateBalance()
        {
            float torqueForce = selfMuscleWeight;
            if (useMuscleWeightFromRagdolAnimator)
                torqueForce *= ragdollAnimator.MasterMuscleWeight;

            if (useMuscleWeightFactorFromRagdolAnimator)
                torqueForce *= ragdollAnimator.MasterMuscleWeightFactor;

            
            
            Rigidbody followBone = ragdollAnimator.RootFollowBone;
            Transform targetBone = ragdollAnimator.RootTargetBone;
            
            
            
            
            var balancePercent = Vector3.Angle(followBone.transform.up, targetBone.up) / 180;
        
            balancePercent = upRightTorqueCurve.Evaluate(balancePercent);
            var rot = Quaternion.FromToRotation(followBone.transform.up, targetBone.up).normalized;
        
            followBone.AddTorque(new Vector3(rot.x, rot.y, rot.z) * torqueForce * balancePercent);
        
            var directionAnglePercent = Vector3.SignedAngle(followBone.transform.forward, targetBone.forward, targetBone.up) / 180;
            //followBone.AddRelativeTorque(0, directionAnglePercent * torqueForce, 0);
        }
    }
}