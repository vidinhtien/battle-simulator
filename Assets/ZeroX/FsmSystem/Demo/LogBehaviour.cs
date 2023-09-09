using System;
using UnityEngine;

namespace ZeroX.FsmSystem.Demo
{
    public class LogBehaviour : MonoStateBehaviour
    {
        [SerializeField] private string message;
        [SerializeField] private string nextState;


        public override void OnStateEnter()
        {
            Debug.Log("OnStateEnter: " + message);
        }

        public override void OnStateUpdate()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
                SetTrigger(nextState);
        }
    }
}