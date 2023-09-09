using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using ZeroX.RagdollSystem.MonoEditors;

namespace ZeroX.RagdollSystem.Editors {

	[CustomEditor(typeof(RagdollEditor))]
	public class RagdollEditorInspector : Editor {
		
		public RagdollEditor script { get { return target as RagdollEditor; }}
		private const float buttonInlineWidth = 60;
		private const float indentWidth = 20;
		private static bool rigidbodyConfigFoldout = false;
		private static bool configurableJointConfigFoldout = false;
		private static bool physicMaterialConfigFoldout = false;
		
		
		//Rigidbody Config
		public SerializedProperty listRigidbodySp;
		public SerializedProperty rigidbodyConfigSp;
		public SerializedProperty massMultiplierSp;
		public SerializedProperty totalMassSp;
		public SerializedProperty dragSp;
		public SerializedProperty angularDragSp;
		public SerializedProperty useGravitySp;
		public SerializedProperty isKinematicSp;
		public SerializedProperty interpolateSp;
		public SerializedProperty collisionDetectionSp;
		
		
		//Configurable Joint Config
		public SerializedProperty listConfigurableJointSp;
		public SerializedProperty configurableJointConfigSp;
		
		public SerializedProperty xMotionSp;
		public SerializedProperty yMotionSp;
		public SerializedProperty zMotionSp;

		public SerializedProperty angularXMotionSp;
		public SerializedProperty angularYMotionSp;
		public SerializedProperty angularZMotionSp;
		
		public SerializedProperty rotationDriveModeSp;
		
		public SerializedProperty angularXDriveSp;
		public SerializedProperty angularXDrive_positionSpringSp;
		public SerializedProperty angularXDrive_positionDamperSp;
		public SerializedProperty angularXDrive_maximumForceSp;
		
		public SerializedProperty angularYZDriveSp;
		public SerializedProperty angularYZDrive_positionSpringSp;
		public SerializedProperty angularYZDrive_positionDamperSp;
		public SerializedProperty angularYZDrive_maximumForceSp;
		
		public SerializedProperty enablePreprocessingSp;

		
		//PhysicMaterial Config
		public SerializedProperty feetPhysicMaterialSp;
		
		
		
		private Transform t;
		private bool isDragging;
		
		
		
		
		

		public override void OnInspectorGUI() 
		{
			//DrawDefaultInspector();
			//GUILayout.Space(10);

			serializedObject.Update();
			
			DrawRigidbodyConfig();
			GUILayout.Space(15);
			DrawConfigurableJointConfig();
			GUILayout.Space(15);
			DrawPhysicMaterialConfig();
			
			serializedObject.ApplyModifiedProperties();

			GUILayout.Space(20);
			GUILayout.Label("Use the Scene View gizmos to edit Colliders and Joint limits", EditorStyles.miniLabel);
		}

		private Rigidbody[] GetRigidbodies()
		{
			return script.listRigidbody.ToArray();
		}
		
		private ConfigurableJoint[] GetConfigurableJoints()
		{
			return script.listConfigurableJoint.ToArray();
		}

		void OnEnable() {
			t = script.transform;
		}

		void OnDisable() {
			if (script == null) return;

			Collider[] colliders = script.transform.GetComponentsInChildren<Collider>();
			foreach (Collider collider in colliders) collider.enabled = true;
		}

		void OnDestroy() {
			if (t == null) return;

			Collider[] colliders = t.GetComponentsInChildren<Collider>();
			foreach (Collider collider in colliders) collider.enabled = true;
		}

