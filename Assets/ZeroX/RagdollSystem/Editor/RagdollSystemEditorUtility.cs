using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.RagdollSystem.Editors
{
    public static class RagdollSystemEditorUtility
    {
        public static int GetTransformDepth(Transform trans)
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
        
        public static Transform FindChildRecursive_WithName(Transform parent, string childName, bool includeParent, bool includeDeActive)
        {
            if (includeParent)
            {
                if (parent.name == childName)
                {
                    if(includeDeActive)
                        return parent;
                    else
                    {
                        if (parent.gameObject.activeSelf)
                            return parent;
                    }
                }
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
        
        public static void FindAllChildRecursive_WithName(Transform parent, string childName, bool includeParent, bool includeDeActive, List<Transform> listResult)
        {
            if (includeParent)
            {
                if (parent.name == childName)
                {
                    if(includeDeActive)
                        listResult.Add(parent);
                    else
                    {
                        if (parent.gameObject.activeSelf)
                            listResult.Add(parent);
                    }
                }
            }
            
            foreach (Transform child in parent)
            {
                if(includeDeActive == false)
                    if(child.gameObject.activeSelf == false)
                        continue;
                
                if(child.name == childName)
                {
                    listResult.Add(child);
                }
                
                FindAllChildRecursive_WithName(child, childName, false, includeDeActive, listResult);
            }
        }

        private static void GetChild_Recursive(Transform parent, bool includeParent, bool includeDeActive, List<Transform> listResult)
        {
            if (includeParent)
            {
                if(includeDeActive)
                    listResult.Add(parent);
                else
                {
                    if (parent.gameObject.activeSelf)
                        listResult.Add(parent);
                }
            }
            
            foreach (Transform child in parent)
            {
                if(includeDeActive == false)
                    if(child.gameObject.activeSelf == false)
                        continue;
                
                listResult.Add(child);
                
                GetChild_Recursive(child, false, includeDeActive, listResult);
            }
        }
        
        public static List<Transform> GetAllChild(Transform parent, bool includeParent, bool includeDeActive)
        {
            List<Transform> listResult = new List<Transform>();
            GetChild_Recursive(parent, includeParent, includeDeActive, listResult);
            return listResult;
        }

        public static TComp FindRootComponent<TComp>(Transform rootTransformToFind) where TComp : Component
        {
            var components = rootTransformToFind.GetComponentsInChildren<TComp>();
            foreach (var component in components)
            {
                Transform componentParent = component.transform.parent;
                if(componentParent == null)
                    return component;


                List<TComp> listComponentInParent = componentParent.GetComponentsInParent<TComp>().ToList();
                
                listComponentInParent.RemoveAll(j => j.transform.IsChildOf(rootTransformToFind) == false && j.transform != rootTransformToFind);
                if (listComponentInParent.Count > 0)
                    continue;

                
                int componentCount = 0;
                foreach (Transform child in componentParent)
                {
                    if (child.GetComponent<TComp>() != null)
                        componentCount++;
                }

                if (componentCount == 1)
                    return component;

                if (componentCount == 0)
                {
                    Debug.LogError("Component count cannot is 0 -> check");
                    continue;
                }
            }

            return null;
        }
        
        public static ConfigurableJoint FindRootJoint(RagdollAnimator ragdollAnimator)
        {
            return FindRootComponent<ConfigurableJoint>(ragdollAnimator.FollowPose);
        }
        
        public static Rigidbody FindRootRigidbody(RagdollAnimator ragdollAnimator)
        {
            return FindRootComponent<Rigidbody>(ragdollAnimator.FollowPose);
        }
        
        public static List<MuscleData> GenerateListMuscleData(RagdollAnimator ragdollAnimator, bool removeRootJoint)
        {
            Transform rootTarget = ragdollAnimator.TargetPose;
            Transform rootFollow = ragdollAnimator.FollowPose;
            

            if (rootTarget == null)
            {
                Debug.LogError("Root target null");
                return null;
            }
            
            if (rootFollow == null)
            {
                Debug.LogError("Root follow null");
                return null;
            }


            
            
            List<MuscleData> listMuscleData = new List<MuscleData>();
            var joints = rootFollow.GetComponentsInChildren<Joint>(false);
            
            foreach (var joint in joints)
            {
                Transform followBone = joint.transform;
                Transform targetBone = FindChildRecursive_WithName(rootTarget, followBone.name, true, true);

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
            
            
            //Remove Root Joint
            if (removeRootJoint)
            {
                ConfigurableJoint rootJoint = FindRootJoint(ragdollAnimator);
                if (rootJoint != null)
                {
                    listMuscleData.RemoveAll(d => d.followBone == rootJoint.transform);
                }
            }
            

            //Sort
            listMuscleData = listMuscleData.OrderBy(d => GetTransformDepth(d.followBone)).ToList();
            
            
            //Result
            return listMuscleData;
        }
    }
}