using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.RagdollSystem.Editors
{
    [CustomEditor(typeof(BoundRagdollTransformSyncer))]
    [CanEditMultipleObjects]
    public class BoundRagdollTransformSyncerEditor : Editor
    {
        private SerializedProperty ragdollAnimatorSp;
        private SerializedProperty syncTargetSp;
        private SerializedProperty listPointSp;
        private SerializedProperty listIgnoreSyncSp;
        private SerializedProperty syncModeSp;
        private SerializedProperty syncSpeedSp;
        private SerializedProperty syncSmoothTimeSp;
        
        private void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            ragdollAnimatorSp = serializedObject.FindProperty("ragdollAnimator");
            syncTargetSp = serializedObject.FindProperty("syncTarget");
            listPointSp = serializedObject.FindProperty("listPoint");
            listIgnoreSyncSp = serializedObject.FindProperty("listIgnoreSync");
            
            syncModeSp = serializedObject.FindProperty("syncMode");
            syncSpeedSp = serializedObject.FindProperty("syncSpeed");
            syncSmoothTimeSp = serializedObject.FindProperty("syncSmoothTime");


            EditorGUILayout.PropertyField(ragdollAnimatorSp);
            EditorGUILayout.PropertyField(syncTargetSp);
            EditorGUILayout.PropertyField(listPointSp);
            EditorGUILayout.PropertyField(listIgnoreSyncSp);
            
            GUILayout.Space(10);
            GUILayout.Label("Sync Configs", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(syncModeSp);

            if (syncModeSp.enumValueIndex == (int) BoundRagdollTransformSyncer.SyncMode.Lerp)
            {
                EditorGUILayout.PropertyField(syncSpeedSp);
            }
            else if (syncModeSp.enumValueIndex == (int) BoundRagdollTransformSyncer.SyncMode.MoveTowards)
            {
                EditorGUILayout.PropertyField(syncSpeedSp);
            }
            else if (syncModeSp.enumValueIndex == (int) BoundRagdollTransformSyncer.SyncMode.SmoothDamp)
            {
                EditorGUILayout.PropertyField(syncSpeedSp);
                EditorGUILayout.PropertyField(syncSmoothTimeSp);
            }

            GUILayout.Space(20);
            GUILayout.Label("Editors", EditorStyles.boldLabel);
            
            DrawButton_AutoFillListPoint();
            
            serializedObject.ApplyModifiedProperties();
        }


        private void DrawButton_AutoFillListPoint()
        {
            if(GUILayout.Button("Fill List Point") == false)
                return;

            foreach (BoundRagdollTransformSyncer transformSyncer in targets)
            {
                AutoFillListPoint(transformSyncer);
            }
        }

        void AutoFillListPoint(BoundRagdollTransformSyncer transformSyncer)
        {
            if (transformSyncer.RagdollAnimator == null)
            {
                Debug.LogError("Ragdoll Animator null", transformSyncer);
                return;
            }

            List<Transform> listPoint = GenerateListPoint(transformSyncer.RagdollAnimator);
            
            Undo.RecordObject(transformSyncer, "Change List Point");
            
            transformSyncer.ListPoint.Clear();
            transformSyncer.ListPoint.AddRange(listPoint);
            
            EditorUtility.SetDirty(transformSyncer);
        }

        List<Transform> GenerateListPoint(RagdollAnimator ragdollAnimator)
        {
            List<Transform> listPoint = new List<Transform>();


            listPoint.Add(ragdollAnimator.GetFollowBoneByMuscleName("left_foot"));
            listPoint.Add(ragdollAnimator.GetFollowBoneByMuscleName("right_foot"));
            
            listPoint.Add(ragdollAnimator.GetFollowBoneByMuscleName("left_hand"));
            listPoint.Add(ragdollAnimator.GetFollowBoneByMuscleName("right_hand"));
            
            listPoint.Add(ragdollAnimator.GetFollowBoneByMuscleName("hip"));
            
            listPoint.Add(ragdollAnimator.GetFollowBoneByMuscleName("head"));

            listPoint.RemoveAll(p => p == null);

            return listPoint;
        }
    }
}