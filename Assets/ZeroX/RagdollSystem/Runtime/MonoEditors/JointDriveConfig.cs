using UnityEngine;

namespace ZeroX.RagdollSystem.MonoEditors
{
    [System.Serializable]
    public class JointDriveConfig
    {
        public float positionSpring = 0;
        public float positionDamper = 10;
        public float maximumForce = Mathf.Infinity;

        
        
        public JointDrive ExportToJointDrive()
        {
            JointDrive jointDrive = new JointDrive();
            jointDrive.positionSpring = positionSpring;
            jointDrive.positionDamper = positionDamper;
            jointDrive.maximumForce = maximumForce;

            return jointDrive;
        }
    }
}