using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public class MonoScriptState : State
    {
        [SerializeField] public List<MonoStateBehaviour> listBehaviour = new List<MonoStateBehaviour>();


        public const string fn_listBehaviour = "listBehaviour";

        #region State Method

        public override void OnStateAwake()
        {
            //Init for listStateBehaviour
            //OnBehaviourAddToState
            for (int i = 0; i < listBehaviour.Count; i++)
            {
                var behaviour = listBehaviour[i];
                if(behaviour == null)
                    continue;

                if (behaviour.State != null)
                {
                    listBehaviour.RemoveAt(i);
                    i--;
                    
                    LogErrorAddBehaviour(behaviour);
                    continue;
                }
                
                
                //Set data to behaviour
                behaviour.FsmGraph = fsmGraph;
                behaviour.State = this;

                    
                //Call OnAddToState
                try
                {
                    behaviour.OnAddToState();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }


            //Call OnStateAwake if not
            foreach (var behaviour in listBehaviour)
            {
                if (behaviour != null && behaviour.isAwaken == false)
                {
                    behaviour.isAwaken = true;
                    behaviour.OnStateAwake();
                }
            }
        }
        
        public override void OnStateEnter()
        {
            foreach (var behaviour in listBehaviour)
            {
                if(behaviour != null && behaviour.isActiveAndEnabled)
                    behaviour.OnStateEnter();
            }
        }

        public override void OnStateExit()
        {
            foreach (var behaviour in listBehaviour)
            {
                if(behaviour != null && behaviour.isActiveAndEnabled)
                    behaviour.OnStateExit();
            }
        }

        

        public override void OnStateUpdate()
        {
            foreach (var behaviour in listBehaviour)
            {
                if(behaviour != null && behaviour.isActiveAndEnabled)
                    behaviour.OnStateUpdate();
            }
        }
        
        public override void OnStateLateUpdate()
        {
            foreach (var behaviour in listBehaviour)
            {
                if(behaviour != null && behaviour.isActiveAndEnabled)
                    behaviour.OnStateLateUpdate();
            }
        }

        public override void OnStateFixedUpdate()
        {
            foreach (var behaviour in listBehaviour)
            {
                if(behaviour != null && behaviour.isActiveAndEnabled)
                    behaviour.OnStateFixedUpdate();
            }
        }

        public override void OnStateLateFixedUpdate()
        {
            foreach (var behaviour in listBehaviour)
            {
                if(behaviour != null && behaviour.isActiveAndEnabled)
                    behaviour.OnStateLateFixedUpdate();
            }
        }
        
        

        public override void OnGraphPause()
        {
            foreach (var behaviour in listBehaviour)
            {
                if(behaviour != null && behaviour.isActiveAndEnabled)
                    behaviour.OnGraphPause();
            }
        }

        public override void OnGraphResume()
        {
            foreach (var behaviour in listBehaviour)
            {
                if(behaviour != null && behaviour.isActiveAndEnabled)
                    behaviour.OnGraphResume();
            }
        }

        #endregion

        void LogErrorAddBehaviour(MonoStateBehaviour behaviour)
        {
            if(behaviour.State == this)
                Debug.LogErrorFormat("State '{0}': Can't add behaviour again because it's already in this state", Name);
            else
                Debug.LogErrorFormat("State '{0}': Can't add behaviour because it belongs to another state", Name);
        }

        public void AddBehaviour(MonoStateBehaviour behaviour)
        {
            if (behaviour.State != null)
            {
                LogErrorAddBehaviour(behaviour);
                return;
            }

            listBehaviour.Add(behaviour);
            
            
            //Set data to behaviour
            behaviour.FsmGraph = fsmGraph;
            behaviour.State = this;

            
            //Call OnAddToState
            try
            {
                behaviour.OnAddToState();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            
            //Call OnStateAwake if not
            if (behaviour.isAwaken == false)
            {
                behaviour.isAwaken = true;
                behaviour.OnStateAwake();
            }
        }

        public void RemoveBehaviour(MonoStateBehaviour behaviour)
        {
            if(behaviour.State == null)
                return;
            
            if (behaviour.State != this)
            {
                Debug.LogErrorFormat("State '{0}': Can't remove behaviour because it doesn't belong to this state", Name);
                return;
            }
            
            listBehaviour.Remove(behaviour);

            
            //Call OnRemoveFromState
            try
            {
                behaviour.OnRemoveFromState();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            
            //Remove data from behaviour
            behaviour.FsmGraph = null;
            behaviour.State = null;
        }

        public void RemoveAllBehaviour()
        {
            foreach (var behaviour in listBehaviour)
            {
                //Call OnRemoveFromState
                try
                {
                    behaviour.OnRemoveFromState();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                
                
                //Remove data from behaviour
                behaviour.FsmGraph = null;
                behaviour.State = null;
            }
            
            listBehaviour.Clear();
        }

        public T GetBehaviour<T>() where T : MonoStateBehaviour
        {
            foreach (var behaviour in listBehaviour)
            {
                if (behaviour is T castedBehaviour)
                    return castedBehaviour;
            }

            return null;
        }

        public List<T> GetBehaviours<T>() where T : MonoStateBehaviour
        {
            List<T> listCastedBehaviour = new List<T>();
            
            foreach (var behaviour in listBehaviour)
            {
                if (behaviour is T castedBehaviour)
                    listCastedBehaviour.Add(castedBehaviour);
            }

            return listCastedBehaviour;
        }
    }
}