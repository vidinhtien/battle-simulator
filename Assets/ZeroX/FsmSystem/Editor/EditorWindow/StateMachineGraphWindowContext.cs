using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    [System.Serializable]
    public class StateMachineGraphWindowContext : StateMachineGraphContext
    {
        public override void SetGraphSp(SerializedProperty newGraphSp)
        {
            base.SetGraphSp(newGraphSp);
            
            listSelectedStateId.Clear();
            selectedTransitionId = Transition.emptyId;
        }

        public bool HasGraph => (this.GraphSp != null && GraphOwner != null);
        
        public bool IsPrefabAsset
        {
            get => HasGraph && PrefabUtility.IsPartOfAnyPrefab(GraphSp.serializedObject.targetObject);
        }

        public bool CannotEditGraph => EditorApplication.isPlaying && !IsPrefabAsset; //Khi đang trong chế độ chơi và ko phải prefab

        public bool CanEditGraph => !CannotEditGraph;

        
        
        #region Settings

        [SerializeField] private ZoomSettings zoomSettings = new ZoomSettings();
        [SerializeField] private DragSettings dragSettings = new DragSettings();
        [System.NonSerialized] private GridSettings gridSettings = new GridSettings();
        [System.NonSerialized] private LabelSettings labelSettings = new LabelSettings();
        
        public bool IsGridEnabled
        {
            get => this.gridSettings.IsEnabled;
            set => this.gridSettings.IsEnabled = value;
        }

        public bool ShowLabels
        {
            get => this.labelSettings.IsEnabled;
            set => this.labelSettings.IsEnabled = value;
        }

        /// <summary>
        /// Gets or sets the ZoomFactor of the graph.
        /// If the value has changed, an event is fired.
        /// </summary>
        public float ZoomFactor
        {
            get => this.zoomSettings.ZoomFactor;
            set => this.zoomSettings.ZoomFactor = value;
        }

        /// <summary>
        /// Gets or sets the DragOffset of the graph.
        /// If the value has changed, an event is fired.
        /// </summary>
        public Vector2 DragOffset
        {
            get => this.dragSettings.DragOffset;
            set => this.dragSettings.DragOffset = value;
        }
        
        #endregion
        
        

        #region Selection

        public SelectionRect SelectionRect { get; private set; } = new SelectionRect();

        //State
        private readonly HashSet<long> listSelectedStateId = new HashSet<long>();

        public long FirstSelectedStateId => listSelectedStateId.Count > 0 ? listSelectedStateId.First() : State.emptyId;
        
        public int CountSelectedState()
        {
            return listSelectedStateId.Count;
        }
        
        public bool IsSelectedState(long stateId)
        {
            return listSelectedStateId.Contains(stateId);
        }
        
        public void SelectState(long stateId)
        {
            listSelectedStateId.Add(stateId);
        }

        public void UnSelectState(long stateId)
        {
            listSelectedStateId.Remove(stateId);
        }

        public void UnSelectAllState()
        {
            listSelectedStateId.Clear();
        }

        public void SelectOnlyOneState(long stateId)
        {
            listSelectedStateId.Clear();
            listSelectedStateId.Add(stateId);
        }

        public IEnumerable<long> GetListSelectedState()
        {
            foreach (var stateId in listSelectedStateId)
            {
                yield return stateId;
            }
        }

        public HashSet<long> HashSetSelectedStateId => listSelectedStateId;


        //Transition
        private long selectedTransitionId = Transition.emptyId;

        public long FirstSelectedTransitionId => selectedTransitionId;

        public int CountSelectedTransition()
        {
            if (selectedTransitionId == Transition.emptyId)
                return 0;
            else
                return 1;
        }
        
        public bool IsSelectedTransition(long transitionId)
        {
            return selectedTransitionId == transitionId;
        }
        
        public void SelectOnlyOneTransition(long transitionId)
        {
            selectedTransitionId = transitionId;
        }

        public void UnSelectAllTransition()
        {
            selectedTransitionId = Transition.emptyId;
        }

        #endregion


        
        #region Queue Edit Action

        private Queue<Action> queueEditAction = new Queue<Action>();

        public void EnqueueEditAction(Action action)
        {
            queueEditAction.Enqueue(action);
        }

        public Action DequeueEditAction()
        {
            return queueEditAction.Dequeue();
        }

        public int CountQueueEditAction => queueEditAction.Count;

        #endregion
    }
}