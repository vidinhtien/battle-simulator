using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZeroX.RagdollSystem
{
    [System.Serializable]
    public class MuscleData
    {
        public string name;
        public Transform targetBone;
        public Transform followBone;


        
        [Tooltip("Nên được coi như giá trị weight ít thay đổi. Giá trị này tương quan cho tỉ lệ weight giữa các bone")]
        [Min(0)]
        public float muscleStrength = 1;
        
        [Tooltip("Có thể được sử dụng để tăng giảm mức độ weight trong các state")]
        [Min(0)]
        public float muscleStrengthFactor = 1;


        
        public float massFactor = 1;
        public float gravityFactor = 1;
        



        [NonSerialized] public Rigidbody Rigidbody;
        [NonSerialized] public ConfigurableJoint Joint;
        [NonSerialized] public Quaternion InitJointLocalRotation;
        [NonSerialized] public Quaternion InitJointRotation;
        [NonSerialized] public float Mass = 1;


        public void Init()
        {
            Rigidbody = followBone.GetComponent<Rigidbody>();
            Joint = followBone.GetComponent<ConfigurableJoint>();
            InitJointLocalRotation = followBone.localRotation;
            InitJointRotation = followBone.rotation;

            Mass = Rigidbody.mass;
        }
    }
}