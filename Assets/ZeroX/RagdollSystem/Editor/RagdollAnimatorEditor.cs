using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZeroX.RagdollSystem.Editors
{
    [CustomEditor(typeof(RagdollAnimator))]
    [CanEditMultipleObjects]
    public class RagdollAnimatorEditor : Editor
    {
        private PoseClipboard poseClipboard = new PoseClipboard();
        private GUIStyle buttonStyle_WordWrap;

        private GUIStyle ButtonStyle_WordWrap
        {
            get
            {
                if (buttonStyle_WordWrap == null)
                {
                    buttonStyle_WordWrap = new GUIStyle(GUI.skin.button);
                    buttonStyle_WordWrap.wordWrap = true;
                }

                return buttonStyle_WordWrap;
            }
        }
        

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            
            
            GUILayout.Space(30);
            
            if (EditorApplication.isPlaying == false || PrefabUtility.IsPartOfPrefabAsset(target))
            {
                DrawEditors_NoPlaying_Prefab();
            }
            else
            {
                DrawEditors_Playing();
            }
        }

        void DrawEditors_NoPlaying_Prefab()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            DrawButton_FindRootFollowBone();
            DrawButton_FindRootTargetBone();
            GUILayout.EndHorizontal();
            
            //DrawButton_RemoveJointOnRootFollowBone();

            if (serializedObject.isEditingMultipleObjects == false)
            {
                GUILayout.BeginHorizontal();
                DrawButton_AutoFillListMuscleData_KeepConfig();
                DrawButton_AutoFillMuscleName();
                GUILayout.EndHorizontal();
            }

            //DrawButton_AutoFillListMuscleData();
            GUILayout.BeginHorizontal();
            DrawButton_DisableAllRendererInTarget();
            DrawButton_SetAnimator_CullingModeToAlwaysAnimate();
            GUILayout.EndHorizontal();
            
            DrawButton_SetRootJointToWorldSpace();
            
            
            GUILayout.Space(15);
            
            
            GUILayout.Label("Pose Editor");
            
            GUILayout.BeginHorizontal();
            DrawButton_CopyTargetPose();
            DrawButton_CopyFollowPose();
            GUILayout.EndHorizontal();
            
            DrawButton_CopyTargetPose_InAnimation();
            
            GUILayout.BeginHorizontal();
            DrawButton_PastePoseToFollowPose();
            DrawButton_PastePoseToTargetPose();
            GUILayout.EndHorizontal();
            
            
            
            serializedObject.ApplyModifiedProperties();
        }

        void DrawEditors_Playing()
        {
            EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);
            
            DrawButton_OnOffBalance();
            DrawButton_OnOffSyncTransform();
        }
        
        void DrawButton_FindRootFollowBone()
        {
            if (GUILayout.Button("Find Root Follow Bone", GUILayout.Height(25)) == false)
                return;

            if (serializedObject.isEditingMultipleObjects == false)
            {
                FindRootFollowBone(serializedObject, (RagdollAnimator)target);
            }
            else
            {
                foreach (var t in serializedObject.targetObjects)
                {
                    var so = new SerializedObject(t);
                    so.Update();
                    FindRootFollowBone(so, (RagdollAnimator)so.targetObject);
                    so.ApplyModifiedProperties();
                }
            }
            
        }
        
        void DrawButton_FindRootTargetBone()
        {
            if (GUILayout.Button("Find Root Target Bone", GUILayout.Height(25)) == false)
                return;
            
            if (serializedObject.isEditingMultipleObjects == false)
            {
                FindRootTargetBone(serializedObject, (RagdollAnimator)target);
            }
            else
            {
                foreach (var t in serializedObject.targetObjects)
                {
                    var so = new SerializedObject(t);
                    so.Update();
                    FindRootTargetBone(so, (RagdollAnimator)so.targetObject);
                    so.ApplyModifiedProperties();
                }
            }
        } 

        void DrawButton_RemoveJointOnRootFollowBone()
        {
            RagdollAnimator ragdollAnimator = (RagdollAnimator) target;
            if (ragdollAnimator.RootFollowBone == null)
                return;

            ConfigurableJoint joint = ragdollAnimator.RootFollowBone.GetComponent<ConfigurableJoint>();
            if(joint == null)
                return;
            
            if(GUILayout.Button("Remove Joint On Root Follow Bone", GUILayout.Height(25)) == false)
                return;

            foreach (RagdollAnimator t in targets)
            {
                RemoveJointOnRootFollowBone(t);
            }
        }

        void DrawButton_AutoFillListMuscleData_KeepConfig()
        {
            if(GUILayout.Button("Fill List Muscle Data", GUILayout.Height(25)) == false)
                return;

            foreach (var t in targets)
            {
                AutoFillListMuscleData_KeepConfig((RagdollAnimator)t);
            }
        }

        void DrawButton_AutoFillMuscleName()
        {
            if(GUILayout.Button("Fill Muscle Name", GUILayout.Height(25)) == false)
                return;
            
            foreach (var t in targets)
            {
                AutoFillMuscleName((RagdollAnimator)t);
            }
        }
        
        void DrawButton_DisableAllRendererInTarget()
        {
            if(GUILayout.Button("Disable All Renderer In Target Pose", ButtonStyle_WordWrap, GUILayout.Height(35)) == false)
                return;

            foreach (var t in targets)
            {
                DisableAllRendererInTargetPose((RagdollAnimator)t);
            }
        }
        
        void DrawButton_SetAnimator_CullingModeToAlwaysAnimate()
        {
            bool hasAnyTargetPoseAnimatorNotAlwaysAnimate = targets.Select(t => (RagdollAnimator) t)
                .Any(ra =>
                {
                    if (ra.TargetPose == null)
                        return false;

                    Animator animator = ra.TargetPose.GetComponentInChildren<Animator>();
                    if (animator == null)
                        return false;

                    if (animator.cullingMode == AnimatorCullingMode.AlwaysAnimate)
                        return false;

                    return true;
                });
            
            if(hasAnyTargetPoseAnimatorNotAlwaysAnimate == false)
                return;
            
            
            
            
            

            Color oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.42f, 1f, 0.43f);

            if(GUILayout.Button("Set Animator Culling Mode To Always Animate", ButtonStyle_WordWrap, GUILayout.Height(35)) == false)
            {
                GUI.backgroundColor = oldBgColor;
                return;
            }
            GUI.backgroundColor = oldBgColor;
            
            foreach (var t in targets)
            {
                SetAnimatorCullingModeToAlwaysAnimate((RagdollAnimator)t);
            }
        } 

        void DrawButton_SetRootJointToWorldSpace()
        {
            bool hasAnyRootJointNotConfiguredInWorldSpace = targets.Select(t => (RagdollAnimator) t)
                .Any(ra =>
                {
                    if (ra.RootFollowBone == null)
                        return false;

                    ConfigurableJoint configurableJoint = ra.RootFollowBone.GetComponentInChildren<ConfigurableJoint>();
                    if (configurableJoint == null)
                        return false;

                    if (configurableJoint.configuredInWorldSpace)
                        return false;

                    return true;
                });
            
            if(hasAnyRootJointNotConfiguredInWorldSpace == false)
                return;
            

            
            
            

            Color oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.42f, 1f, 0.43f);
            
            if (GUILayout.Button("Set Root Joint To Configured In World Space", GUILayout.Height(25)) == false)
            {
                GUI.backgroundColor = oldBgColor;
                return;
            }
            GUI.backgroundColor = oldBgColor;

            foreach (var t in targets)
            {
                SetRootJointToWorldSpace((RagdollAnimator)t);
            }
        }

        void DrawButton_CopyTargetPose()
        {
            if(serializedObject.isEditingMultipleObjects)
                return;
            
            if(GUILayout.Button("Copy Target Pose", GUILayout.Height(25)) == false)
                return;
            
            
            
            //Check các thành phần cần thiết
            RagdollAnimator ragdollAnimator = (RagdollAnimator) target;
            if (ragdollAnimator.TargetPose == null)
            {
                Debug.LogError("No Target Pose", ragdollAnimator);
                return;
            }
            
            
            
            poseClipboard.CopyTargetPose((RagdollAnimator)target);
        }
        
        void DrawButton_CopyFollowPose()
        {
            if(serializedObject.isEditingMultipleObjects)
                return;
            
            if(GUILayout.Button("Copy Follow Pose", GUILayout.Height(25)) == false)
                return;
            
            
            
            //Check các thành phần cần thiết
            RagdollAnimator ragdollAnimator = (RagdollAnimator) target;
            if (ragdollAnimator.FollowPose == null)
            {
                Debug.LogError("No Follow Pose", ragdollAnimator);
                return;
            }
            
            
            
            poseClipboard.CopyFollowPose((RagdollAnimator)target);
        }

        void DrawButton_CopyTargetPose_InAnimation()
        {
            if(serializedObject.isEditingMultipleObjects)
                return;
            
            if(GUILayout.Button("Copy Target Pose In Animation", GUILayout.Height(25)) == false)
                return;

            
            
            
            //Check các thành phần cần thiết
            RagdollAnimator ragdollAnimator = (RagdollAnimator) target;
            if (ragdollAnimator.TargetPose == null)
            {
                Debug.LogError("No Target Pose", ragdollAnimator);
                return;
            }

            Animator animator = ragdollAnimator.TargetPose.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator on Target Pose", ragdollAnimator);
                return;
            }

            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError("Animator has no controller", ragdollAnimator);
                return;
            }
            
            
            
            //Tạo Generic Menu
            GenericMenu menu = new GenericMenu();
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                menu.AddItem(new GUIContent(clip.name), false, () => CopyTargetPoseInAnimation_GenericMenu_Select(ragdollAnimator, animator, clip));
            }
            
            menu.ShowAsContext();
        }

        void DrawButton_PastePoseToTargetPose()
        {
            if(serializedObject.isEditingMultipleObjects)
                return;

            if (poseClipboard.HasData == false)
            {
                GUILayout.Button("No data to paste", GUILayout.Height(25));
                return;
            }
            
            if(GUILayout.Button("Paste To Target Pose", GUILayout.Height(25)) == false)
                return;
            
            
            
            //Check các thành phần cần thiết
            RagdollAnimator ragdollAnimator = (RagdollAnimator) target;
            if (ragdollAnimator.TargetPose == null)
            {
                Debug.LogError("No Target Pose", ragdollAnimator);
                return;
            }
            
            
            poseClipboard.PasteToTargetPose();
        }
        
        void DrawButton_PastePoseToFollowPose()
        {
            if(serializedObject.isEditingMultipleObjects)
                return;
            
            if (poseClipboard.HasData == false)
            {
                GUILayout.Button("No data to paste", GUILayout.Height(25));
                return;
            }
            
            if(GUILayout.Button("Paste To Follow Pose", GUILayout.Height(25)) == false)
                return;
            
            
            
            //Check các thành phần cần thiết
            RagdollAnimator ragdollAnimator = (RagdollAnimator) target;
            if (ragdollAnimator.FollowPose == null)
            {
                Debug.LogError("No Follow Pose", ragdollAnimator);
                return;
            }
            
            
            
            poseClipboard.PasteToFollowPose();
        }
        

        

        private void FindRootFollowBone(SerializedObject serializedObject, RagdollAnimator ragdollAnimator)
        {
            if (ragdollAnimator.FollowPose == null)
            {
                Debug.LogError("Follow Pose null", ragdollAnimator);
                return;
            }
            
            
            Rigidbody rootFollowBone = RagdollSystemEditorUtility.FindRootRigidbody(ragdollAnimator);
            if(rootFollowBone == null)
                return;

            var rootFollowBoneSp = serializedObject.FindProperty(RagdollAnimator.fn_rootFollowBone);
            rootFollowBoneSp.objectReferenceValue = rootFollowBone;
        }
        
        private void FindRootTargetBone(SerializedObject serializedObject, RagdollAnimator ragdollAnimator)
        {
            if (ragdollAnimator.TargetPose == null)
            {
                Debug.LogError("Target Pose null", ragdollAnimator);
                return;
            }
            
            if (ragdollAnimator.RootFollowBone == null)
            {
                Debug.LogError("Root Follow Bone null", ragdollAnimator);
                return;
            }


            Transform rootTargetBone = RagdollSystemEditorUtility.FindChildRecursive_WithName(ragdollAnimator.TargetPose, ragdollAnimator.RootFollowBone.name, true, true);
            if(rootTargetBone == null)
                return;

            var rootTargetBoneSp = serializedObject.FindProperty(RagdollAnimator.fn_rootTargetBone);
            rootTargetBoneSp.objectReferenceValue = rootTargetBone;
        }

        private void RemoveJointOnRootFollowBone(RagdollAnimator ragdollAnimator)
        {
            if (ragdollAnimator.RootFollowBone == null)
                return;

            
            ConfigurableJoint joint = ragdollAnimator.RootFollowBone.GetComponent<ConfigurableJoint>();
            if(joint == null)
                return;

            GameObject gameObject = joint.gameObject;
            
            Undo.DestroyObjectImmediate(joint);
  
            EditorUtility.SetDirty(gameObject);
        }

        private void AutoFillListMuscleData_KeepConfig(RagdollAnimator ragdollAnimator)
        {
            var listMuscleData = RagdollSystemEditorUtility.GenerateListMuscleData(ragdollAnimator, false);
            if(listMuscleData == null)
                return;
            
            
            //Fill vào ragdollAnimator
            var listMuscleDataSp = serializedObject.FindProperty(RagdollAnimator.fn_listMuscleData);

            int oldSize = listMuscleDataSp.arraySize;
            listMuscleDataSp.arraySize = listMuscleData.Count;

            for (int i = 0; i < listMuscleData.Count; i++)
            {
                var muscleData = listMuscleData[i];
                var muscleDataSp = listMuscleDataSp.GetArrayElementAtIndex(i);
                muscleDataSp.FindPropertyRelative(nameof(muscleData.targetBone)).objectReferenceValue = muscleData.targetBone;
                muscleDataSp.FindPropertyRelative(nameof(muscleData.followBone)).objectReferenceValue = muscleData.followBone;
                

                
                if (i >= oldSize) //Nếu là element mới
                {
                    muscleDataSp.FindPropertyRelative(nameof(muscleData.muscleStrength)).floatValue = 1;
                    muscleDataSp.FindPropertyRelative(nameof(muscleData.muscleStrengthFactor)).floatValue = 1;
                    muscleDataSp.FindPropertyRelative(nameof(muscleData.massFactor)).floatValue = 1;
                    muscleDataSp.FindPropertyRelative(nameof(muscleData.gravityFactor)).floatValue = 1;
                }
            }
        }

        private void AutoFillMuscleName(RagdollAnimator ragdollAnimator)
        {
            if (ragdollAnimator.TargetPose == null)
            {
                Debug.LogError("Target Pose null", ragdollAnimator);
                return;
            }

            Animator animator = ragdollAnimator.TargetPose.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator null", ragdollAnimator.TargetPose);
            }
            
            if (animator.avatar == null)
            {
                Debug.LogError("Avatar null", ragdollAnimator.TargetPose);
            }
            
            
            var listMuscleDataSp = serializedObject.FindProperty(RagdollAnimator.fn_listMuscleData);
            

            for (int i = 0; i < listMuscleDataSp.arraySize; i++)
            {
                var muscleData = ragdollAnimator.ListMuscleData[i];
                var muscleDataSp = listMuscleDataSp.GetArrayElementAtIndex(i);
                var nameSp = muscleDataSp.FindPropertyRelative(nameof(muscleData.name));

                nameSp.stringValue = BoneNameNormalizer.NormalizeBoneName(muscleData.targetBone, animator.avatar);
            }
        }

        private void DisableAllRendererInTargetPose(RagdollAnimator ragdollAnimator)
        {
            if (ragdollAnimator.TargetPose == null)
            {
                Debug.LogError("Root target null", ragdollAnimator);
                return;
            }
                
            var listRenderer = ragdollAnimator.TargetPose.GetComponentsInChildren<Renderer>();

            foreach (var renderer in listRenderer)
            {
                Undo.RecordObject(renderer.gameObject, "Disable Renderer");
                renderer.gameObject.SetActive(false);
                EditorUtility.SetDirty(renderer.gameObject);
            }
        }

        private void SetAnimatorCullingModeToAlwaysAnimate(RagdollAnimator ragdollAnimator)
        {
            if (ragdollAnimator.TargetPose == null)
            {
                Debug.LogError("Target Pose null", ragdollAnimator);
                return;
            }

            Animator animator = ragdollAnimator.TargetPose.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("Target Pose does not have an animator", ragdollAnimator);
                return;
            }
            
            Undo.RecordObject(animator, "Change culling mode");
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            EditorUtility.SetDirty(animator);
        }

        private void SetRootJointToWorldSpace(RagdollAnimator ragdollAnimator)
        {
            if (ragdollAnimator.RootFollowBone == null)
            {
                Debug.LogError("Root follow bone null", ragdollAnimator);
                return;
            }

            ConfigurableJoint configurableJoint = ragdollAnimator.RootFollowBone.GetComponent<ConfigurableJoint>();
            if (configurableJoint == null)
            {
                Debug.LogError("Root joint null", ragdollAnimator);
                return;
            }
            
            Undo.RecordObject(configurableJoint, "Set configuredInWorldSpace true");
            configurableJoint.configuredInWorldSpace = true;
            EditorUtility.SetDirty(configurableJoint);
        }

        private void CopyTargetPoseInAnimation_GenericMenu_Select(RagdollAnimator ragdollAnimator, Animator animator, AnimationClip animationClip)
        {
            Debug.Log("Select clip: " + animationClip.name);
            
            
            AnimationMode.StartAnimationMode();

            //Phải try catch để chắc chắn có thể StopAnimationMode, nếu ko sẽ có khả năng cao ko thể thoát khỏi animation mode dù có gọi stop sau đó
            try
            {
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(animator.gameObject, animationClip, 0);
                AnimationMode.EndSampling();
                poseClipboard.CopyTargetPose(ragdollAnimator);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            
            AnimationMode.StopAnimationMode();
        }
            
            
            
            

        void DrawButton_OnOffBalance()
        {
            RagdollAnimator ragdollAnimator = (RagdollAnimator) target;
            
            GUILayout.BeginHorizontal();
            
            if (ragdollAnimator.IsBalancing)
            {
                GUILayout.Label("Balance Status: Balancing");
                if (GUILayout.Button("Lost Balance"))
                {
                    foreach (RagdollAnimator t in targets)
                    {
                        t.LostBalance();
                    }
                }
            }
            else
            {
                GUILayout.Label("Balance Status: Not Balancing");
                if (GUILayout.Button("Keep Balance"))
                {
                    foreach (RagdollAnimator t in targets)
                    {
                        t.KeepBalance();
                    }
                }
            }
            
            GUILayout.EndHorizontal();
        }
        
        void DrawButton_OnOffSyncTransform()
        {
            RagdollAnimator ragdollAnimator = (RagdollAnimator) target;
            
            GUILayout.BeginHorizontal();
            
            if (ragdollAnimator.IsSyncingTransform)
            {
                GUILayout.Label("Sync Transform Status: Syncing");
                if (GUILayout.Button("Lost Sync Transform"))
                {
                    foreach (RagdollAnimator t in targets)
                    {
                        t.LostSyncTransform();
                    }
                }
            }
            else
            {
                GUILayout.Label("Sync Transform Status: Not Syncing");
                if (GUILayout.Button("Keep Sync Transform"))
                {
                    foreach (RagdollAnimator t in targets)
                    {
                        t.KeepSyncTransform();
                    }
                }
            }
            
            GUILayout.EndHorizontal();
        }
    }
}