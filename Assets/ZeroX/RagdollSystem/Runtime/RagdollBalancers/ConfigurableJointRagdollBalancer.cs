using System;
using UnityEngine;


namespace ZeroX.RagdollSystem
{
    public class ConfigurableJointRagdollBalancer : RagdollBalancer
    {
        [SerializeField] private RagdollAnimator ragdollAnimator;
        [SerializeField] private string muscleName = "hip";
        
        [SerializeField] private bool useMuscleStrengthOfMuscleData = true;
        [SerializeField] private bool useMuscleStrengthOfRagdollAnimator = true;
        
        [SerializeField] private float muscleStrength = 1;
        [SerializeField] private float muscleStrengthFactor = 0.2f;
        [SerializeField] private AnimationCurve muscleStrengthCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private float maxOffsetAngle = 90;

        private bool isBalancing = false;
        public override bool IsBalancing => isBalancing;
        
        private MuscleData muscleData;
        private ConfigurableJoint configurableJoint;


        private void Start()
        {
            
        }

        private void Reset()
        {
            ragdollAnimator = GetComponent<RagdollAnimator>();
        }


        public override void KeepBalance()
        {
            if (muscleData == null)
            {
                muscleData = ragdollAnimator.GetMuscleDataByName(muscleName);
                InitConfigurableJoint();
            }
            
            isBalancing = true;
        }

        public override void LostBalance()
        {
            if (muscleData == null)
            {
                muscleData = ragdollAnimator.GetMuscleDataByName(muscleName);
                InitConfigurableJoint();
            }
            
            isBalancing = false;
            
            
            
            //Khi turn off thì set spring về 0
            if (muscleData != null)
            {
                //Cập nhật angularXDrive
                JointDrive angularXDrive = muscleData.Joint.angularXDrive;
                angularXDrive.positionSpring = 0;
                muscleData.Joint.angularXDrive = angularXDrive;
                
                //Cập nhật angularYZDrive
                JointDrive angularYZDrive = muscleData.Joint.angularYZDrive;
                angularYZDrive.positionSpring = 0;
                muscleData.Joint.angularYZDrive = angularYZDrive;
            }
        }

        public override bool OverrideMuscleControl(MuscleData muscleData)
        {
            if (this.muscleData != muscleData)
                return false;
            
            
            
            float finalMuscleWeight = CalculateFinalMuscleWeight(this.muscleData);
            
            if (isBalancing == false) //Ko running tức là sẽ làm cho mất cân bằng, chứ ko phải là ko override muscle control nữa, vì nếu ko override nữa thì ragdollAnimator sẽ lại khiến muscle hoạt động
                finalMuscleWeight = 0;
            
            ragdollAnimator.UpdateMuscle(this.muscleData, finalMuscleWeight);
            return true;
        }

        float CalculateFinalMuscleWeight(MuscleData muscleData)
        {
            float finalMuscleWeight = muscleStrength * muscleStrengthFactor;
            
            if (useMuscleStrengthOfMuscleData)
            {
                finalMuscleWeight *= muscleData.muscleStrength * muscleData.muscleStrengthFactor;
            }

            if (useMuscleStrengthOfRagdollAnimator)
            {
                finalMuscleWeight *= ragdollAnimator.MasterMuscleWeight * ragdollAnimator.MasterMuscleWeightFactor;
            }

            finalMuscleWeight = EvaluateMuscleStrength(finalMuscleWeight);
            
            return finalMuscleWeight;
        }

        float EvaluateMuscleStrength(float muscleStrengthValue)
        {
            float angle = Quaternion.Angle(muscleData.followBone.rotation, muscleData.targetBone.rotation);
            return muscleStrengthValue * muscleStrengthCurve.Evaluate(angle / maxOffsetAngle);
        }

        void InitConfigurableJoint()
        {
            configurableJoint = muscleData.Joint;
            configurableJoint.xMotion = ConfigurableJointMotion.Free;
            configurableJoint.yMotion = ConfigurableJointMotion.Free;
            configurableJoint.zMotion = ConfigurableJointMotion.Free;
        }
    }
}