		void OnSceneGUI() 
		{
			Rigidbody[] rigidbodies = script.transform.GetComponentsInChildren<Rigidbody>();
			if (rigidbodies.Length == 0) return;

			Collider[] colliders = script.transform.GetComponentsInChildren<Collider>();

			if (script.selectedRigidbody == null) script.selectedRigidbody = rigidbodies[0];
			if (script.selectedCollider == null && colliders.Length > 0) script.selectedCollider = colliders[0];

			Rigidbody symmetricRigidbody = script.symmetry && script.selectedRigidbody != null? SymmetryTools.GetSymmetric(script.selectedRigidbody, rigidbodies, script.transform): null;
			Collider symmetricCollider = script.symmetry && script.selectedCollider != null? SymmetryTools.GetSymmetric(script.selectedCollider, colliders, script.transform): null;

			Joint joint = script.selectedRigidbody != null? script.selectedRigidbody.GetComponent<Joint>(): null;
			Joint symmetricJoint = joint != null && symmetricRigidbody != null? symmetricRigidbody.GetComponent<Joint>(): null;

			// Selected
			Transform selected = script.mode == RagdollEditor.Mode.Colliders? (script.selectedCollider != null? script.selectedCollider.transform: null): (script.selectedRigidbody != null? script.selectedRigidbody.transform: null);

			if (selected != null) {
				Handles.BeginGUI();		
				GUILayout.BeginVertical(new GUIContent("Ragdoll Editor", string.Empty), "Window", GUILayout.Width(200), GUILayout.Height(20));
				GUILayout.BeginHorizontal();
				GUILayout.Label(selected.name);
				if (GUILayout.Button("Select GameObject", EditorStyles.miniButton)) {
					Selection.activeGameObject = selected.gameObject;
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(10);

				//GUILayout.BeginVertical("Box");

				GUILayout.BeginHorizontal();
				GUILayout.Label("Edit Mode", GUILayout.Width(86));
				script.mode = (RagdollEditor.Mode)EditorGUILayout.EnumPopup(string.Empty, script.mode, GUILayout.Width(100));
				GUILayout.EndHorizontal();


				GUILayout.BeginHorizontal();
				GUILayout.Label("Symmetry", GUILayout.Width(86));
				script.symmetry = GUILayout.Toggle(script.symmetry, string.Empty);
				GUILayout.EndHorizontal();

				GUILayout.Space(10);

				// COLLIDERS
				if (script.mode == RagdollEditor.Mode.Colliders && selected != null) {

					if (script.selectedCollider is CapsuleCollider) {
						var capsule = script.selectedCollider as CapsuleCollider;
						var capsuleS = symmetricCollider != null? symmetricCollider.transform.GetComponent<CapsuleCollider>(): null;
							
						if (GUILayout.Button("Convert To Box Collider")) {
							ColliderTools.ConvertToBoxCollider(capsule);
							script.selectedCollider = selected.GetComponent<Collider>();
							if (capsuleS != null) ColliderTools.ConvertToBoxCollider(capsuleS);
							return;
						}

						if (GUILayout.Button("Convert To Sphere Collider")) {
							ColliderTools.ConvertToSphereCollider(capsule);
							script.selectedCollider = selected.GetComponent<Collider>();
							if (capsuleS != null) ColliderTools.ConvertToSphereCollider(capsuleS);
							return;
						}
							
						string capsuleDir = capsule.direction == 0? "X": (capsule.direction == 1? "Y": "Z");
							
						if (GUILayout.Button("Direction: " + capsuleDir)) {
							if (capsuleS == null) {
								Undo.RecordObject(capsule, "Change Capsule Direction");
							} else {
								Undo.RecordObjects(new Object[2] { capsule, capsuleS }, "Change Capsule Direction");
							}

							capsule.direction ++;
							if (capsule.direction > 2) capsule.direction = 0;
							if (capsuleS != null) capsuleS.direction = capsule.direction; 
						}
					} else if (script.selectedCollider is BoxCollider) {
						var box = script.selectedCollider as BoxCollider;
						var boxS = symmetricCollider != null? symmetricCollider.transform.GetComponent<BoxCollider>(): null;
							
						if (GUILayout.Button("Convert To Capsule Collider")) {
							ColliderTools.ConvertToCapsuleCollider(box);
							script.selectedCollider = selected.GetComponent<Collider>();
							if (boxS != null) ColliderTools.ConvertToCapsuleCollider(boxS);
							return;
						}

						if (GUILayout.Button("Convert To Sphere Collider")) {
							ColliderTools.ConvertToSphereCollider(box);
							script.selectedCollider = selected.GetComponent<Collider>();
							if (boxS != null) ColliderTools.ConvertToSphereCollider(boxS);
							return;
						}
							
						if (GUILayout.Button("Rotate Collider")) {
							if (boxS == null) {
								Undo.RecordObject(box, "Rotate Collider");
							} else {
								Undo.RecordObjects(new Object[2] { box, boxS }, "Rotate Collider");
							}

							box.size = new Vector3(box.size.y, box.size.z, box.size.x);
							if (boxS != null) boxS.size = box.size;
						}
					} else if (script.selectedCollider is SphereCollider) {
						var sphere = script.selectedCollider as SphereCollider;
						var sphereS = symmetricCollider != null? symmetricCollider.transform.GetComponent<SphereCollider>(): null;
					
						if (GUILayout.Button("Convert To Capsule Collider")) {
							ColliderTools.ConvertToCapsuleCollider(sphere);
							script.selectedCollider = selected.GetComponent<Collider>();
							if (sphereS != null) ColliderTools.ConvertToCapsuleCollider(sphereS);
							return;
						}

						if (GUILayout.Button("Convert To Box Collider")) {
							ColliderTools.ConvertToBoxCollider(sphere);
							script.selectedCollider = selected.GetComponent<Collider>();
							if (sphereS != null) ColliderTools.ConvertToBoxCollider(sphereS);
							return;
						}
					}
				}

				// JOINTS
				if (script.mode == RagdollEditor.Mode.Joints) {
					if (joint != null && (joint is CharacterJoint || joint is ConfigurableJoint)) {

						GUILayout.BeginHorizontal();
						GUILayout.Label("Connected Body");

						var lastConnectedBody = joint.connectedBody;
						var newConnectedBody = (Rigidbody)EditorGUILayout.ObjectField(joint.connectedBody, typeof(Rigidbody), true);
						if (newConnectedBody != lastConnectedBody) {
							Undo.RecordObject(joint, "Changing Joint ConnectedBody");
							joint.connectedBody = newConnectedBody;
						}

						GUILayout.EndHorizontal();

						if (joint is CharacterJoint) {
							var j = joint as CharacterJoint;
							var sJ = symmetricJoint != null && symmetricJoint is CharacterJoint? symmetricJoint as CharacterJoint: null;

							if (GUILayout.Button("Convert to Configurable")) {
								JointConverter.CharacterToConfigurable(j);

								if (sJ != null) {
									JointConverter.CharacterToConfigurable(sJ);
								}
							}

							if (GUILayout.Button("Switch Yellow/Green")) {
								JointTools.SwitchXY(ref j);
								if (sJ != null) JointTools.SwitchXY(ref sJ);
							}
							
							if (GUILayout.Button("Switch Yellow/Blue")) {
								JointTools.SwitchXZ(ref j);
								if (sJ != null) JointTools.SwitchXZ(ref sJ);
							}
							
							if (GUILayout.Button("Switch Green/Blue")) {
								JointTools.SwitchYZ(ref j);
								if (sJ != null) JointTools.SwitchYZ(ref sJ);
							}

							if (GUILayout.Button("Invert Yellow")) {
								JointTools.InvertAxis(ref joint);
								if (sJ != null) JointTools.InvertAxis(ref symmetricJoint);
							}
						}

						if (joint is ConfigurableJoint) {
							var j = joint as ConfigurableJoint;
							var sJ = symmetricJoint != null && symmetricJoint is ConfigurableJoint? symmetricJoint as ConfigurableJoint: null;

							if (GUILayout.Button("Switch Yellow/Green")) {
								JointTools.SwitchXY(ref j);
								if (sJ != null) JointTools.SwitchXY(ref sJ);
							}

							if (GUILayout.Button("Switch Yellow/Blue")) {
								JointTools.SwitchXZ(ref j);
								if (sJ != null) JointTools.SwitchXZ(ref sJ);
							}

							if (GUILayout.Button("Switch Green/Blue")) {
								JointTools.SwitchYZ(ref j);
								if (sJ != null) JointTools.SwitchYZ(ref sJ);
							}

							if (GUILayout.Button("Invert Yellow")) {
								JointTools.InvertAxis(ref joint);
								if (sJ != null) JointTools.InvertAxis(ref symmetricJoint);
							}
						}
					}
				}

				GUILayout.EndVertical();
				//GUILayout.EndArea();
				Handles.EndGUI();

				if (script.mode == RagdollEditor.Mode.Joints && joint != null) {
					if (joint is CharacterJoint) {
						var j = joint as CharacterJoint;

						SoftJointLimit lowTwistLimit = j.lowTwistLimit;
						SoftJointLimit highTwistLimit = j.highTwistLimit;
						SoftJointLimit swing1Limit = j.swing1Limit;
						SoftJointLimit swing2Limit = j.swing2Limit;

						CharacterJointInspector.DrawJoint(j);

						if (symmetricJoint != null && symmetricJoint is CharacterJoint) {
							var sJ = symmetricJoint as CharacterJoint;

							CharacterJointInspector.DrawJoint(sJ, false, 0.5f);

							// Low Twist
							if (lowTwistLimit.limit != j.lowTwistLimit.limit) {
								Undo.RecordObject(sJ, "Change Joint Limits");
								
								Vector3 lowXAxisWorld = JointTools.GetLowXAxisWorld(j);
								Vector3 lowXAxisWorldS = JointTools.GetLowXAxisWorld(sJ);
								Vector3 lowXAxisWorldMirror = SymmetryTools.Mirror(lowXAxisWorld, script.transform);
								bool low = Vector3.Dot(lowXAxisWorldMirror, lowXAxisWorldS) < 0f;
								
								JointAxis sJointAxis = JointTools.GetSymmetricJointAxis(j, JointAxis.Primary, sJ, script.transform);
								float delta = j.lowTwistLimit.limit - lowTwistLimit.limit;
								
								JointTools.ApplyXDeltaToJointLimit(ref sJ, delta, sJointAxis, low);
							}

							// High Twist
							if (highTwistLimit.limit != j.highTwistLimit.limit) {
								Undo.RecordObject(sJ, "Change Joint Limits");
								
								Vector3 highXAxisWorld = JointTools.GetHighXAxisWorld(j);
								Vector3 highXAxisWorldS = JointTools.GetHighXAxisWorld(sJ);
								Vector3 highXAxisWorldMirror = SymmetryTools.Mirror(highXAxisWorld, script.transform);
								bool low = Vector3.Dot(highXAxisWorldMirror, highXAxisWorldS) > 0f;
								
								JointAxis sJointAxis = JointTools.GetSymmetricJointAxis(j, JointAxis.Primary, sJ, script.transform);
								float delta = j.highTwistLimit.limit - highTwistLimit.limit;
								
								JointTools.ApplyXDeltaToJointLimit(ref sJ, -delta, sJointAxis, low);
							}

							// Swing 1
							if (swing1Limit.limit != j.swing1Limit.limit) {
								Undo.RecordObject(sJ, "Change Joint Limits");
								JointAxis sJointAxis = JointTools.GetSymmetricJointAxis(j, JointAxis.Secondary, sJ, script.transform);
								float delta = j.swing1Limit.limit - swing1Limit.limit;
								
								JointTools.ApplyDeltaToJointLimit(ref sJ, delta, sJointAxis);
							}

							// Swing 2
							if (swing2Limit.limit != j.swing2Limit.limit) {
								Undo.RecordObject(sJ, "Change Joint Limits");
								JointAxis sJointAxis = JointTools.GetSymmetricJointAxis(j, JointAxis.Tertiary, sJ, script.transform);
								float delta = j.swing2Limit.limit - swing2Limit.limit;
								
								JointTools.ApplyDeltaToJointLimit(ref sJ, delta, sJointAxis);
							}
						}

					} else if (joint is ConfigurableJoint) {

						var j = joint as ConfigurableJoint;

						SoftJointLimit lowAngularXLimit = j.lowAngularXLimit;
						SoftJointLimit highAngularXLimit = j.highAngularXLimit;
						SoftJointLimit angularYLimit = j.angularYLimit;
						SoftJointLimit angularZLimit = j.angularZLimit;

						ConfigurableJointInspector.DrawJoint(j);

						if (symmetricJoint != null && symmetricJoint is ConfigurableJoint) {
							var sJ = symmetricJoint as ConfigurableJoint;

							ConfigurableJointInspector.DrawJoint(sJ, false, 0.5f);
						
							// Low X
							if (lowAngularXLimit.limit != j.lowAngularXLimit.limit) {
								Undo.RecordObject(sJ, "Change Joint Limits");

								Vector3 lowXAxisWorld = JointTools.GetLowXAxisWorld(j);
								Vector3 lowXAxisWorldS = JointTools.GetLowXAxisWorld(sJ);
								Vector3 lowXAxisWorldMirror = SymmetryTools.Mirror(lowXAxisWorld, script.transform);
								bool low = Vector3.Dot(lowXAxisWorldMirror, lowXAxisWorldS) < 0f;

								JointAxis sJointAxis = JointTools.GetSymmetricJointAxis(j, JointAxis.Primary, sJ, script.transform);
								float delta = j.lowAngularXLimit.limit - lowAngularXLimit.limit;

								JointTools.ApplyXDeltaToJointLimit(ref sJ, delta, sJointAxis, low);
							}

							// High X
							if (highAngularXLimit.limit != j.highAngularXLimit.limit) {
								Undo.RecordObject(sJ, "Change Joint Limits");

								Vector3 highXAxisWorld = JointTools.GetHighXAxisWorld(j);
								Vector3 highXAxisWorldS = JointTools.GetHighXAxisWorld(sJ);
								Vector3 highXAxisWorldMirror = SymmetryTools.Mirror(highXAxisWorld, script.transform);
								bool low = Vector3.Dot(highXAxisWorldMirror, highXAxisWorldS) > 0f;

								JointAxis sJointAxis = JointTools.GetSymmetricJointAxis(j, JointAxis.Primary, sJ, script.transform);
								float delta = j.highAngularXLimit.limit - highAngularXLimit.limit;

								JointTools.ApplyXDeltaToJointLimit(ref sJ, -delta, sJointAxis, low);
							}

							// Y
							if (angularYLimit.limit != j.angularYLimit.limit) {
								Undo.RecordObject(sJ, "Change Joint Limits");
								JointAxis sJointAxis = JointTools.GetSymmetricJointAxis(j, JointAxis.Secondary, sJ, script.transform);
								float delta = j.angularYLimit.limit - angularYLimit.limit;

								JointTools.ApplyDeltaToJointLimit(ref sJ, delta, sJointAxis);
							}

							// Z
							if (angularZLimit.limit != j.angularZLimit.limit) {
								Undo.RecordObject(sJ, "Change Joint Limits");
								JointAxis sJointAxis = JointTools.GetSymmetricJointAxis(j, JointAxis.Tertiary, sJ, script.transform);
								float delta = j.angularZLimit.limit - angularZLimit.limit;

								JointTools.ApplyDeltaToJointLimit(ref sJ, delta, sJointAxis);
							}
						}
					}
				}
			}

			if (!Application.isPlaying) {
				foreach (Collider c in colliders) {
					c.enabled = script.mode == RagdollEditor.Mode.Colliders;
				}
			}

			// HANDLES
			Color color = new Color(0.2f, 0.9f, 0.5f);
			Handles.color = color;

			if (Event.current.type == EventType.MouseUp) isDragging = false;
			if (Event.current.type == EventType.MouseDown) isDragging = false;

			if (script.mode == RagdollEditor.Mode.Colliders) {

				// Select/move scale colliders
				for (int i = 0; i < colliders.Length; i++) {

					if (colliders[i] == script.selectedCollider) {

						// Moving and scaling selected colliders
						switch(Tools.current) {
						case Tool.Move:
							Vector3 oldPosition = ColliderTools.GetColliderCenterWorld(colliders[i]);
							Vector3 newPosition = Handles.PositionHandle(oldPosition, colliders[i].transform.rotation);
							if (newPosition != oldPosition) {
								if (!isDragging) {
									Undo.RecordObjects(SymmetryTools.GetColliderPair(colliders[i], symmetricCollider), "Move Colliders");
									
									isDragging = true;
								}
								
								ColliderTools.SetColliderCenterWorld(colliders[i], symmetricCollider, newPosition, script.transform);
							}
							break;
						case Tool.Scale:
							Vector3 position = ColliderTools.GetColliderCenterWorld(colliders[i]);
							Vector3 oldSize = ColliderTools.GetColliderSize(colliders[i]);
							Vector3 oldSizeS = ColliderTools.GetColliderSize(symmetricCollider);
							Vector3 newSize = Handles.ScaleHandle(oldSize, position, colliders[i].transform.rotation, HandleUtility.GetHandleSize(position));
							if (newSize != oldSize) {
								if (!isDragging) {
									Undo.RecordObjects(SymmetryTools.GetColliderPair(colliders[i], symmetricCollider), "Scale Colliders");
									
									isDragging = true;
								}
								
								ColliderTools.SetColliderSize(colliders[i], symmetricCollider, newSize, oldSize, oldSizeS, script.transform);
							}
							
							Handles.color = color;
							break;
						}

						// Dot on selected collider
						if (Tools.current != Tool.Scale) {
							Handles.color = new Color(0.8f, 0.8f, 0.8f);
							Vector3 center = ColliderTools.GetColliderCenterWorld(colliders[i]);
							float size = GetHandleSize(center);
							Inspector.CubeCap(0, center, colliders[i].transform.rotation, size);
							Handles.color = color;
						}

					} else {
						// Selecting colliders
						Vector3 center = ColliderTools.GetColliderCenterWorld(colliders[i]);
						float size = GetHandleSize(center);
						
						if (Inspector.DotButton(center, Quaternion.identity, size * 0.5f, size * 0.5f)) {
							Undo.RecordObject(script, "Change RagdollEditor Selection");
							
							script.selectedCollider = colliders[i];
						}
					}


				}
			} else {
				// Select joints
				for (int i = 0; i < rigidbodies.Length; i++) {
					
					if (rigidbodies[i] == script.selectedRigidbody) {
						// Dots on selected joints
						Handles.color = new Color(0.8f, 0.8f, 0.8f);
						Vector3 center = rigidbodies[i].transform.position;
						float size = GetHandleSize(center);
						Inspector.CubeCap(0, center, rigidbodies[i].transform.rotation, size);
						Handles.color = color;
					} else {
						// Selecting joints
						Vector3 center = rigidbodies[i].transform.position;
						float size = GetHandleSize(center);
						
						if (Inspector.DotButton(center, Quaternion.identity, size * 0.5f, size * 0.5f)) {
							Undo.RecordObject(script, "Change RagdollEditor Selection");
							
							script.selectedRigidbody = rigidbodies[i];
						}
					}
				}
			}

			Handles.color = Color.white;
		}

		private static float GetHandleSize(Vector3 position) {
			float s = HandleUtility.GetHandleSize(position) * 0.1f;
			return Mathf.Lerp(s, 0.025f, 0.2f);
		}












		
		#region Rigidbody Config
		

		private void DrawRigidbodyConfig()
		{
			rigidbodyConfigFoldout = EditorGUILayout.Foldout(rigidbodyConfigFoldout, "Rigidbody Config", true, EditorStyles.foldoutHeader);
			if(rigidbodyConfigFoldout == false)
				return;
			
			CacheSerializeProperty_Of_RigidbodyConfig();

			GUILayout.BeginHorizontal(EditorStyles.helpBox);
			GUILayout.Space(indentWidth);
			GUILayout.BeginVertical();
			
			
			DrawListRigidbody();
			
			GUILayout.Space(10);
			DrawTotalMass();
			DrawMultiMass();
			DrawSetTotalMass();
			DrawSetDrag();
			DrawSetAngularDrag();
			DrawSetUseGravity();
			DrawSetIsKinematic();
			DrawSetInterpolate();
			DrawSetCollisionDetection();
			
			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

		}
		
		private void CacheSerializeProperty_Of_RigidbodyConfig()
		{
			rigidbodyConfigSp = serializedObject.FindProperty(nameof(script.rigidbodyConfig));
			RigidbodyConfig rigidbodyConfig = script.rigidbodyConfig;

			listRigidbodySp = serializedObject.FindProperty(nameof(script.listRigidbody));
			
			massMultiplierSp = rigidbodyConfigSp.FindPropertyRelative(nameof(rigidbodyConfig.massMultiplier));
			totalMassSp = rigidbodyConfigSp.FindPropertyRelative(nameof(rigidbodyConfig.totalMass));
			dragSp = rigidbodyConfigSp.FindPropertyRelative(nameof(rigidbodyConfig.drag));
			angularDragSp = rigidbodyConfigSp.FindPropertyRelative(nameof(rigidbodyConfig.angularDrag));
			useGravitySp = rigidbodyConfigSp.FindPropertyRelative(nameof(rigidbodyConfig.useGravity));
			isKinematicSp = rigidbodyConfigSp.FindPropertyRelative(nameof(rigidbodyConfig.isKinematic));
			interpolateSp = rigidbodyConfigSp.FindPropertyRelative(nameof(rigidbodyConfig.interpolate));
			collisionDetectionSp = rigidbodyConfigSp.FindPropertyRelative(nameof(rigidbodyConfig.collisionDetection));
		}

		private void DrawListRigidbody()
		{
			EditorGUILayout.PropertyField(listRigidbodySp);
			if(GUILayout.Button("Find Rigidbodies") == false)
				return;

			var rigidbodies = script.GetComponentsInChildren<Rigidbody>();
			listRigidbodySp.ClearArray();
			listRigidbodySp.arraySize = rigidbodies.Length;

			for (int i = 0; i < rigidbodies.Length; i++)
			{
				listRigidbodySp.GetArrayElementAtIndex(i).objectReferenceValue = rigidbodies[i];
			}
		}

		private void DrawTotalMass()
		{
			Rigidbody[] rigidbodies = GetRigidbodies();
			float totalMass = rigidbodies.Sum(r => r.mass);
			
			EditorGUILayout.LabelField("Current Total Mass: " + totalMass);
		}

		private void DrawMultiMass()
		{
			GUILayout.BeginHorizontal();
			
			EditorGUILayout.PropertyField(massMultiplierSp);
			float massMultiplier = massMultiplierSp.floatValue;
			
			
			if (GUILayout.Button("Multiply", GUILayout.Width(buttonInlineWidth))) 
			{
				Rigidbody[] rigidbodies = GetRigidbodies();
				Undo.RecordObjects(rigidbodies, "Multiply Mass");

				float totalMass = 0f;
				foreach (Rigidbody r in rigidbodies) 
				{
					r.mass *= massMultiplier;
					totalMass += r.mass;
					
					EditorUtility.SetDirty(r);
				}
			}
			
			GUILayout.EndHorizontal();
		}

		private void DrawSetTotalMass()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(totalMassSp);
			float newTotalMass = totalMassSp.floatValue;
			

			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				Rigidbody[] rigidbodies = GetRigidbodies();
				Undo.RecordObjects(rigidbodies, "Set New Total Mass");
				
				float currentTotalMass = rigidbodies.Sum(r => r.mass);
				float factor = newTotalMass / currentTotalMass;

				foreach (var rigidbody in rigidbodies)
				{
					rigidbody.mass *= factor;
					EditorUtility.SetDirty(rigidbody);
				}
			}
			
