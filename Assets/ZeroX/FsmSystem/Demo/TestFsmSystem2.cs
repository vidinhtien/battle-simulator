using UnityEngine;

namespace ZeroX.FsmSystem
{
    public class TestFsmSystem2 : MonoBehaviour
    {
        [SerializeField] private StateMachine stateMachine;

        private void Awake()
        {
            var graph = stateMachine.Graph;
            graph.ClearGraph();
            
            //Add Normal State
            graph.AddNormalState<ActionState>("state 1", -279, -110);
            graph.AddNormalState<ActionState>("state 2", -108, -30);
            graph.AddNormalState<ActionState>("state 3", 151, 81);
            graph.AddNormalState<ActionState>("state 4", 173, 222);

//Add Any State
            graph.AddAnyState("any state 1", -477, 14);
            graph.AddAnyState("any state 2", -101, 116);


//Add Transition
            #region Add Transition
            graph.AddTransition("state 1", "finished", "state 2");
            graph.AddTransition("any state 1", "any1", "state 1");
            graph.AddTransition("any state 1", "any2", "state 2");
            graph.AddTransition("any state 2", "any3", "state 3");
            graph.AddTransition("any state 2", "any4", "state 4");
            #endregion

            graph.SetEntryState("state 1");
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                Debug.Log("dsad");
                stateMachine.Graph.RemoveState("any state 1");
                Debug.Log("Remove");
            }
                
        }

        void State1_OnStateEnter()
        {
            Debug.Log("Enter State 1");
        }
        
        void State2_OnStateEnter()
        {
            Debug.Log("Enter State 2");
        }
    }
}