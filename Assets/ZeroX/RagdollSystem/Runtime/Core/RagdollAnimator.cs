using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


namespace ZeroX.RagdollSystem
{
    public partial class RagdollAnimator : MonoBehaviour
    {
        [Header("Pose")]
        [SerializeField] private Transform targetPose;
        [SerializeField] private Transform followPose;
        
        [Header("Root Bone")][Header("    ")]
        [SerializeField] private Transform rootTargetBone;
        [SerializeField] private Rigidbody rootFollowBone;
        
        [Header("Muscle")][Header("    ")]
        [Min(0)]
        [SerializeField] private float masterMuscleStrength = 1500;
        
        [Min(0)]
        [SerializeField] private float masterMuscleStrengthFactor = 1;
        
        [Header("    ")]
        [SerializeField] private UpdateMode updateMuscleMode = UpdateMode.LateUpdate;
        [SerializeField] private List<MuscleData> listMuscleData = new List<MuscleData>();

        
        [Header("Mass & Gravity")][Header("    ")]
        [SerializeField] private float masterMassFactor = 1;
        [SerializeField] private float masterGravityFactor = 1;


        [Header("Plugins")][Header("    ")]
        [SerializeField] private RagdollBalancer balancer;
        [SerializeField] private bool keepBalanceOnStart = true;
        
        [Header("    ")]
        [SerializeField] private RagdollTransformSyncer transformSyncer;
        [SerializeField] private bool keepSyncTransformOnStart = true;


        
        //FieldName
        public static string fn_rootTargetBone = "rootTargetBone";
        public static string fn_rootFollowBone = "rootFollowBone";
        public static string fn_listMuscleData = "listMuscleData";
        

        
        //Properties
        public Transform TargetPose => targetPose;
        public Transform FollowPose => followPose;
        
        public Transform RootTargetBone => rootTargetBone;
        public Rigidbody RootFollowBone => rootFollowBone;
        
        
        public float MasterMuscleWeight
        {
            get => masterMuscleStrength;
            set => masterMuscleStrength = value;
        }

        public float MasterMuscleWeightFactor
        {
            get => masterMuscleStrengthFactor;
            set => masterMuscleStrengthFactor = value;
        }

        public UpdateMode UpdateMuscleMode
        {
            get => updateMuscleMode;
            set => updateMuscleMode = value;
        }

        public List<MuscleData> ListMuscleData => listMuscleData;
        public RagdollBalancer Balancer => balancer;
        public RagdollTransformSyncer TransformSyncer => transformSyncer;




        #region Unity Method

        private void Awake()
        {
            
        }

        private void Start()
        {
            InitListMuscleData();
            
            if(balancer != null && keepBalanceOnStart)
                Balancer.KeepBalance();
            
            if(transformSyncer != null && keepSyncTransformOnStart)
                transformSyncer.KeepSync();
        }

        private void Update()
        {
            if(updateMuscleMode == UpdateMode.Update)
                UpdateAllMuscle();
        }
        
        private void LateUpdate()
        {
            if(updateMuscleMode == UpdateMode.LateUpdate)
                UpdateAllMuscle();
        }
        
        private void FixedUpdate()
        {
            if(updateMuscleMode == UpdateMode.FixedUpdate)
                UpdateAllMuscle();
            
            UpdateMass();
            UpdateGravity();
        }

        #endregion


        
        
        private void InitListMuscleData()
        {
            foreach (var muscleData in listMuscleData)
            {
                muscleData.Init();
            }
        }

        public MuscleData GetMuscleDataByName(string muscleName)
        {
            return listMuscleData.FirstOrDefault(d => d.name == muscleName);
        }

        public Transform GetFollowBoneByMuscleName(string muscleName)
        {
            var muscleData = listMuscleData.FirstOrDefault(d => d.name == muscleName);
            if (muscleData == null)
                return null;

            return muscleData.followBone;
        }