			GUILayout.EndHorizontal();
		}


		private void DrawSetDrag()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(dragSp);
			float drag = dragSp.floatValue;
			

			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				Rigidbody[] rigidbodies = GetRigidbodies();
				Undo.RecordObjects(rigidbodies, "Set New Drag");
				
				foreach (var rigidbody in rigidbodies)
				{
					rigidbody.drag = drag;
					EditorUtility.SetDirty(rigidbody);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSetAngularDrag()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularDragSp);
			float angularDrag = angularDragSp.floatValue;
			

			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				Rigidbody[] rigidbodies = GetRigidbodies();
				Undo.RecordObjects(rigidbodies, "Set New Angular Drag");
				
				foreach (var rigidbody in rigidbodies)
				{
					rigidbody.angularDrag = angularDrag;
					EditorUtility.SetDirty(rigidbody);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSetUseGravity()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(useGravitySp);
			bool useGravity = useGravitySp.boolValue;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				Rigidbody[] rigidbodies = GetRigidbodies();
				Undo.RecordObjects(rigidbodies, "Set New Use Gravity");
				
				foreach (var rigidbody in rigidbodies)
				{
					rigidbody.useGravity = useGravity;
					EditorUtility.SetDirty(rigidbody);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSetIsKinematic()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(isKinematicSp);
			bool isKinematic = isKinematicSp.boolValue;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				Rigidbody[] rigidbodies = GetRigidbodies();
				Undo.RecordObjects(rigidbodies, "Set New Is Kinematic");
				
				foreach (var rigidbody in rigidbodies)
				{
					rigidbody.isKinematic = isKinematic;
					EditorUtility.SetDirty(rigidbody);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSetInterpolate()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(interpolateSp);
			RigidbodyInterpolation interpolate = (RigidbodyInterpolation)interpolateSp.enumValueIndex;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				Rigidbody[] rigidbodies = GetRigidbodies();
				Undo.RecordObjects(rigidbodies, "Set New Interpolate");
				
				foreach (var rigidbody in rigidbodies)
				{
					rigidbody.interpolation = interpolate;
					EditorUtility.SetDirty(rigidbody);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSetCollisionDetection()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(collisionDetectionSp);
			CollisionDetectionMode collisionDetectionMode = (CollisionDetectionMode)collisionDetectionSp.enumValueIndex;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				Rigidbody[] rigidbodies = GetRigidbodies();
				Undo.RecordObjects(rigidbodies, "Set New Collision Detection Mode");
				
				foreach (var rigidbody in rigidbodies)
				{
					rigidbody.collisionDetectionMode = collisionDetectionMode;
					EditorUtility.SetDirty(rigidbody);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		
		#endregion


		#region Configurable Joint Config

		private void DrawConfigurableJointConfig()
		{
			configurableJointConfigFoldout = EditorGUILayout.Foldout(configurableJointConfigFoldout, "Configurable Joint Config", true, EditorStyles.foldoutHeader);
			if(configurableJointConfigFoldout == false)
				return;
			
			CacheSerializeProperty_Of_ConfigurableJointConfig();
			
			GUILayout.BeginHorizontal(EditorStyles.helpBox);
			GUILayout.Space(indentWidth);
			GUILayout.BeginVertical();
			
			DrawListConfigurableJoint();
			
			GUILayout.Space(10);
			DrawSet_XMotion();
			DrawSet_YMotion();
			DrawSet_ZMotion();
			
			GUILayout.Space(10);
			DrawSet_AngularXMotion();
			DrawSet_AngularYMotion();
			DrawSet_AngularZMotion();
			
			GUILayout.Space(10);
			DrawSet_RotationDriveMode();
			DrawSet_AngularXDrive();
			DrawSet_AngularYZDrive();
			
			DrawSet_EnablePreprocessing();
			
			DrawButton_ApplyAllConfigurableJointConfig();
			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		
		private void CacheSerializeProperty_Of_ConfigurableJointConfig()
		{
			configurableJointConfigSp = serializedObject.FindProperty(nameof(script.configurableJointConfig));
			ConfigurableJointConfig configurableJointConfig = script.configurableJointConfig;
			JointDriveConfig jointDriveConfig = script.configurableJointConfig.angularXDrive;
			
			listConfigurableJointSp = serializedObject.FindProperty(nameof(script.listConfigurableJoint));
			
			xMotionSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.xMotion));
			yMotionSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.yMotion));
			zMotionSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.zMotion));
			
			angularXMotionSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.angularXMotion));
			angularYMotionSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.angularYMotion));
			angularZMotionSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.angularZMotion));
			
			rotationDriveModeSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.rotationDriveMode));
			
			angularXDriveSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.angularXDrive));
			angularXDrive_positionSpringSp = angularXDriveSp.FindPropertyRelative(nameof(jointDriveConfig.positionSpring));
			angularXDrive_positionDamperSp = angularXDriveSp.FindPropertyRelative(nameof(jointDriveConfig.positionDamper));
			angularXDrive_maximumForceSp = angularXDriveSp.FindPropertyRelative(nameof(jointDriveConfig.maximumForce));
			
			angularYZDriveSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.angularYZDrive));
			angularYZDrive_positionSpringSp = angularYZDriveSp.FindPropertyRelative(nameof(jointDriveConfig.positionSpring));
			angularYZDrive_positionDamperSp = angularYZDriveSp.FindPropertyRelative(nameof(jointDriveConfig.positionDamper));
			angularYZDrive_maximumForceSp = angularYZDriveSp.FindPropertyRelative(nameof(jointDriveConfig.maximumForce));
			
			enablePreprocessingSp = configurableJointConfigSp.FindPropertyRelative(nameof(configurableJointConfig.enablePreprocessing));
		}
		
		private void DrawListConfigurableJoint()
		{
			EditorGUILayout.PropertyField(listConfigurableJointSp);
			if(GUILayout.Button("Find Configurable Joints") == false)
				return;

			var configurableJoints = script.GetComponentsInChildren<ConfigurableJoint>();
			listConfigurableJointSp.ClearArray();
			listConfigurableJointSp.arraySize = configurableJoints.Length;

			for (int i = 0; i < configurableJoints.Length; i++)
			{
				listConfigurableJointSp.GetArrayElementAtIndex(i).objectReferenceValue = configurableJoints[i];
			}
		}

		private void DrawSet_XMotion()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(xMotionSp);
			ConfigurableJointMotion xMotion = (ConfigurableJointMotion)xMotionSp.enumValueIndex;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New xMotion");
				
				foreach (var joint in joints)
				{
					joint.xMotion = xMotion;
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_YMotion()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(yMotionSp);
			ConfigurableJointMotion yMotion = (ConfigurableJointMotion)yMotionSp.enumValueIndex;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New yMotion");
				
				foreach (var joint in joints)
				{
					joint.yMotion = yMotion;
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_ZMotion()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(zMotionSp);
			ConfigurableJointMotion zMotion = (ConfigurableJointMotion)zMotionSp.enumValueIndex;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New zMotion");
				
				foreach (var joint in joints)
				{
					joint.zMotion = zMotion;
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_AngularXMotion()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularXMotionSp);
			ConfigurableJointMotion angularXMotion = (ConfigurableJointMotion)angularXMotionSp.enumValueIndex;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New angularXMotion");
				
				foreach (var joint in joints)
				{
					joint.angularXMotion = angularXMotion;
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_AngularYMotion()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularYMotionSp);
			ConfigurableJointMotion angularYMotion = (ConfigurableJointMotion)angularYMotionSp.enumValueIndex;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New angularYMotion");
				
				foreach (var joint in joints)
				{
					joint.angularYMotion = angularYMotion;
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_AngularZMotion()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularZMotionSp);
			ConfigurableJointMotion angularZMotion = (ConfigurableJointMotion)angularZMotionSp.enumValueIndex;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New angularZMotion");
				
				foreach (var joint in joints)
				{
					joint.angularZMotion = angularZMotion;
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_RotationDriveMode()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(rotationDriveModeSp);
			RotationDriveMode rotationDriveMode = (RotationDriveMode)rotationDriveModeSp.enumValueIndex;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New rotationDriveMode");
				
				foreach (var joint in joints)
				{
					joint.rotationDriveMode = rotationDriveMode;
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}

		private void DrawSet_AngularXDrive()
		{
			var foldoutStyle = angularXDriveSp.isDefaultOverride ? EditorStyles.foldoutHeader : EditorStyles.foldout;
			angularXDriveSp.isExpanded = EditorGUILayout.Foldout(angularXDriveSp.isExpanded, angularXDriveSp.displayName, true, foldoutStyle);
			if(angularXDriveSp.isExpanded == false)
				return;
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(indentWidth);
			GUILayout.BeginVertical();
			DrawSet_AngularXDrive_PositionSpring();
			DrawSet_AngularXDrive_PositionDamper();
			DrawSet_AngularXDrive_MaximumForce();
			GUILayout.EndVertical();
			
			GUILayout.EndHorizontal();
		}

		private void DrawSet_AngularXDrive_PositionSpring()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularXDrive_positionSpringSp);
			float positionSpring = angularXDrive_positionSpringSp.floatValue;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New AngularXDrive positionSpring");
				
				foreach (var joint in joints)
				{
					JointDrive jointDrive = joint.angularXDrive;
					jointDrive.positionSpring = positionSpring;
					joint.angularXDrive = jointDrive;
					
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_AngularXDrive_PositionDamper()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularXDrive_positionDamperSp);
			float positionDamper = angularXDrive_positionDamperSp.floatValue;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New AngularXDrive positionDamper");
				
				foreach (var joint in joints)
				{
					JointDrive jointDrive = joint.angularXDrive;
					jointDrive.positionDamper = positionDamper;
					joint.angularXDrive = jointDrive;
					
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_AngularXDrive_MaximumForce()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularXDrive_maximumForceSp);
			float maximumForce = angularXDrive_maximumForceSp.floatValue;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New AngularXDrive maximumForce");
				
				foreach (var joint in joints)
				{
					JointDrive jointDrive = joint.angularXDrive;
					jointDrive.maximumForce = maximumForce;
					joint.angularXDrive = jointDrive;
					
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		
		
		private void DrawSet_AngularYZDrive()
		{
			var foldoutStyle = angularYZDriveSp.isDefaultOverride ? EditorStyles.foldoutHeader : EditorStyles.foldout;
			angularYZDriveSp.isExpanded = EditorGUILayout.Foldout(angularYZDriveSp.isExpanded, angularYZDriveSp.displayName, true, foldoutStyle);
			if(angularYZDriveSp.isExpanded == false)
				return;
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(indentWidth);
			GUILayout.BeginVertical();
			DrawSet_AngularYZDrive_PositionSpring();
			DrawSet_AngularYZDrive_PositionDamper();
			DrawSet_AngularYZDrive_MaximumForce();
			GUILayout.EndVertical();
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_AngularYZDrive_PositionSpring()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularYZDrive_positionSpringSp);
			float positionSpring = angularYZDrive_positionSpringSp.floatValue;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New AngularYZDrive positionSpring");
				
				foreach (var joint in joints)
				{
					JointDrive jointDrive = joint.angularYZDrive;
					jointDrive.positionSpring = positionSpring;
					joint.angularYZDrive = jointDrive;
					
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_AngularYZDrive_PositionDamper()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularYZDrive_positionDamperSp);
			float positionDamper = angularYZDrive_positionDamperSp.floatValue;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New AngularYZDrive positionDamper");
				
				foreach (var joint in joints)
				{
					JointDrive jointDrive = joint.angularYZDrive;
					jointDrive.positionDamper = positionDamper;
					joint.angularYZDrive = jointDrive;
					
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void DrawSet_AngularYZDrive_MaximumForce()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(angularYZDrive_maximumForceSp);
			float maximumForce = angularYZDrive_maximumForceSp.floatValue;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New AngularYZDrive maximumForce");
				
				foreach (var joint in joints)
				{
					JointDrive jointDrive = joint.angularYZDrive;
					jointDrive.maximumForce = maximumForce;
					joint.angularYZDrive = jointDrive;
					
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}

		private void DrawSet_EnablePreprocessing()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(enablePreprocessingSp);
			bool enablePreprocessing = enablePreprocessingSp.boolValue;
			
			
			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				ConfigurableJoint[] joints = GetConfigurableJoints();
				Undo.RecordObjects(joints, "Set New enablePreprocessing");
				
				foreach (var joint in joints)
				{
					joint.enablePreprocessing = enablePreprocessing;
					EditorUtility.SetDirty(joint);
				}
			}
			
			GUILayout.EndHorizontal();
		}

		private void DrawButton_ApplyAllConfigurableJointConfig()
		{
			if(GUILayout.Button("Apply All Configurable Joint Config") == false)
				return;

			RagdollEditor ragdollEditor = (RagdollEditor) target;
			var joints = GetConfigurableJoints();
			foreach (var joint in joints)
			{
				Undo.RecordObject(joint, "Change Joint Config");
				
				ragdollEditor.ApplyConfigurableJointConfig(joint);
				
				EditorUtility.SetDirty(joint);
			}
		}

		#endregion


		#region PhysicMaterial Config

		private void DrawPhysicMaterialConfig()
		{
			physicMaterialConfigFoldout = EditorGUILayout.Foldout(physicMaterialConfigFoldout, "Physic Material Config", true, EditorStyles.foldoutHeader);
			if(physicMaterialConfigFoldout == false)
				return;
			
			CacheSerializeProperty_Of_PhysicMaterialConfig();
			
			GUILayout.BeginHorizontal(EditorStyles.helpBox);
			GUILayout.Space(indentWidth);
			GUILayout.BeginVertical();
			
			
			DrawSet_FeetPhysicMaterial();
			
			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		
		private void CacheSerializeProperty_Of_PhysicMaterialConfig()
		{
			feetPhysicMaterialSp = serializedObject.FindProperty(nameof(script.feetPhysicMaterial));
		}

		private void DrawSet_FeetPhysicMaterial()
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(feetPhysicMaterialSp);
			Object physicMaterialObj = feetPhysicMaterialSp.objectReferenceValue;
			

			if (GUILayout.Button("Set", GUILayout.Width(buttonInlineWidth)))
			{
				PhysicMaterial physicMaterial = physicMaterialObj == null ? null : (PhysicMaterial) physicMaterialObj;

			
				//Feet
				List<Transform> listFeet = new List<Transform>();
				RagdollSystemEditorUtility.FindAllChildRecursive_WithName(script.transform, "Foot Collider", true, true, listFeet);

				foreach (var feet in listFeet)
				{
					Collider collider = feet.GetComponent<Collider>();
					if(collider == null)
						continue;

				
				
					Undo.RecordObject(collider, "Change Physic Material");
				
					collider.material = physicMaterial;
				
					EditorUtility.SetDirty(collider);
				}
			}
			
			GUILayout.EndHorizontal();
		}

		#endregion
	}
}
