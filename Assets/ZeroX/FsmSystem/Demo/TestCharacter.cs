using UnityEngine;

namespace ZeroX.FsmSystem.Demo
{
    public class TestCharacter : MonoBehaviour
    {
        [SerializeField] private FsmGraph graph;

        private void Awake()
        {
            InitGraph();
            // FsmCenter.RegisterFsmGraph(this, graph, true, true, true, true);
            // graph.GetState<UnityEventState>("run")
            //     .RegisterOnStateLateFixedUpdate(Run_OnStateLateFixedUpdate)
            //     .RegisterOnStateUpdate(Run_OnStateUpdate);
            graph.StartGraph();
            
            //graph.SetTrigger("run");
        }

        void InitGraph()
        {
            graph.ClearGraph();
            FsmCenter.RegisterFsmGraph(this, graph, true, true, true, true);
            
            //Add Normal State
            graph.AddNormalState<ActionState>("fall_back", 320, 95)
                .RegisterOnStateEnter(FallBack_OnStateEnter);

            graph.AddNormalState<ActionState>("idle", 294, -90)
                .RegisterOnStateEnter(Idle_OnStateEnter);

            graph.AddNormalState<ActionState>("run", 454, -12)
                .RegisterOnStateLateFixedUpdate(Run_OnStateLateFixedUpdate)
                .RegisterOnStateUpdate(Run_OnStateUpdate);

//Add Any State
            graph.AddAnyState("any state 1", 150, 7);


//Add Transition
            #region Add Transition
            graph.AddTransition("fall_back", "New Transition 1");
            graph.AddTransition("idle", "run", "run");
            graph.AddTransition("any state 1", "collide_barrier", "fall_back");
            #endregion

            graph.SetEntryState("idle");
        }
        
        private void Idle_OnStateEnter()
        {
            graph.SetTrigger("run");
        }

        private int count = 0;
        private void Run_OnStateLateFixedUpdate()
        {
            count++;
            
        }

        private void Run_OnStateUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                graph.SetTrigger("collide_barrier");
                Debug.Log("Set Trigger");
            }
        }

        private void FallBack_OnStateEnter()
        {
            Debug.Log("FallBack Enter");
        }
    }
}