        public void UpdateAllMuscle()
        {
            for (int i = 0; i < listMuscleData.Count; i++)
            {
                var muscleData = listMuscleData[i];
                
                if (balancer != null && balancer.enabled)
                {
                    if(balancer.OverrideMuscleControl(muscleData)) //Nếu balancer ghi đè kiểm soát cơ bắp thì continue
                        continue;
                }
                
                
                float finalMuscleWeight = muscleData.muscleStrength * muscleData.muscleStrengthFactor * masterMuscleStrength * masterMuscleStrengthFactor;
                UpdateMuscle(muscleData, finalMuscleWeight);
            }
        }

        public void UpdateMuscle(MuscleData muscleData, float weight)
        {
            //Cập nhật angularXDrive
            JointDrive angularXDrive = muscleData.Joint.angularXDrive;
            angularXDrive.positionSpring = weight;
            muscleData.Joint.angularXDrive = angularXDrive;
                
            //Cập nhật angularYZDrive
            JointDrive angularYZDrive = muscleData.Joint.angularYZDrive;
            angularYZDrive.positionSpring = weight;
            muscleData.Joint.angularYZDrive = angularYZDrive;
                
            //Cập nhật TargetRotation
            if (muscleData.Joint.configuredInWorldSpace)
            {
                ConfigurableJointUtility.SetTargetRotation(muscleData.Joint, muscleData.targetBone.rotation, muscleData.InitJointRotation);
            }
            else
            {
                ConfigurableJointUtility.SetTargetRotationLocal(muscleData.Joint, muscleData.targetBone.localRotation, muscleData.InitJointLocalRotation);
            }
        }


        private void UpdateMass()
        {
            foreach (var muscleData in listMuscleData)
            {
                if (balancer != null && balancer.enabled)
                {
                    if(balancer.OverrideMassControl(muscleData))
                        continue;
                }
                
                
                float newMass = muscleData.Mass * muscleData.massFactor * masterMassFactor;
                if(Mathf.Approximately(newMass, muscleData.Rigidbody.mass))
                    continue;
                
                muscleData.Rigidbody.mass = newMass;
            }
        }

        private void UpdateGravity()
        {
            foreach (var muscleData in listMuscleData)
            {
                if (balancer != null && balancer.enabled)
                {
                    if(balancer.OverrideGravityControl(muscleData))
                        continue;
                }

                float finalGravityFactor = muscleData.gravityFactor * masterGravityFactor;
                if(Mathf.Approximately(finalGravityFactor, 1))
                    continue;
                
                Vector3 force = (finalGravityFactor - 1) * Physics.gravity;
                muscleData.Rigidbody.AddForce(force, ForceMode.Acceleration);
            }
        }


        #region Balancer
        
        public bool IsBalancing => balancer == null ? false : balancer.IsBalancing;
        
        public void SetBalancer(RagdollBalancer newBalancer, bool alsoKeepBalance)
        {
            if(balancer != null)
                balancer.LostBalance();
            
            balancer = newBalancer;
            
            if(alsoKeepBalance)
                balancer.KeepBalance();
        }
        
        public void KeepBalance()
        {
            if(balancer != null)
                balancer.KeepBalance();
        }
        
        public void LostBalance()
        {
            if(balancer != null)
                balancer.LostBalance();
        }
        
        #endregion




        
        #region Transform Syncer
        
        public bool IsSyncingTransform => transformSyncer == null ? false : transformSyncer.IsSyncing;
        
        public void SetTransformSyncer(RagdollTransformSyncer newTransformSyncer, bool alsoTurnOn)
        {
            if(transformSyncer != null)
                transformSyncer.LostSync();
            
            transformSyncer = newTransformSyncer;
            
            if(alsoTurnOn)
                transformSyncer.KeepSync();
        }
        
        public void KeepSyncTransform()
        {
            if(transformSyncer != null)
                transformSyncer.KeepSync();
        }
        
        public void LostSyncTransform()
        {
            if(transformSyncer != null)
                transformSyncer.LostSync();
        }
        
        #endregion
    }
}