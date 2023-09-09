using System.Collections.Generic;
using UnityEngine;
using ZeroX.FsmSystem;

namespace BattleSimulatorV2.StateBehaviours
{
    public class DisconnectAllJoint : MonoStateBehaviour
    {
        public List<Joint> listJointToDisconnect = new List<Joint>();

        
        
        
        public override void OnStateEnter()
        {
            foreach (var joint in listJointToDisconnect)
            {
                joint.connectedBody = null;
            }
        }
        
        
#if UNITY_EDITOR
        [ContextMenu("Auto Fill List Joint To Disconnect")]
        private void Editor_AutoFillListJointToDisconnect()
        {
            UnityEditor.Undo.RecordObject(this, "AutoFillListJointToDisconnect");
            
            var joints = transform.root.GetComponentsInChildren<Joint>();
            
            listJointToDisconnect.Clear();
            listJointToDisconnect.AddRange(joints);
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}