using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroX.RagdollSystem.Editors
{
    public class PoseClipboard
    {
        public class BoneCopiedData
        {
            public Transform targetBone;
            public Transform followBone;
            
            public Vector3 localPos;
            public Quaternion localRot;
        }

        public List<BoneCopiedData> listData = new List<BoneCopiedData>();

        public bool HasData => listData.Count > 0;
        

        public void CopyTargetPose(RagdollAnimator ragdollAnimator)
        {
            List<Transform> listTargetBone = RagdollSystemEditorUtility.GetAllChild(ragdollAnimator.TargetPose, false, true);
            
            listData.Clear();
            
            for (int i = 0; i < listTargetBone.Count; i++)
            {
                Transform targetBone = listTargetBone[i];
                Transform followBone = RagdollSystemEditorUtility.FindChildRecursive_WithName(ragdollAnimator.FollowPose, targetBone.name, false, true);
                if (followBone == null)
                {
                    Debug.LogError("Cannot find follow bone with name: " + targetBone.name);
                    listData.Clear();
                    return;
                }
                
                BoneCopiedData data = new BoneCopiedData();
                listData.Add(data);
                
                data.targetBone = targetBone;
                data.followBone = followBone;

                data.localPos = targetBone.localPosition;
                data.localRot = targetBone.localRotation;
            }
        }

        public void CopyFollowPose(RagdollAnimator ragdollAnimator)
        {
            List<Transform> listFollowBone = RagdollSystemEditorUtility.GetAllChild(ragdollAnimator.FollowPose, false, true);
            
            listData.Clear();
            
            for (int i = 0; i < listFollowBone.Count; i++)
            {
                Transform followBone = listFollowBone[i];
                Transform targetBone = RagdollSystemEditorUtility.FindChildRecursive_WithName(ragdollAnimator.FollowPose, followBone.name, false, true);
                if (targetBone == null)
                {
                    Debug.LogError("Cannot find target bone with name: " + followBone.name);
                    listData.Clear();
                    return;
                }
                
                BoneCopiedData data = new BoneCopiedData();
                listData.Add(data);
                
                data.targetBone = targetBone;
                data.followBone = followBone;

                data.localPos = followBone.localPosition;
                data.localRot = followBone.localRotation;
            }
        }

        public void PasteToTargetPose()
        {
            foreach (var data in listData)
            {
                Undo.RecordObject(data.targetBone, "PoseClipboard - Change Transform");
                
                data.targetBone.localPosition = data.localPos;
                data.targetBone.localRotation = data.localRot;
                
                EditorUtility.SetDirty(data.targetBone);
            }
        }

        public void PasteToFollowPose()
        {
            foreach (var data in listData)
            {
                Undo.RecordObject(data.followBone, "PoseClipboard - Change Transform");
                
                data.followBone.localPosition = data.localPos;
                data.followBone.localRotation = data.localRot;
                
                EditorUtility.SetDirty(data.followBone);
            }
        }
    }
}