using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ZeroX.RagdollSystem.MonoEditors 
{
	[AddComponentMenu("ZeroX/Ragdoll System/Ragdoll Manager/Ragdoll Editor")]
	public class RagdollEditor : MonoBehaviour {
		
		[System.Serializable]
		public enum Mode {
			Colliders,
			Joints
		}

		[HideInInspector] public Rigidbody selectedRigidbody;
		[HideInInspector] public Collider selectedCollider;
		[HideInInspector] public bool symmetry = true;
		[HideInInspector] public Mode mode;
		
		
		//Rigidbody Config
		public List<Rigidbody> listRigidbody = new List<Rigidbody>();
		public RigidbodyConfig rigidbodyConfig = new RigidbodyConfig();
		
		//Configurable Joint Config
		public List<ConfigurableJoint> listConfigurableJoint = new List<ConfigurableJoint>();
		public ConfigurableJointConfig configurableJointConfig = new ConfigurableJointConfig();
		
		//Physic Material Config
		public PhysicMaterial feetPhysicMaterial;


		private void Reset()
		{
			listRigidbody = GetComponentsInChildren<Rigidbody>().ToList();
			listConfigurableJoint = GetComponentsInChildren<ConfigurableJoint>().ToList();
		}


		public void ApplyConfigurableJointConfig(ConfigurableJoint joint)
		{
			joint.xMotion = configurableJointConfig.xMotion;
			joint.yMotion = configurableJointConfig.yMotion;
			joint.zMotion = configurableJointConfig.zMotion;
			
			joint.angularXMotion = configurableJointConfig.angularXMotion;
			joint.angularYMotion = configurableJointConfig.angularYMotion;
			joint.angularZMotion = configurableJointConfig.angularZMotion;

			joint.rotationDriveMode = configurableJointConfig.rotationDriveMode;
			joint.angularXDrive = configurableJointConfig.angularXDrive.ExportToJointDrive();
			joint.angularYZDrive = configurableJointConfig.angularYZDrive.ExportToJointDrive();

			joint.enablePreprocessing = configurableJointConfig.enablePreprocessing;
		}
	}
}
