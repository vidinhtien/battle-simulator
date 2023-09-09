using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ZeroX.FsmSystem.Demo
{
    [System.Serializable]
    public class Data
    {
        [SerializeField] private List<FsmGraph> listGraph = new List<FsmGraph>();
    }
    
    
    public class TestFsmSystem : MonoBehaviour
    {
        [SerializeField] private FsmGraph graph;
        [SerializeField] private Data data;
        [SerializeField] private List<GameObject> listObj;
        
        

        private void Awakea()
        {
            StartCoroutine(LateFixedUpdate());
            
            graph.ClearGraph();

            graph.AddNormalState<UnityEventState>("Init", Vector2.zero)
                .RegisterOnStateEnter(() => Debug.Log("Init Enter: " + Time.fixedTime))
                .RegisterOnStateExit(() => Debug.Log("Init Exit: " + Time.fixedTime))
                .RegisterOnStateUpdate(() => Debug.Log("Init Update: " + Time.fixedTime))
                //.RegisterOnStateFixedUpdate(() => Debug.Log("Init FixedUpdate: " + Time.fixedTime));
                .RegisterOnStateLateFixedUpdate(() => Debug.Log("Init LateFixedUpdate: " + Time.fixedTime));

            graph.AddNormalState<UnityEventStateLite>("Intro", 125, 100)
                .RegisterOnStateEnter(() => Debug.Log("Intro Enter"))
                .RegisterOnStateExit(() => Debug.Log("Intro Exit"));;
            
            graph.AddNormalState<UnityEventStateLite>("Wait Play", 250, 200)
                .RegisterOnStateEnter(() => Debug.Log("Wait Play Enter"))
                .RegisterOnStateExit(() => Debug.Log("Wait Play Exit"));;
            
            
            graph.AddTransition("Init", "Finished", "Intro");
            graph.AddTransition("Init", "Not Finished");
            
            graph.AddTransition("Intro", "Finished", "Wait Play");
            
            //graph.AddTransition("Wait Play", "Click Play");
            
            graph.SetEntryState("Init");
            graph.StartGraph();
            graph.SetTrigger("Finished");
            graph.SetTrigger("Finished");
            graph.SetTrigger("Click Play");
            graph.SetTransition("Wait Play", "Click Play", "Intro");
            graph.SetTrigger("Click Play");
            
            //FsmCenter.RegisterFsmGraph(this, graph, true, true, true, true);
            //graph.SetTrigger("Finished");
            //graph.SetTrigger("Finished");
        }

        private void Awakeb()
        {
            
            graph.ClearGraph();

            //Add Normal State
            graph.AddNormalState<UnityEventStateLite>("idle", -108, -45);
            graph.AddNormalState<UnityEventStateLite>("run", 77, 47);
            graph.AddNormalState<UnityEventStateOneUpdate>("state 3", 90, -99);

//Add Any State
            graph.AddAnyState("any state 1", -144, 111);

//Add Parallel State
            graph.AddParallelState<UnityEventStateLite>("parrllel state 1", -137, 201);
            graph.AddParallelState<MonoScriptState>("state 1", 6, 218);

//Add Transition
            graph.AddTransition("idle", "New Transition 1", "run");
            graph.AddTransition("idle", "New Transition 2", "run");
            graph.AddTransition("run", "New Transition 1", "idle");
            graph.AddTransition("state 3", "Oh god");
            graph.AddTransition("any state 1", "New Transition 1", "run");

        }

        private void Awakec()
        {
            graph.ClearGraph();
            for (int i = 0; i < 50; i++)
            {
                string stateName = "state " + i;
                graph.AddNormalState<UnityEventStateLite>(stateName, i * 100, i * 100);
            }

            for (int i = 0; i < 49; i++)
            {
                string originStateName = "state " + i;
                string targetStateName = "state " + (i + 1);

                for (int j = 0; j < 3; j++)
                {
                    string transitionName = "transition " + j;
                    graph.AddTransition(originStateName, transitionName, targetStateName);
                }
            }

            for (int i = 0; i < 50; i++)
            {
                string stateName = "any state " + i;
                graph.AddAnyState(stateName, i * 100 - 200, i * 100);

                for (int j = 0; j < 3; j++)
                {
                    string transitionName = "transition " + j;
                    graph.AddTransition(stateName, transitionName);
                }
            }
            
            for (int i = 0; i < 50; i++)
            {
                string stateName = "parallel state " + i;
                graph.AddParallelState<UnityEventStateLite>(stateName, i * 100 - 400, i * 100);
            }
        }

        private void Awaked()
        {
            //Register Normal State
            graph.GetState<UnityEventStateLite>("run")
                .RegisterOnStateExit(() =>
                {
                    Debug.Log("Exit Run");
                });
            
            graph.GetState<UnityEventStateLite>("attack")
                .RegisterOnStateExit(() =>
                {
                    Debug.Log("Exit Attack");
                });
            
            graph.StartGraph();
            graph.RemoveTransition("any state 1", "any run");
            graph.SetTrigger("any run");
        }

        private void Awake()
        {
            graph.ClearGraph();
            
            //Add Normal State
            graph.AddNormalState<ActionState>("state 1", 111, 20)
                .RegisterOnStateEnter(State1_OnStateEnter);

            graph.AddNormalState<ActionState>("state 2", 253, 98)
                .RegisterOnStateFixedUpdate(State2_OnStateFixedUpdate)
                .RegisterOnStateLateFixedUpdate(State2_OnStateLateFixedUpdate);

//Add Any State
            graph.AddAnyState("any state 1", 138, 182);


//Add Parallel State
            graph.AddParallelState<ActionState>("state 3", 36, 119)
                .RegisterOnStateEnter(State3_OnStateEnter)
                .RegisterOnStateExit(State3_OnStateExit);

//Add Transition
            #region Add Transition
            graph.AddTransition("state 1", "New Transition 1", "state 2");
            graph.AddTransition("state 2", "New Transition 1", "state 1");
            #endregion

            graph.SetEntryState("state 1");
        }

        private void Start()
        {
            graph.StartGraph();

            graph.SetTrigger("linh tinh");
        }

        private void State1_OnStateEnter()
        {
            Debug.Log("On Enter State 1");
        }

        private void State2_OnStateFixedUpdate()
        {
    
        }

        private void State2_OnStateLateFixedUpdate()
        {
    
        }

        private void State3_OnStateEnter()
        {
    
        }

        private void State3_OnStateExit()
        {
    
        }


        private void OnDestroy()
        {
            Debug.Log("On Destroy");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Dictionary<string, long> dict = new Dictionary<string, long>();
                for (int i = 0; i < 200; i++)
                {
                    dict.Add("state " + i, i + 10000);
                }
                
                Stopwatch st = new Stopwatch();
                st.Start();
                for (int i = 0; i < 99999; i++)
                {
                    //long a = dict["state 188"];
                    
                    graph.SetTrigger("Finished");
                }
                st.Stop();
                Debug.Log("Set Trigger: " + st.ElapsedMilliseconds);
            }
            //graph.UpdateGraph();
        }

        private void LateUpdate()
        {
            //graph.LateUpdateGraph();
        }

        private void FixedUpdate()
        {
            //graph.FixedUpdateGraph();
        }

        IEnumerator LateFixedUpdate()
        {
            var waitFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitFixedUpdate;
                //graph.LateFixedUpdateGraph();
            }
        }

        private void OnDisable()
        {
            Debug.Log("On Disable");
        }

        public void OnStateEnter(string message)
        {
            Debug.Log("OnStateEnter: " + message);
        }
        
        public void OnStateExit(string message)
        {
            Debug.Log("OnStateExit: " + message);
        }

        public void Log(string message)
        {
            Debug.Log(message);
        }

        [ContextMenu("Log Enable")]
        public void LogEnable()
        {
            Debug.Log("Enable: " + enabled);
        }
    }
}