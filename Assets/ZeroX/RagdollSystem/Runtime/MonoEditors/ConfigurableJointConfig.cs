using UnityEngine;

namespace ZeroX.RagdollSystem.MonoEditors
{
    [System.Serializable]
    public class ConfigurableJointConfig
    {
        public ConfigurableJointMotion xMotion = ConfigurableJointMotion.Locked;
        public ConfigurableJointMotion yMotion = ConfigurableJointMotion.Locked;
        public ConfigurableJointMotion zMotion = ConfigurableJointMotion.Locked;
		
        public ConfigurableJointMotion angularXMotion = ConfigurableJointMotion.Free;
        public ConfigurableJointMotion angularYMotion = ConfigurableJointMotion.Free;
        public ConfigurableJointMotion angularZMotion = ConfigurableJointMotion.Free;

        public RotationDriveMode rotationDriveMode = RotationDriveMode.XYAndZ;
        
        public JointDriveConfig angularXDrive = new JointDriveConfig();
        public JointDriveConfig angularYZDrive = new JointDriveConfig();

        public bool enablePreprocessing = true;
    }
}