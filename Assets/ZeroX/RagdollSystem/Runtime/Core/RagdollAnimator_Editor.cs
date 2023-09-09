#if UNITY_EDITOR

using System.Linq;
using UnityEngine;

namespace ZeroX.RagdollSystem
{
    public partial class RagdollAnimator : MonoBehaviour
    {
        private static Transform FindChildRecursive_WithName(Transform parent, string childName, bool includeParent, bool includeDeActive)
        {
            if (includeParent)
            {
                if (parent.name == childName)
                    return parent;
            }
            
            foreach (Transform child in parent)
            {
                if(includeDeActive == false)
                    if(child.gameObject.activeSelf == false)
                        continue;
                
                if(child.name == childName)
                {
                    return child;
                }
                
                var result = FindChildRecursive_WithName(child, childName, false, includeDeActive);
                if (result != null)
                    return result;
            }

            return null;
        }
        
        
        
        [ContextMenu("Auto Fill List Muscle Data")]
        private void Editor_AutoFillListMuscleData()
        {
            if (targetPose == null)
            {
                Debug.LogError("Root target null");
                return;
            }
            
            if (followPose == null)
            {
                Debug.LogError("Root follow null");
                return;
            }


            
            UnityEditor.Undo.RecordObject(this, "Change List Muscle Data");
            listMuscleData.Clear();
            
            
            var joints = followPose.GetComponentsInChildren<Joint>(false);
            foreach (var joint in joints)
            {
                Transform followBone = joint.transform;
                Transform targetBone = FindChildRecursive_WithName(targetPose, followBone.name, true, false);

                if (targetBone == null)
                {
                    Debug.LogError("Cannot find targetBone for followBone: " + followBone.name);
                    continue;
                }

                //Add vào list
                MuscleData muscleData = new MuscleData();
                muscleData.targetBone = targetBone;
                muscleData.followBone = followBone;
                listMuscleData.Add(muscleData);
            }
            
            
            //Sort
            listMuscleData = listMuscleData.OrderBy(d => Editor_GetTransformDepth(d.followBone)).ToList();
            

            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        
        [ContextMenu("Sort List Muscle Data By Depth")]
        private void Editor_SortListMuscleDataByDepth()
        {
            UnityEditor.Undo.RecordObject(this, "Sort List Muscle Data");
            
            listMuscleData = listMuscleData.OrderBy(d => Editor_GetTransformDepth(d.followBone)).ToList();
            
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private int Editor_GetTransformDepth(Transform trans)
        {
            int depth = 0;
            
            trans = trans.parent;
            while (trans != null)
            {
                depth++;
                trans = trans.parent;
            }

            return depth;
        }
    }
}

#endif