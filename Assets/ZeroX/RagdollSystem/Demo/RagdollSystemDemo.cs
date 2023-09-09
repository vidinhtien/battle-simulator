using System;
using UnityEngine;

namespace ZeroX.RagdollSystem.Demo
{
    public class RagdollSystemDemo : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private Rigidbody rotatePivot;
        [SerializeField] private Transform target;
        [SerializeField] private Avatar avatar;
        [SerializeField] private Rigidbody rigidbody;


        [ContextMenu("DisableAllAutoConfigureConnectedAnchor")]
        public void DisableAllAutoConfigureConnectedAnchor()
        {
            
            var joints = root.GetComponentsInChildren<ConfigurableJoint>();
            foreach (var joint in joints)
            {
                UnityEditor.Undo.RecordObject(joint, "Change autoConfigureConnectedAnchor");
                
                joint.autoConfigureConnectedAnchor = true;
                
                UnityEditor.EditorUtility.SetDirty(joint);
            }
        }

        private void FixedUpdate()
        {
            //rotatePivot.MoveRotation(target.rotation);
        }

        [ContextMenu("Log Avatar")]
        public void LogAvatar()
        {
            Debug.Log(avatar.humanDescription.human.Length == avatar.humanDescription.skeleton.Length);
            return;
            foreach (var humanBone in avatar.humanDescription.human)
            {
                Debug.LogFormat("HumanName: {0} - BoneName: {1}", humanBone.humanName, humanBone.boneName);
            }
        }

        [ContextMenu("Log rigidbody")]
        public void LogRigidbody()
        {
            Debug.Log(rigidbody.constraints);
        }
    }
}